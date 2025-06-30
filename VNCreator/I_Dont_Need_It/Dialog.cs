using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Dialog")]
public class Dialog : ScriptableObject
{
    [HideInInspector] public List<Link> links;
    [HideInInspector] public List<DialogStepNode> nodes;

    public void SetLists(List<DialogStepNode> _nodes, List<Link> _links)
    {
        links = new List<Link>();
        foreach (var t in _links)
            links.Add(t);

        nodes = new List<DialogStepNode>();
        foreach (var t in _nodes)
            nodes.Add(t);
    }

    public DialogStepNode GetFirstNode()
    {
        for (int i = 0; i < nodes.Count; i++)
        {
            if (nodes[i].startNode)
                return nodes[i];
        }

        return null;
    }

    public DialogStepNode GetCurrentNode(string _currentGuid)
    {
        for (int i = 0; i < nodes.Count; i++)
        {
            if (nodes[i].guid == _currentGuid)
                return nodes[i];
        }

        return null;
    }

    List<Link> _tempLinks = new List<Link>();

    public DialogStepNode GetNextNode(string _currentGuid, int _choiceId)
    {
        _tempLinks = new List<Link>();

        for (int i = 0; i < links.Count; i++)
        {
            if (links[i].guid == _currentGuid)
                _tempLinks.Add(links[i]);
        }

        if (_choiceId < _tempLinks.Count)
            return GetCurrentNode(_tempLinks[_choiceId].targetGuid);
        else
            return null;
    }
}