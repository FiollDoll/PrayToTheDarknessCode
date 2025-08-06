using System.Collections.Generic;
using System.Linq;
using UnityEditor;
#if UNITY_EDITOR
using UnityEditor.Experimental.GraphView;
using UnityEngine;
#endif
using UnityEngine.UIElements;

public class SaveUtility
{
#if UNITY_EDITOR
    public void SaveGraph(Dialog _story, ExtendedGraphView _graph)
    {
        EditorUtility.SetDirty(_story);

        List<DialogStepNode> nodes = new List<DialogStepNode>();
        List<Link> links = new List<Link>();

        foreach (BaseNode _node in _graph.nodes.ToList().Cast<BaseNode>().ToList())
        {
            nodes.Add(
                new DialogStepNode
                {
                    guid = _node.DialogStepNode.guid,
                    character = _node.DialogStepNode.character,
                    dialogTextRu = _node.DialogStepNode.dialogTextRu,
                    dialogTextEn = _node.DialogStepNode.dialogTextEn,
                    thisDialogLock = _node.DialogStepNode.thisDialogLock,
                    startNode = _node.DialogStepNode.startNode,
                    endNode = _node.DialogStepNode.endNode,
                    choices = _node.DialogStepNode.choices,
                    choiceName = _node.DialogStepNode.choiceName,
                    choiceOptionsRu = _node.DialogStepNode.choiceOptionsRu,
                    choiceOptionsEn = _node.DialogStepNode.choiceOptionsEn,
                    choiceLock = _node.DialogStepNode.choiceLock,
                    dialogLock = _node.DialogStepNode.dialogLock,
                    dialogLockState = _node.DialogStepNode.dialogLockState,
                    dialogChoiceLock = _node.DialogStepNode.dialogChoiceLock,
                    choiceNameToChange = _node.DialogStepNode.choiceNameToChange,
                    choiceNewState = _node.DialogStepNode.choiceNewState,
                    moreRead = _node.DialogStepNode.moreRead,
                    canMove = _node.DialogStepNode.canMove,
                    canInter = _node.DialogStepNode.canInter,
                    mood = _node.DialogStepNode.mood,
                    styleOfDialog = _node.DialogStepNode.styleOfDialog,
                    stepSpeech = _node.DialogStepNode.stepSpeech,
                    bigPicture = _node.DialogStepNode.bigPicture,
                    mainPanelStartDelay = _node.DialogStepNode.mainPanelStartDelay,
                    delayAfterNext = _node.DialogStepNode.delayAfterNext,
                    darkAfterEnd = _node.DialogStepNode.darkAfterEnd,
                    fastChanges = _node.DialogStepNode.fastChanges,
                    npcChangeRelation = _node.DialogStepNode.npcChangeRelation,
                    npcChangeMoveToPlayer = _node.DialogStepNode.npcChangeMoveToPlayer,
                    changeRelation = _node.DialogStepNode.changeRelation,
                    changeKarma = _node.DialogStepNode.changeKarma,
                    changeSanity = _node.DialogStepNode.changeSanity,
                    newStateMoveToPlayer = _node.DialogStepNode.newStateMoveToPlayer,
                    nodePosition = _node.GetPosition(),
                });
        }

        List<Edge> _edges = _graph.edges.ToList();
        for (int i = 0; i < _edges.Count; i++)
        {
            BaseNode _output = (BaseNode)_edges[i].output.node;
            BaseNode _input = (BaseNode)_edges[i].input.node;

            links.Add(new Link
            {
                guid = _output.DialogStepNode.guid,
                targetGuid = _input.DialogStepNode.guid,
                portId = i
            });
        }

        _story.SetLists(nodes, links);

        //_story.nodes = nodes;
        //_story.links = links;
    }

    public void LoadGraph(Dialog _story, ExtendedGraphView _graph)
    {
        foreach (DialogStepNode _data in _story.nodes)
        {
            BaseNode _tempNode = _graph.CreateNode("", _data.nodePosition.position, _data.choices, _data.choiceName,
                _data.choiceOptionsRu, _data.choiceOptionsEn, _data.choiceLock, _data.startNode, _data.endNode, _data);
            _graph.AddElement(_tempNode);
        }

        GenerateLinks(_story, _graph);
    }

    void GenerateLinks(Dialog _story, ExtendedGraphView _graph)
    {
        List<BaseNode> _nodes = _graph.nodes.ToList().Cast<BaseNode>().ToList();

        for (int i = 0; i < _nodes.Count; i++)
        {
            int _outputIdx = 4;
            List<Link> _links = _story.links.Where(x => x.guid == _nodes[i].DialogStepNode.guid).ToList();
            for (int j = 0; j < _links.Count; j++)
            {
                string targetGuid = _links[j].targetGuid;
                BaseNode _target = _nodes.First(x => x.DialogStepNode.guid == targetGuid);
                LinkNodes(_nodes[i].outputContainer[_links.Count > 1 ? _outputIdx : 0].Q<Port>(),
                    (Port)_target.inputContainer[0], _graph);
                _outputIdx += 5; // Эта хуйня учитывает текстовые филды
            }
        }
    }

    private void LinkNodes(Port _output, Port _input, ExtendedGraphView _graph)
    {
        Edge _temp = new Edge
        {
            output = _output,
            input = _input
        };

        _temp.input.Connect(_temp);
        _temp.output.Connect(_temp);
        _graph.Add(_temp);
    }
#endif
}