using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CardData", menuName = "Scriptable Objects/CardData")]
public class CardData : ScriptableObject
{
    public new string name;
    public Sprite gridSprite;
    public List<CardMove> moveset = new List<CardMove>();

    public CardData GetInstance()
    {
        return Instantiate(this);
    }
}
