using UnityEngine;


public interface IVisualWidget : IUiElementPresenter
{
}

public abstract class VisualWidgetRequestBase
{
    public abstract System.Type GetWidgetType();
    public abstract IBoolStateObserver IsStarted { get; }
    public abstract void StartInstance( IVisualWidget go );
}

public class VisualWidgetRequest<T> : VisualWidgetRequestBase, IReferenceHolder<T> where T : IVisualWidget
{
    private CachedReference<T> _widget = new CachedReference<T>();
    private BoolState _isStarted = new BoolState();
    
    public override System.Type GetWidgetType() => typeof(T);
    
    public override IBoolStateObserver IsStarted => _isStarted;
    
	public T CurrentReference => _widget.CurrentReference;
	public IEventRegister<T,IEventValidator> OnReferenceSet => _widget.OnReferenceSet;
	public IEventRegister<T> OnReferenceRemove => _widget.OnReferenceRemove;
	public IEventValidator Validator=> _widget.Validator;
	public bool IsNull => _widget.IsNull;

    public virtual void Kill()
    {
        _widget?.Kill();
        _isStarted?.Kill();
    }

    public override void StartInstance( IVisualWidget widget )
    {
        if( widget == null ) Debug.LogError( $"Widget({typeof(T)}) receive start call with null instance parameter" );
        var specificWidget = (T)widget;
        if( specificWidget == null ) Debug.LogError( $"Cannot find Widget({typeof(T)}) on {widget.ToStringOrNull()}" );
        else _widget.ChangeReference( specificWidget );
        _isStarted.SetTrue();
    }
}
