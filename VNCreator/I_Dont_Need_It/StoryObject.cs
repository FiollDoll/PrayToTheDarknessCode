using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Story", menuName = "New Story")]
public class StoryObject : ScriptableObject
{
    [HideInInspector] public List<Link> links;
    [HideInInspector] public List<NodeData> nodes;

    public void SetLists(List<NodeData> _nodes, List<Link> _links)
    {
        links = new List<Link>();
        for (int i = 0; i < _links.Count; i++)
        {
            links.Add(_links[i]);
        }

        nodes = new List<NodeData>();
        for (int i = 0; i < _nodes.Count; i++)
        {
            nodes.Add(_nodes[i]);
        }
    }

    public NodeData GetFirstNode()
    {
        for (int i = 0; i < nodes.Count; i++)
        {
            if (nodes[i].startNode)
            {
                return nodes[i];
            }
        }

        Debug.LogError("You need a start node");
        return null;
    }

    public NodeData GetCurrentNode(string _currentGuid)
    {
        for (int i = 0; i < nodes.Count; i++)
        {
            if (nodes[i].guid == _currentGuid)
                return nodes[i];
        }

        return null;
    }

    List<Link> _tempLinks = new List<Link>();

    public NodeData GetNextNode(string _currentGuid, int _choiceId)
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