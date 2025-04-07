using System.Collections;
using UnityEngine;

public class PlayerPiece : MonoBehaviour
{
    
    public enum unitType { pawn, king }; //peças do jogo 
    public enum player { p1, p2 }; //jogadores

    public GameObject unitObj;
    public int unitId;
    public BoardTile node;
    public unitType usType;
    public player ptype;

    public void MoveToPosition(Vector3 pos)
    {
        StartCoroutine(MoveAnimation(pos));
    }

    //movimentação peça
    private IEnumerator MoveAnimation(Vector3 pos)
    {
        //    AudioManager.PlayClip("startMove"); //Audio para movimentação inicial, verificar se colocar
        yield return StartCoroutine(MoveUtils.SmoothLerp(0.2f, unitObj.transform.position, pos + new Vector3(0, 0.5f, 0), unitObj));
        yield return StartCoroutine(MoveUtils.SmoothLerp(0.2f, unitObj.transform.position, pos, unitObj));
        //    AudioManager.PlayClip("endMove"); //Audio para movimentação final, verificar se colocar
    }

    //mover a peça para um quadradinho do tabuleiro, nó especifico
    public void MoveToGridNode(BoardTile targetNode)
    {
        if (targetNode.occupyingUnit != null) return;

        targetNode.occupyingUnit = this;
        node.occupyingUnit = null;
        node = targetNode;
        MoveToPosition(targetNode.position);
    }

    public void Destroy()
    {
        Destroy(unitObj);
        Destroy(this);
    }
}
