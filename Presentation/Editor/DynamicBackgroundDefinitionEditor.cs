using UnityEditor;
using UnityEngine;


[CustomEditor( typeof( DynamicBackgroundDefinition ) )]
public class DynamicBackgroundDefinitionEditor : Editor
{
    DynamicBackgroundController _currentBackground;

	void OnEnable()
    {
        _currentBackground = Singleton<DynamicBackgroundController>.Instance;
    }

	public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var def = target as DynamicBackgroundDefinition;

        EditorGUI.BeginDisabledGroup( def == null || _currentBackground == null );
        if( GUILayout.Button( "Show Colors on scene" ) ) 
        {
            _currentBackground.SetColors( def );
            EditorUtility.SetDirty( _currentBackground );
        }
        if( GUILayout.Button( "Copy current displayed Colors" ) )
        {
            serializedObject.Update();
            serializedObject.FindProperty( "_background" ).colorValue = _currentBackground.BackgroundColor;
            serializedObject.FindProperty( "_topBright" ).colorValue = _currentBackground.TopBrightColor;
            serializedObject.FindProperty( "_leftGradient" ).colorValue = _currentBackground.LeftGradientColor;
            serializedObject.FindProperty( "_rightGradient" ).colorValue = _currentBackground.RightGradientColor;
            serializedObject.FindProperty( "_bottomBright" ).colorValue = _currentBackground.BottomBrightColor;
            serializedObject.FindProperty( "_middleBright" ).colorValue = _currentBackground.MiddleBrightColor;
            serializedObject.ApplyModifiedProperties();
        }
        EditorGUI.EndDisabledGroup();
    }
}