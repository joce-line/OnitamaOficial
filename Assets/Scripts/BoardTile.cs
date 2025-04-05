using System.Collections.Generic;
using UnityEngine;

public class BoardTile
{
    public int x, y;
    public int index;
    public Vector3 position;

    public Transform objHolder;
    public bool walkable = true;

    //public UnitS occupyingUnit = null;
    public GameObject tempUnit;

    public BoardTile(int indexv, int xv, int yv, Vector3 posv)
    {
        index = indexv; x = xv; y = yv; position = posv;
    }

    public Vector3 GetPosition()
    {
        return position;
    }

    public List<int> GetXY()
    {
        return new List<int> { x, y };
    }

    public Transform GetNodeObject()
    {
        return objHolder;
    }

    #region Interações do tabuleiro
    public void TurnOnHighlight()
    {
        objHolder.GetComponent<SpriteRenderer>().color = Color.yellow;
    }

    public void TurnOffHighlight()
    {
        objHolder.GetComponent<SpriteRenderer>().color = Color.clear;
    }

    public void TurnOnMoveHighlight()
    {
        objHolder.GetComponent<SpriteRenderer>().color = Color.green;
    }
    #endregion
}