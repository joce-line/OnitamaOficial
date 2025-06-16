using System.Collections;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Networking;

public class UnitS : MonoBehaviourPun
{
    public enum unitType { pawn, king };
    public enum player { p1, p2 };

    public GameObject unitObj;
    public int unitId;
    public GridNodeS node;
    public unitType usType;
    public player ptype;
    public SpriteRenderer spriteRenderer; // Novo: para carregar sprite

    public bool isMine => GetComponent<PhotonView>().IsMine;

    void Awake()
    {
        unitObj = gameObject;
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer não encontrado em " + gameObject.name);
        }
    }

    public void MoveToPosition(Vector3 pos)
    {

        GetComponent<PhotonView>().RPC("RPC_MoveToPosition", RpcTarget.AllBuffered, pos.x, pos.y, pos.z);

    }


    [PunRPC]
    public void RPC_MoveToPosition(float x, float y, float z)
    {
        Vector3 pos = new Vector3(x, y, z);
        StartCoroutine(MoveAnimation(pos));
    }

    [PunRPC]
    public void RPC_SetData(int unitId, int x, int y, string spriteUrl)
    {
        this.unitId = unitId;

        node = GridManagerS.GetNodeS(x, y);
        node.occupyingUnit = this;
        unitObj = gameObject;
        transform.position = node.GetPosition() + new Vector3(0, 0.1f, -1);

        //SpriteRenderer sr = GetComponent<SpriteRenderer>();
        //sr.sprite = Resources.Load<Sprite>(spritePath);
        //sr.transform.localScale = new Vector3(0.6f, 0.6f, 1);

        //if (photonView.Owner.ActorNumber == 1)
        //    ptype = player.p1;
        //else
        //    ptype = player.p2;

        spriteRenderer.transform.localScale = new Vector3(0.6f, 0.6f, 1);

        if (photonView.Owner.ActorNumber == 1)
            ptype = player.p1;
        else
            ptype = player.p2;

        usType = gameObject.name.Contains("Pawn") ? unitType.pawn : unitType.king;

        // Carrega imagem da URL
        StartCoroutine(LoadImage(spriteUrl));

    }

    private IEnumerator LoadImage(string url)
    {
        if (string.IsNullOrEmpty(url))
        {
            Sprite defaultSprite = Resources.Load<Sprite>("Skins/DefaultPawn");
            if (defaultSprite != null)
            {
                spriteRenderer.sprite = defaultSprite;
            }
            else
            {
                Debug.LogError("Sprite padrão 'Skins/DefaultPawn' não encontrado!");
            }
            yield break;
        }

        UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"[UnitS] Erro ao carregar imagem {url}: {request.error}");
            Sprite defaultSprite = Resources.Load<Sprite>("Skins/DefaultPawn");
            if (defaultSprite != null)
            {
                spriteRenderer.sprite = defaultSprite;
            }
            yield break;
        }

        Texture2D texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        spriteRenderer.sprite = sprite;
    }

    //movimentação peça
    private IEnumerator MoveAnimation(Vector3 pos)
    {
        //    AudioManager.PlayClip("startMove"); // TODO: #Audio para movimentação inicial, verificar se colocar
        yield return StartCoroutine(MoveUtils.SmoothLerp(0.2f, unitObj.transform.position, pos + new Vector3(0, 0.5f, -1), unitObj));
        yield return StartCoroutine(MoveUtils.SmoothLerp(0.2f, unitObj.transform.position, pos, unitObj));
        //    AudioManager.PlayClip("endMove"); // TODO: #Audio para movimentação final, verificar se colocar
    }


    public void MoveToGridNode(GridNodeS targetNode)
    {
        photonView.RPC("RPC_MoveToNode", RpcTarget.AllBuffered, targetNode.x, targetNode.y);
    }

    [PunRPC]
    public void RPC_MoveToNode(int x, int y)
    {
        GridNodeS targetNode = GridManagerS.GetNodeS(x, y);

        if (targetNode.occupyingUnit != null)
        {
            Debug.Log("Comendo peça inimiga");
            GameManager.RemoveUnitAtNode(targetNode);
        }

        targetNode.occupyingUnit = this;

        if (node != null)
            node.occupyingUnit = null;

        node = targetNode;

        MoveToPosition(targetNode.position);
    }

    [PunRPC]
    public void DestroyRPC()
    {
        if (photonView.IsMine)
        {
            PhotonNetwork.Destroy(unitObj);
            PhotonNetwork.Destroy(gameObject);
        }
    }
}
