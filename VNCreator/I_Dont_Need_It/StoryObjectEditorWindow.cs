using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

#if UNITY_EDITOR
public class StoryObjectEditorWindow : EditorWindow
{
    StoryObject storyObj;
    ExtendedGraphView graphView;
    SaveUtility save = new SaveUtility();

    private Vector2 mousePosition = new Vector2();

    public static void Open(StoryObject _storyObj)
    {
        StoryObjectEditorWindow window = GetWindow<StoryObjectEditorWindow>("Story");
        window.storyObj = _storyObj;
        window.CreateGraphView(_storyObj.nodes == null ? 0 : 1);
        window.minSize = new Vector2(200, 100);
    }

    void MouseDown(MouseDownEvent _e)
    {
        if (_e.button == 1)
        {
            mousePosition = Event.current.mousePosition;
            GenericMenu _menu = new GenericMenu();
            _menu.AddItem(new GUIContent("Add Node"), false,
                () => graphView.GenerateNode("", mousePosition, 1, false, false));
            _menu.AddItem(new GUIContent("Add Node (2 Choices)"), false,
                () => graphView.GenerateNode("", mousePosition, 2, false, false));
            _menu.AddItem(new GUIContent("Add Node (3 Choices)"), false,
                () => graphView.GenerateNode("", mousePosition, 3, false, false));
            _menu.AddItem(new GUIContent("Add Node (Start)"), false,
                () => graphView.GenerateNode("", mousePosition, 1, true, false));
            _menu.AddItem(new GUIContent("Add Node (End)"), false,
                () => graphView.GenerateNode("", mousePosition, 1, false, true));
            _menu.AddItem(new GUIContent("Save"), false, () => save.SaveGraph(storyObj, graphView));
            _menu.ShowAsContext();
        }
    }

    void CreateGraphView(int _nodeCount)
    {
        graphView = new ExtendedGraphView();
        graphView.RegisterCallback<MouseDownEvent>(MouseDown);
        graphView.StretchToParentSize();
        rootVisualElement.Add(graphView);
        if (_nodeCount == 0)
        {
            //graphView.GenerateNode(Vector2.zero, 1, true, false);
            return;
        }

        save.LoadGraph(storyObj, graphView);
    }
}
#endif