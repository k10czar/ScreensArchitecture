using UnityEngine;
using System.Collections.Generic;
using System;

public interface IVisualWidgetHooksDataSource
{
	IPrefabAddressableTypeHookData GetWidgetHookData( Type type );
}

[CreateAssetMenu(fileName="VisualWidgetHooks",menuName="K10/UI/UiVisualWidgetHookSystemData",order=21)]
public class UiVisualWidgetHookSystemData : ScriptableObject, IVisualWidgetHooksDataSource
{
    [SerializeField] List<VisualWidgetHookData> _widgets = new List<VisualWidgetHookData>();

    //TODO_OPTIMIZATION: Create a cache with some collection that query better than o(N) if GetPresentationScreenData start to take long

    public IPrefabAddressableTypeHookData GetWidgetHookData( Type type )
    {
        for( int i = 0; i < _widgets.Count; i++ )
        {
            var data = _widgets[i];
            var t = data.GetTypeToHook();
            if( t != type ) continue;
            return data;
        }
        return null;
    }
}
