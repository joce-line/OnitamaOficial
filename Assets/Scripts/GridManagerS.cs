using System.Collections.Generic;
using UnityEngine;

public class GridManagerS : MonoBehaviour
{
    public static GridManagerS instance;

    public static GridManagerS GetInstance()
    {
        if (instance == null)
            instance = (GridManagerS)FindAnyObjectByType(typeof(GridManagerS));
        return instance;
    }

    // Dimensões do tabuleiro
    public int dimensionX = 5;
    public int dimensionY = 5;
    public float nodeSize = 1f;

    public Transform nodePrefab;
    public Transform gridParent;

    //lista de tiles
    public List<List<GridNodeS>> grid = new List<List<GridNodeS>>();
    //lista com os nós
    private List<GridNodeS> allNodes = new List<GridNodeS>();

    void Awake()
    {
        if (instance == null)
            instance = this;
        GenerateGridList();
    }

    void Start()
    {
        //GenerateGridList();
    }

    public void GenerateGridList()
    {
        //calcula o offset para centralizar a grade no centro
        float offsetX = (dimensionX * nodeSize * 0.5f) - (nodeSize * 0.5f);
        float offsetY = (dimensionY * nodeSize * 0.5f) - (nodeSize * 0.5f);

        int count = 0;
        for (int x = 0; x < dimensionX; x++)
        {
            List<GridNodeS> tempList = new List<GridNodeS>();
            for (int y = 0; y < dimensionY; y++)
            {
                Vector3 position = new Vector3(x, y, 0) * nodeSize - new Vector3(offsetX, offsetY, 0);
                GridNodeS node = new GridNodeS(count, x, y, position);
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
                GridNodeS node = grid[x][y];
                node.objHolder = Instantiate(nodePrefab, node.GetPosition(), Quaternion.identity);
                node.objHolder.transform.parent = gridParent;
                node.objHolder.localScale = Vector3.one * nodeSize;
                node.objHolder.gameObject.GetComponent<SpriteRenderer>().enabled = node.walkable;
                node.objHolder.tag = "GridNode";
            }
        }
    }

    //busca um tile com base no objeto visual
    public static GridNodeS GetNodeS(GameObject go)
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

    //busca um tile pelo índice da grade
    public static GridNodeS GetNodeS(int x, int y)
    {
        if (x < 0 || x >= instance.dimensionX || y < 0 || y >= instance.dimensionY)
        {
            Debug.LogError($"Tentativa de acessar nó fora dos limites: x={x}, y={y}, gridSizeX={instance.dimensionX}, gridSizeY={instance.dimensionY}");
            return null;
        }

        return instance.grid[x][y];
    }
}
