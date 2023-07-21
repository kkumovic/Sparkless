using Sparkless.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    [SerializeField] private GameObject _point;
    [SerializeField] private GameObject _topEdge;
    [SerializeField] private GameObject _bottomEdge;
    [SerializeField] private GameObject _leftEdge;
    [SerializeField] private GameObject _rightEdge;
    [SerializeField] private GameObject _highLight;

    private Dictionary<Node, GameObject> ConnectedEdges;

    [HideInInspector] public int colorId;
    public Vector2Int Pos2D { get; set; }
    public void Init()
    {
        _point.SetActive(false);
        _topEdge.SetActive(false);
        _bottomEdge.SetActive(false);
        _leftEdge.SetActive(false);
        _rightEdge.SetActive(false);
        ConnectedEdges = new Dictionary<Node, GameObject>();
        ConnectedNodes = new List<Node>();

    }
    public void SetColorForPoint(int colorIdForSpawnedNode)
    {
        this.colorId = colorIdForSpawnedNode;
        _point.SetActive(true);
        _point.GetComponent<SpriteRenderer>().color = GameplayManager.instance.NodeColors[colorId];
    }
    public void SetEdge(Vector2Int offset, Node node)
    {
        if(offset == Vector2Int.up)
        {
            ConnectedEdges[node] = _topEdge;
            return;
        }
        if (offset == Vector2Int.down)
        {
            ConnectedEdges[node] = _bottomEdge;
            return;
        }
        if (offset == Vector2Int.left)
        {
            ConnectedEdges[node] = _leftEdge;
            return;
        }
        if (offset == Vector2Int.right)
        {
            ConnectedEdges[node] = _rightEdge;
            return;
        }
    }
    public List<Node> ConnectedNodes;

}
