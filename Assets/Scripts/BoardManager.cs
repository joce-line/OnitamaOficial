using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public static BoardManager instance;
    public static BoardManager GetInstance()
    {
        if (instance == null)
            instance = (BoardManager)FindAnyObjectByType(typeof(BoardManager));
        return instance;
    }

    public int dimensionX = 5;
    public int dimensionY = 5;
    public float nodeSize = 1f;

    public Transform nodePrefab;
    public Transform gridParent;

    public List<List<BoardTile>> grid = new List<List<BoardTile>>();
    private List<BoardTile> allNodes = new List<BoardTile>();

    void Awake()
    {
        if (instance == null)
            instance = this;
    }

    void Start()
    {
        GenerateGridList();
    }

    public void GenerateGridList()
    {
        float offsetX = (dimensionX * nodeSize * 0.5f) - (nodeSize * 0.5f);
        float offsetY = (dimensionY * nodeSize * 0.5f) - (nodeSize * 0.5f);

        int count = 0;
        for (int x = 0; x < dimensionX; x++)
        {
            List<BoardTile> tempList = new List<BoardTile>();
            for (int y = 0; y < dimensionY; y++)
            {
                Vector3 position = new Vector3(x, y, 0) * nodeSize - new Vector3(offsetX, offsetY, 0);
                BoardTile node = new BoardTile(count, x, y, position);
                tempList.Add(node);
                allNodes.Add(node);
                count++;
            }
            grid.Add(tempList);
        }

        EnableGridObjects();
    }

    public void EnableGridObjects()
    {
        if (gridParent == null)
        {
            gridParent = new GameObject("Grid").transform;
            gridParent.parent = transform;
        }

        for (int x = 0; x < dimensionX; x++)
        {
            for (int y = 0; y < dimensionY; y++)
            {
                BoardTile node = grid[x][y];
                node.objHolder = Instantiate(nodePrefab, node.GetPosition(), Quaternion.identity);
                node.objHolder.transform.parent = gridParent;
                node.objHolder.localScale = Vector3.one * nodeSize;
                node.objHolder.gameObject.GetComponent<SpriteRenderer>().enabled = node.walkable;
                node.objHolder.tag = "GridNode";
            }
        }
    }

    public static BoardTile GetNodeS(GameObject go)
    {
        foreach (var row in instance.grid)
        {
            foreach (var node in row)
            {
                if (node.GetNodeObject().gameObject == go)
                    return node;
            }
        }
        return null;
    }

    public static BoardTile GetNodeS(int x, int y)
    {
        if (x < 0 || x >= instance.dimensionX || y < 0 || y >= instance.dimensionY)
            return null;

        return instance.grid[x][y];
    }

}