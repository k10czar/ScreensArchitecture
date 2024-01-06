using UnityEngine;
using TypeReferences;
using System;

public interface IPrefabAddressableTypeHookData
{
    System.Type GetTypeToHook();
    UnityEngine.AddressableAssets.AssetReference Prefab { get; }
}

[System.Serializable]
public class PresentationScreenData : IPrefabAddressableTypeHookData
{
    const string ASSEMBLY = "K10.UiMvcCore.Logic";

    [SerializeField] UnityEngine.AddressableAssets.AssetReference _prefab;
    [SerializeField, Inherits(typeof(LogicUiScreen), IncludeAdditionalAssemblies = new[] { ASSEMBLY })] TypeReference _hookedType;

    public UnityEngine.AddressableAssets.AssetReference Prefab => _prefab;
	public System.Type GetTypeToHook() => _hookedType.Type;
}
