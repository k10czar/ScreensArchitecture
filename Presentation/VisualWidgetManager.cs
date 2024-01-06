using System.Collections.Generic;
using UnityEngine;

public class VisualWidgetManager : ICustomDisposableKill
{
    IVisualWidgetHooksDataSource _data;
    IReferenceHolder<GameObject> _currentGameObjectHolder;

    ConditionalEventsCollection _validator = new ConditionalEventsCollection();

    List<IVisualWidget> _currentWidgets = new List<IVisualWidget>();
    List<IVisualWidget> _reccurrentWidgets = new List<IVisualWidget>();
    Dictionary<System.Type,VisualWidgetRequestBase> _widgetsRequests = new Dictionary<System.Type,VisualWidgetRequestBase>();

    public VisualWidgetManager( IVisualWidgetHooksDataSource data, IReferenceHolder<GameObject> gameObjectHolder )
    {
        Debug.Log( $"<color=yellow>VisualWidgetManager</color> registered with {data.ToStringOrNull()}" );
        _data = data;
        _currentGameObjectHolder = gameObjectHolder;
        gameObjectHolder.Synchronize( _validator.Validated<GameObject>( OnGameObjectChange ) );
    }

    void OnGameObjectChange( GameObject go )
    {
        var requests = go?.GetComponent<IVisualWidgetRequester>()?.WidgetRequests ?? null;

        if( requests != null )
        {
            _reccurrentWidgets.Clear();
            _widgetsRequests.Clear();
            for( int i = 0; i < requests.Count; i++ )
            {
                var request = requests[i];
                _widgetsRequests.Add( request.GetWidgetType(), request );
            }

            for( int i = _currentWidgets.Count - 1; i >= 0; i-- )
            {
                var widget = _currentWidgets[i];
                if( _widgetsRequests.TryGetValue( widget.GetType(), out var request ) )
                {
                    Debug.Log( $"Reusing {widget.GetType()}" );
                    _currentWidgets[i] = null;
                    _reccurrentWidgets.Add( widget );
                    request.StartInstance( widget );
                }
            }
            _widgetsRequests.Clear();
        }

        for( int i = 0; i < _currentWidgets.Count; i++ )
        {
            var widget = _currentWidgets[i];
            if( widget == null ) continue;
            Debug.Log( $"Destroying {widget.GetType()}" );
            widget.HideAndDestroy();
        }
        _currentWidgets.Clear();

        if( requests != null )
        {
            for( int i = 0; i < _reccurrentWidgets.Count; i++ )
            {
                var reccurrentWidget = _reccurrentWidgets[i];
                _currentWidgets.Add( reccurrentWidget );
            }
            _reccurrentWidgets.Clear();
            
            for( int i = 0; i < requests.Count; i++ )
            {
                var request = requests[i];
                if( request.IsStarted.Value ) continue;
                var requestedType = request.GetWidgetType();
                var data = _data.GetWidgetHookData( requestedType );
                if( data == null ) 
                {
                    Debug.LogError( $"Cannot find hook for {requestedType}" );
                    continue;
                }

                data.Prefab.InstantiateAsync().ExecuteWhenItsDone<GameObject>( ( op ) => {
                    var go = op.Result;
                    var widget = go.GetComponent<IVisualWidget>();
                    if( widget == null )
                    {
                        Debug.LogError( $"Cannot find widget component on {go.HierarchyNameOrNull()} but should have {requestedType} component" );
                        GameObject.Destroy( go );
                    }
                    else
                    {
                        Debug.Log( $"Creating {widget.GetType()}" );
                        request.StartInstance( widget );
                        _currentWidgets.Add( widget );
                        widget?.Show();
                    }
                } );
            }
        }
    }

    public void Kill()
    {
        _data = null;
        _validator?.Kill();
        _currentGameObjectHolder = null;
        _currentWidgets.Clear();
    }
}
