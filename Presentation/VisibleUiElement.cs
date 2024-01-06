using UnityEngine;
using UnityEngine.AddressableAssets;

public interface IUiElementStateObserver
{
    IBoolStateObserver IsActive { get; }
    IBoolStateObserver IsFullActive { get; }
}

public interface IUiElementPresenter
{
    IAsyncOperationObserver Show();
    IAsyncOperationObserver Hide();
    IAsyncOperationObserver HideAndDestroy();
}

public enum EObjectDestroyMode
{
    AddressablesReleaseInstance,
    GameObjectDestroy,
}

public class VisibleUiElement : MonoBehaviour, IUiElementStateObserver, IUiElementPresenter
{
    [BeginReadOnlyGroup,SerializeField] protected BoolState _isActive = new BoolState();
    [BeginReadOnlyGroup,SerializeField] protected BoolState _isFullActive = new BoolState();
    [EndReadOnlyGroup]

	private IAnimationExecution _currentAnim;

    public GameObject _gameObjectOverride;

    [SerializeField] EObjectDestroyMode _destroyMode = EObjectDestroyMode.AddressablesReleaseInstance;

    [SerializeField] UiAnimation _showAnimation;
    [SerializeField] UiAnimation _hideAnimation;

	public IBoolStateObserver IsActive => _isActive;
    public IBoolStateObserver IsFullActive => _isFullActive;


    private void DestroyObject()
    {
        var obj = _gameObjectOverride ?? gameObject;

        switch( _destroyMode )
        {
            case EObjectDestroyMode.AddressablesReleaseInstance:
                Addressables.ReleaseInstance( gameObject );
            break;

            case EObjectDestroyMode.GameObjectDestroy:
                GameObject.Destroy( obj );
            break;
        }
    }

    void Update()
    {
        if( _currentAnim == null ) return;
        _currentAnim.Update( Time.deltaTime, Time.unscaledDeltaTime );
        if( _currentAnim.IsDone.Value ) TryKillCurrentAnim();
    }

    void TryKillCurrentAnim()
    {
        if( _currentAnim == null ) return;
        _currentAnim.Kill();
        _currentAnim = null;
    }
    
    public virtual IAsyncOperationObserver Show()
    {
        _isActive.SetTrue();
        TryKillCurrentAnim();
        _currentAnim = _showAnimation.Execute();
        _currentAnim.ExecuteWhenOperationEnd( _isFullActive.SetTrue );
        return _currentAnim;
    }

    public virtual IAsyncOperationObserver Hide() 
    {
        _isFullActive.SetFalse();
        TryKillCurrentAnim();
        _currentAnim = _hideAnimation.Execute();
        _currentAnim.ExecuteWhenOperationEnd( _isActive.SetFalse );
        return _currentAnim;
    }

    public virtual IAsyncOperationObserver HideAndDestroy() 
    {
        _isFullActive.SetFalse();
        TryKillCurrentAnim();
        _currentAnim = _hideAnimation.Execute();
        _currentAnim.ExecuteWhenOperationEnd( _isActive.SetFalse );
        _currentAnim.ExecuteWhenOperationEnd( DestroyObject );
        return _currentAnim;
    }
}
