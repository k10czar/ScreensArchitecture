using UnityEngine;
using TypeReferences;

[System.Serializable]
public class VisualWidgetHookData : IPrefabAddressableTypeHookData
{
    const string ASSEMBLY = "K10.UiMvcCore.Presentation";

    [SerializeField] UnityEngine.AddressableAssets.AssetReference _prefab;
    [SerializeField, Inherits(typeof(IVisualWidget), IncludeAdditionalAssemblies = new[] { ASSEMBLY })] TypeReference _hookedType;

    public UnityEngine.AddressableAssets.AssetReference Prefab => _prefab;
	public System.Type GetTypeToHook() => _hookedType.Type;
}
