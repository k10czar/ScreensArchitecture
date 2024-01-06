using UnityEditor;

[CustomEditor(typeof(UiVisualWidgetHookSystemData))]
public class UiVisualWidgetHookSystemDataEditor : Editor
{
    // SerializedProperty _screens;
    
    // void OnEnable()
    // {
    //     _screens = serializedObject.FindProperty("_screens");
    // }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.HelpBox( "//TODO\nUse refletion to get all the implementations of IVisualWidget missing in this list to warn the user!", MessageType.Warning );
        base.OnInspectorGUI();
        // serializedObject.Update();
        // EditorGUILayout.PropertyField(_screens);
        // serializedObject.ApplyModifiedProperties();
    }
}
