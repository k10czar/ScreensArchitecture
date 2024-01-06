using UnityEngine;
using System.Collections.Generic;
using System;

public interface IScreenHooksDataSource
{
	IPrefabAddressableTypeHookData GetPresentationScreenData( Type type );
}

[CreateAssetMenu(fileName="ScreenHooks",menuName="K10/UI/UiScreenHookSystem",order=21)]
public class UiScreenHookSystemData : ScriptableObject, IScreenHooksDataSource
{
    [SerializeField] List<PresentationScreenData> _screens = new List<PresentationScreenData>();

    //TODO_OPTIMIZATION: Create a cache with some collection that query better than o(N) if GetPresentationScreenData start to take long

    public IPrefabAddressableTypeHookData GetPresentationScreenData( Type type )
    {
        for( int i = 0; i < _screens.Count; i++ )
        {
            var data = _screens[i];
            var t = data.GetTypeToHook();
            if( t != type ) continue;
            return data;
        }
        return null;
    }
}