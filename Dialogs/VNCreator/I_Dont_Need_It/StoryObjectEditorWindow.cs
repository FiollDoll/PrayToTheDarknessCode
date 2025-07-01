using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

#if UNITY_EDITOR
public class StoryObjectEditorWindow : EditorWindow
{
    private Dialog _storyObj;
    private ExtendedGraphView _graphView;
    private readonly SaveUtility _save = new SaveUtility();

    private Vector2 _mousePosition = new Vector2();

    public static void Open(Dialog storyObj)
    {
        StoryObjectEditorWindow window = GetWindow<StoryObjectEditorWindow>("Story");
        window._storyObj = storyObj;
        window.CreateGraphView(storyObj.nodes == null ? 0 : 1);
        window.minSize = new Vector2(200, 100);
    }

    void MouseDown(MouseDownEvent e)
    {
        if (e.button == 1)
        {
            _mousePosition = Event.current.mousePosition;
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("Add Node"), false,
                () => _graphView.GenerateNode("", _mousePosition, 1, false, false));
            menu.AddItem(new GUIContent("Add Node (2 Choices)"), false,
                () => _graphView.GenerateNode("", _mousePosition, 2, false, false));
            menu.AddItem(new GUIContent("Add Node (3 Choices)"), false,
                () => _graphView.GenerateNode("", _mousePosition, 3, false, false));
            menu.AddItem(new GUIContent("Add Node (Start)"), false,
                () => _graphView.GenerateNode("", _mousePosition, 1, true, false));
            menu.AddItem(new GUIContent("Add Node (End)"), false,
                () => _graphView.GenerateNode("", _mousePosition, 1, false, true));
            menu.AddItem(new GUIContent("Save"), false, () => _save.SaveGraph(_storyObj, _graphView));
            menu.ShowAsContext();
        }
    }

    void CreateGraphView(int nodeCount)
    {
        _graphView = new ExtendedGraphView();
        _graphView.RegisterCallback<MouseDownEvent>(MouseDown);
        _graphView.StretchToParentSize();
        _graphView.save = _save;
        _graphView.storyObj = _storyObj;
        rootVisualElement.Add(_graphView);
        if (nodeCount == 0)
        {
            //graphView.GenerateNode(Vector2.zero, 1, true, false);
            return;
        }

        _save.LoadGraph(_storyObj, _graphView);
    }
}
#endif