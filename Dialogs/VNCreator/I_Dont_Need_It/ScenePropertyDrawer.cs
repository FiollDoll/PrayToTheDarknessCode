using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(SceneAttribute))]
public class ScenePropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        SceneAsset sceneObject = AssetDatabase.LoadAssetAtPath<SceneAsset>(property.stringValue);
        SceneAsset scene = (SceneAsset)EditorGUI.ObjectField(position, label, sceneObject, typeof(SceneAsset), true);
        property.stringValue = AssetDatabase.GetAssetPath(scene);
    }
}
#endif