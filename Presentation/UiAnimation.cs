using UnityEngine;
using UnityEngine.UI;

using static Colors.Console;

public interface IAnimationExecution : ICustomDisposableKill, IAsyncOperationObserver
{
    void Update( float deltaTime, float realDeltaTime );
}

[System.Serializable]
public class UiAnimation
{
    [SerializeField] UiAnimationEntry[] _entries;

    public UiAnimationExecution Execute()
    {
        var exec = new UiAnimationExecution( _entries );
        return exec;
    }

    public class UiAnimationExecution : IAnimationExecution
    {
        IAnimationExecution[] _anims;
        BoolState _isDone = new BoolState();

        public IBoolStateObserver IsDone => _isDone;
        public IBoolStateObserver IsStarted => TrueState.Instance;

        public UiAnimationExecution( UiAnimationEntry[] anims )
        {
            _anims = new IAnimationExecution[ anims.Length ];
            for( int i = 0; i < anims.Length; i++ )
            {
                _anims[i] = anims[i].Execute();
            }
        }

        public void Update( float deltaTime, float realDeltaTime )
        {
            bool isDone = true;
            for( int i = 0; i < _anims.Length; i++ )
            {
                var anim = _anims[i];
                if( anim == null ) continue;
                anim.Update( deltaTime, realDeltaTime );
                isDone = isDone && anim.IsDone.Value;
            }
            _isDone.Setter( isDone );
        }

		public void Kill()
		{
            for( int i = 0; i < _anims.Length; i++ )
            {
                var anim = _anims[i];
                if( anim == null ) continue;
                anim.Kill();
                _anims[i] = null;
            }
            _anims = null;
            _isDone?.Kill();
            _isDone = null;
		}
	}

    [System.Serializable]
    public class UiAnimationEntry
    {
        [SerializeField] Component _component;
        [SerializeField] ENormalizedValueInterpolation _interpolation;
        [SerializeField] float _duration;
        [SerializeField] float _delay;
        [SerializeField] float _floatValue;
        [SerializeField] Color _colorValue;
        [SerializeField] bool _animateFromValue;
        [SerializeField] bool _useRealtime;

        public IValueState<Color> GetColorIntarector()
        {
            if( _component is Image image ) 
            {
                var colorState = new ColorState( image.color );
                colorState.OnChange.Register( ( value ) => image.color = value );
                return colorState;
            }
            return null;
        }

        public IValueState<float> GetFloatIntarector()
        {
            if( _component is CanvasGroup canvas ) 
            {
                var floatState = new FloatState( canvas.alpha );
                floatState.OnChange.Register( ( value ) => canvas.alpha = value );
                return floatState;
            }
            return null;
        }

        public IAnimationExecution Execute()
        {
            if( _component is Image image ) return new UiColorAnimationExecution( this );
            else if( _component is CanvasGroup canvas ) return new UiFloatAnimationExecution( this );
            return null;
        }
        
        private abstract class BaseUiAnimationExecution : IAnimationExecution
        {
            protected UiAnimationEntry _animationData;
            IInterpolationFunction _interpolationFunction;
            protected float _animationTime;
            float _lastValue = 0;
            BoolState _isDone = new BoolState();

            public IBoolStateObserver IsDone => _isDone;
            public IBoolStateObserver IsStarted => TrueState.Instance;

            public BaseUiAnimationExecution( UiAnimationEntry data )
            {
                _animationData = data;
                var totalDuration = _animationData._delay + _animationData._duration;
                _isDone.Setter( _animationTime >= totalDuration || MathAdapter.Approximately( _animationTime, totalDuration ) );
                _interpolationFunction = _animationData._interpolation.GetFunction();
            }

            public abstract void SetValue( float value );

            public void Update( float deltaTime, float realDeltaTime )
            {
                if( _isDone.Value ) return;

                var oldValue = _animationTime;
                var dt = _animationData._useRealtime ? realDeltaTime : deltaTime;
                _animationTime += dt;
                var value = Mathf.Clamp01( ( _animationTime - _animationData._delay ) / _animationData._duration );
                var realValue = _interpolationFunction.Evaluate( value );

                var totalDuration = _animationData._delay + _animationData._duration;
                _isDone.Setter( _animationTime >= totalDuration || MathAdapter.Approximately( _animationTime, totalDuration ) );

                if( _isDone.Value ) 
                {
                    SetValue( 1 );
                    return;
                }

                if( MathAdapter.Approximately( _lastValue, realValue ) ) return;
                
                // Debug.Log( $"{_animationData._component.HierarchyNameOrNull()}.BaseUiAnimationExecution.SetValue( ( ( {_animationTime} - {_animationData._delay} ) / {_animationData._duration} ) = {value} => {realValue} )" );
                _lastValue = realValue;
                SetValue( realValue );
            }

			public virtual void Kill()
			{
				_animationData = null;
				_isDone?.Kill();
                _isDone = null;
			}
		}

        private class UiColorAnimationExecution : BaseUiAnimationExecution
        {
            IValueState<Color> _colorInteractor;
            Color _initialValue;
            Color _finalValue;

            public UiColorAnimationExecution( UiAnimationEntry data ) : base( data )
            {
                _colorInteractor = data.GetColorIntarector();
                _initialValue = data._animateFromValue ? data._colorValue : _colorInteractor.Value;
                _finalValue = data._animateFromValue ? _colorInteractor.Value : data._colorValue;
                Debug.Log( $"{data._component.HierarchyNameOrNull()}.UiColorAnimationExecution( {_initialValue} -> {_finalValue} )" );
                SetValue( IsDone.Value ? 1 : 0 );
            }

            public override void SetValue( float value )
            {
                // Debug.Log( $"{_animationData._component.HierarchyNameOrNull()}.SetValue( {value} ) @ {_animationTime}" );
                _colorInteractor.Setter( Color.Lerp( _initialValue, _finalValue, value ) );
            }
            
			public virtual void Kill()
            {
                base.Kill();
                if( _colorInteractor != null && _colorInteractor is ICustomDisposableKill cdk ) cdk.Kill();
                _colorInteractor = null;
            }
        }

        private class UiFloatAnimationExecution : BaseUiAnimationExecution
        {
            IValueState<float> _floatInteractor;
            float _initialValue;
            float _deltaValue;

            public UiFloatAnimationExecution( UiAnimationEntry data ) : base( data )
            {
                _floatInteractor = data.GetFloatIntarector();
                var finalValue = data._animateFromValue ? _floatInteractor.Value : data._floatValue;
                _initialValue = data._animateFromValue ? data._floatValue : _floatInteractor.Value;
                _deltaValue = finalValue - _initialValue;
                Debug.Log( $"{data._component.HierarchyNameOrNull().Colorfy(Names)}.{"UiFloatAnimationExecution".Colorfy(Verbs)}( {_initialValue.ToStringColored(Numbers)} -> {finalValue.ToStringColored(Numbers)} )" );
                SetValue( IsDone.Value ? 1 : 0 );
            }

            public override void SetValue( float value )
            {
                _floatInteractor.Setter( _initialValue + ( _deltaValue * value ) );
            }
            
			public virtual void Kill()
            {
                base.Kill();
                if( _floatInteractor != null && _floatInteractor is ICustomDisposableKill cdk ) cdk.Kill();
                _floatInteractor = null;
            }
        }
    }
}
