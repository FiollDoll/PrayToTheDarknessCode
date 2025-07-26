using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor.Experimental.GraphView;
#endif
using UnityEngine;
using UnityEngine.UIElements;

#if UNITY_EDITOR
public class ExtendedGraphView : GraphView
{
    public SaveUtility save;
    public Dialog storyObj;
    private Vector2 mousePosition = new Vector2();

    public ExtendedGraphView()
    {
        SetupZoom(0.1f, 2);

        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());

        var grid = new GridBackground();
        Insert(0, grid);
        grid.StretchToParentSize();
    }

    public void GenerateNode(string _nodeName, Vector2 _mousePos, int choiceAmount, bool _startNode, bool _endNode)
    {
        AddElement(CreateNode(_nodeName, _mousePos, choiceAmount, _startNode, _endNode));
        save.SaveGraph(storyObj, this);
    }

    public BaseNode CreateNode(string _nodeName, Vector2 _mousePos, int choiceAmount, bool _startNode,
        bool _endNode)
    {
        return CreateNode(_nodeName, _mousePos, choiceAmount, new string[choiceAmount].ToList(),
            new string[choiceAmount].ToList(), _startNode,
            _endNode, new DialogStepNode());
    }

    public BaseNode CreateNode(string _nodeName, Vector2 _mousePos, int choiceAmount, List<string> _choicesRu,
        List<string> _choicesEn, bool _startNode, bool _endNode, DialogStepNode _data)
    {
        BaseNode _node = new BaseNode(_data, _startNode);

        _node.title = _nodeName;
        _node.SetPosition(new Rect(
            (new Vector2(viewTransform.position.x, viewTransform.position.y) * -(1 / scale)) +
            (_mousePos * (1 / scale)), Vector2.one));
        _node.DialogStepNode.startNode = _startNode;
        _node.DialogStepNode.endNode = _endNode;
        _node.DialogStepNode.choices = choiceAmount;
        _node.DialogStepNode.choiceOptionsRu = _choicesRu;
        _node.DialogStepNode.choiceOptionsEn = _choicesEn;

        if (!_startNode)
        {
            Port _inputPort = CreatePort(_node, Direction.Input, Port.Capacity.Multi);
            _inputPort.portName = "Input";
            _node.inputContainer.Add(_inputPort);
        }

        if (!_endNode)
        {
            if (choiceAmount > 1)
            {
                for (int i = 0; i < choiceAmount; i++)
                {
                    Port _outputPort = CreatePort(_node, Direction.Output, Port.Capacity.Single);
                    _outputPort.portName = "Choice " + (i + 1);

                    //_node.nodeData.choiceOptions.Add(_node.nodeData.choiceOptions[i]);
                    int _idx = i;

                    string _value = _data.choiceOptionsRu.Count == 0
                        ? "Выбор " + (i + 1)
                        : _node.DialogStepNode.choiceOptionsRu[i];

                    TextField _field = new TextField { value = _value };
                    _field.RegisterValueChangedCallback(e =>
                        {
                            _node.DialogStepNode.choiceOptionsRu[_idx] = _field.value;
                        }
                    );

                    _value = _data.choiceOptionsEn.Count == 0
                        ? "Choice " + (i + 1)
                        : _node.DialogStepNode.choiceOptionsEn[i];

                    TextField _field1 = new TextField { value = _value };
                    _field1.RegisterValueChangedCallback(e =>
                        {
                            _node.DialogStepNode.choiceOptionsEn[_idx] = _field1.value;
                        }
                    );

                    _node.outputContainer.Add(_field);
                    _node.outputContainer.Add(_field1);
                    _node.outputContainer.Add(_outputPort);
                }
            }
            else
            {
                Port _outputPort = CreatePort(_node, Direction.Output, Port.Capacity.Single);
                _outputPort.portName = "Next";
                _node.outputContainer.Add(_outputPort);
            }
        }

        _node.mainContainer.Add(_node.visuals);

        _node.RefreshExpandedState();
        _node.RefreshPorts();

        return _node;
    }

    Port CreatePort(BaseNode _node, Direction _portDir, Port.Capacity _capacity)
    {
        return _node.InstantiatePort(Orientation.Horizontal, _portDir, _capacity, typeof(float));
    }

    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
    {
        var compatiblePorts = new List<Port>();
        ports.ForEach((port) =>
        {
            if (startPort != port && startPort.node != port.node)
                compatiblePorts.Add(port);
        });

        return compatiblePorts;
    }

    void MousePos(Vector2 _v2)
    {
        mousePosition = _v2;
    }
}
#endif