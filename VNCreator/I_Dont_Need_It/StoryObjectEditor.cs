using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
[CustomEditor(typeof(Dialog))]
public class StoryObjectEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorGUILayout.Space(40);

        if (GUILayout.Button("Open", GUILayout.Height(40)))
        {
            StoryObjectEditorWindow.Open((Dialog)target);
        }
    }
}
#endif