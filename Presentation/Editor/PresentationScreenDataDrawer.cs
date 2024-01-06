using UnityEditor;
using UnityEngine;
using K10.EditorGUIExtention;
using System.Linq;

[CustomPropertyDrawer(typeof(PresentationScreenData))]
public class PresentationScreenDataDrawer : PropertyDrawer
{
    int ASSERT_BUTTON_SIZE = 60;
    Color ERROR_COLOR_TINT = Color.Lerp( Color.red, Color.yellow, .6f );

    public override void OnGUI( Rect area, SerializedProperty property, GUIContent label )
    {
        var prefab = property.FindPropertyRelative("_prefab");
        var screenType = property.FindPropertyRelative("_hookedType");
        var serializedTypeRef = new TypeReferences.Editor.Util.SerializedTypeReference( screenType );

        var type = serializedTypeRef.TypeNameAndAssembly;
        var noType = string.IsNullOrEmpty( type );

        if( noType ) GuiColorManager.New( ERROR_COLOR_TINT );

        EditorGuiIndentManager.New( 0 );
        EditorGUI.BeginChangeCheck();
        EditorGUI.PropertyField( area.HorizontalSlice( 0, 2 ), prefab, GUIContent.none);
        var changed = EditorGUI.EndChangeCheck();

        var typeRect = area.HorizontalSlice( 1, 2 );
        if( GUI.Button( typeRect.RequestLeft( ASSERT_BUTTON_SIZE ), "Assert" ) || changed ) 
        {
            AssertType( prefab, serializedTypeRef );
        }

        EditorGUI.BeginDisabledGroup( true );
        EditorGUI.PropertyField( typeRect.CutLeft( ASSERT_BUTTON_SIZE ), screenType, GUIContent.none);
        EditorGUI.EndDisabledGroup();
        EditorGuiIndentManager.Revert();

        if( noType ) GuiColorManager.Revert();
    }

    private void AssertType( SerializedProperty prefab, TypeReferences.Editor.Util.SerializedTypeReference screenType )
    {
        var trueAssetRef = prefab.FindPropertyRelative("m_AssetGUID");
        var assetGUID = trueAssetRef.stringValue;

        screenType.SetType( null );

        if( string.IsNullOrEmpty( assetGUID ) )
        {
            Debug.LogError( $"Cannot find asset: \"{assetGUID.ToStringOrNull()}\"" );
            return;
        }

        var assetPath = AssetDatabase.GUIDToAssetPath( assetGUID );
        var obj = AssetDatabase.LoadAssetAtPath<GameObject>( assetPath );

        var screenTypeInstance = obj.GetComponent<IPresentationScreenTypeGetter>();
        if( screenTypeInstance != null ) 
        {
            screenType.SetType( screenTypeInstance.GetLogicScreenType() );
            Debug.Log( $"Asserted type:{screenTypeInstance.ToStringOrNull()}" );
        }
        else
        {
            Debug.LogError( $"Cannot find any IPresentationScreenTypeGetter component on prefab:\n{string.Join( ",\n", obj.GetComponentsInChildren<MonoBehaviour>().ToList().ConvertAll( (mb) => mb.HierarchyNameOrNull() ) )}" );
        }
    }

	public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
	{
        var slh = UnityEditor.EditorGUIUtility.singleLineHeight;
		return slh * 2;
	}
}