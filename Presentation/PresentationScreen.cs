using UnityEngine;
using System.Collections.Generic;

public interface IPresentationScreenTypeGetter
{
    System.Type GetLogicScreenType();
}

public interface IPresentationScreenLogicHook
{
    void SetHook( LogicUiScreen screen );
}

public interface IVisualWidgetRequester
{
    List<VisualWidgetRequestBase> WidgetRequests { get; }
}

public abstract class PresentationScreen<T> 
        : MonoBehaviour, IPresentationScreenTypeGetter, IPresentationScreenLogicHook, IVisualWidgetRequester
        where T : LogicUiScreen
{
    List<VisualWidgetRequestBase> _widgetRequests = null;
	public List<VisualWidgetRequestBase> WidgetRequests
	{
		get
		{
            if( _widgetRequests == null )
            {
                var widgetRequestsConstructor = WidgetRequestsConstructor;
                if( widgetRequestsConstructor != null ) _widgetRequests = new List<VisualWidgetRequestBase>( widgetRequestsConstructor );
                else _widgetRequests = new List<VisualWidgetRequestBase>();
            }
			return _widgetRequests;
		}
	}

	public virtual IEnumerable<VisualWidgetRequestBase> WidgetRequestsConstructor => null;

    public System.Type GetLogicScreenType() => typeof(T);
    protected abstract void SetLogicInstance( T t );
    public void SetHook( LogicUiScreen screen )
    {
        SetLogicInstance( screen as T );
    }
}