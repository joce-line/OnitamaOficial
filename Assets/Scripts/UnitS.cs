using System.Collections;
using UnityEngine;

public class UnitS : MonoBehaviour
{
    public enum unitType { pawn, king };
    public enum player { p1, p2 };

    public GameObject unitObj;
    public int unitId;
    public GridNodeS node;
    public unitType usType;
    public player ptype;

    public void MoveToPosition(Vector3 pos)
    {
        StartCoroutine(MoveAnimation(pos));
    }

    //movimenta��o pe�a
    private IEnumerator MoveAnimation(Vector3 pos)
    {
        //    AudioManager.PlayClip("startMove"); // TODO: #Audio para movimenta��o inicial, verificar se colocar
        yield return StartCoroutine(MoveUtils.SmoothLerp(0.2f, unitObj.transform.position, pos + new Vector3(0, 0.5f, -1), unitObj));
        yield return StartCoroutine(MoveUtils.SmoothLerp(0.2f, unitObj.transform.position, pos, unitObj));
        //    AudioManager.PlayClip("endMove"); // TODO: #Audio para movimenta��o final, verificar se colocar
    }

    //mover a pe�a para um quadradinho do tabuleiro, n� especifico
    public void MoveToGridNode(GridNodeS targetNode)
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
