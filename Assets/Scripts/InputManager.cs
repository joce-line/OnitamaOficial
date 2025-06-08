using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
public class InputManager : MonoBehaviourPun
{
    private GridNodeS lastSelectedNode = null;
    private OnitamaCard lastSelectedCard = null;
    private bool inSelectionMode = false;

    private int layerMask = 1 << 9;

    private List<GridNodeS> moveList = new List<GridNodeS>();

    private int activePlayer;

    void Start()
    {
        layerMask = ~layerMask; // Inverte para ignorar apenas a layer 9
        EventManager.StartListening("ActivePlayer", OnActivePlayerChange);
    }

    void Update()
    {
        if (!PhotonNetwork.IsConnected || !PhotonNetwork.InRoom)
            return;

        if ((PhotonNetwork.LocalPlayer.ActorNumber == 1 && activePlayer != 1) ||
            (PhotonNetwork.LocalPlayer.ActorNumber == 2 && activePlayer != 2))
            return;

        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero, Mathf.Infinity, layerMask);

            if (hit.collider != null)
            {
                GameObject tempHit = hit.collider.gameObject;

                if (tempHit.CompareTag("GridNode"))
                {
                    GridNodeS tempNode = GridManagerS.GetNodeS(tempHit);

                    if (tempNode.occupyingUnit != null && lastSelectedNode != null)
                    {
                        if (lastSelectedNode.occupyingUnit != null &&
                            tempNode.occupyingUnit.ptype == lastSelectedNode.occupyingUnit.ptype)
                        {

                            inSelectionMode = false;
                            lastSelectedNode.TurnOffHighlight();
                        }
                    }

                    if (inSelectionMode)
                    {
                        if (tempNode.occupyingUnit != null)
                        {
                            if (tempNode.occupyingUnit.ptype != lastSelectedNode.occupyingUnit.ptype && CheckIfInMoveList(tempNode))
                            {
                                GameManager.RemoveUnitAtNode(tempNode);
                                lastSelectedNode.occupyingUnit.MoveToGridNode(tempNode);
                                lastSelectedNode.TurnOffHighlight();
                                lastSelectedNode = tempNode;
                                ResetMoveList();
                                EndPlayerMovement();
                            }
                        }
                        else
                        {
                            if (tempNode == lastSelectedNode)
                            {
                                lastSelectedNode.TurnOffHighlight();
                                lastSelectedNode = null;
                            }
                            else if (CheckIfInMoveList(tempNode))
                            {
                                lastSelectedNode.occupyingUnit.MoveToGridNode(tempNode);
                                lastSelectedNode.TurnOffHighlight();
                                lastSelectedNode = tempNode;
                                ResetMoveList();
                                EndPlayerMovement();
                            }
                        }

                        inSelectionMode = false;
                    }
                    else
                    {
                        if (tempNode.occupyingUnit != null && playerUnitCheck(tempNode.occupyingUnit.ptype))
                        {
                            if (!tempNode.occupyingUnit.isMine)
                                return;

                            inSelectionMode = true;
                            lastSelectedNode = tempNode;
                            lastSelectedNode.TurnOnHighlight();
                            SetupMoveList(tempNode);
                        }
                    }
                }
                else if (tempHit.CompareTag("OnitamaCard"))
                {
                    OnitamaCard selectedCard = tempHit.GetComponentInChildren<OnitamaCard>();
                    if (playerCardCheck(selectedCard))
                    {
                        photonView.RPC("SelectCard", RpcTarget.AllBuffered, selectedCard.photonView.ViewID);
                    }
                }
            }
        }
    }

    [PunRPC]
    public void SelectCard(int cardViewID)
    {
        OnitamaCard selectedCard = PhotonView.Find(cardViewID)?.GetComponent<OnitamaCard>();
        if (selectedCard == null)
        {
            Debug.LogError("Carta não encontrada para o ViewID: " + cardViewID);
            return;
        }

        if (inSelectionMode)
        {
            if (lastSelectedCard != null)
                lastSelectedCard.photonView.RPC("TurnOffHighlight", RpcTarget.All);
            lastSelectedCard = selectedCard;
            lastSelectedCard.photonView.RPC("TurnOnHighlight", RpcTarget.All);
            ResetMoveList();
            if (lastSelectedNode != null)
                SetupMoveList(lastSelectedNode);
        }
        else
        {
            if (lastSelectedCard != null)
                lastSelectedCard.photonView.RPC("TurnOffHighlight", RpcTarget.All);
            lastSelectedCard = selectedCard;
            lastSelectedCard.photonView.RPC("TurnOnHighlight", RpcTarget.All);
        }
    }

    private bool CheckIfInMoveList(GridNodeS nodeS)
    {
        foreach (var node in moveList)
        {
            if (node.x == nodeS.x && node.y == nodeS.y)
                return true;
        }
        return false;
    }

    private void SetupMoveList(GridNodeS node)
    {
        ResetMoveList();
        if (lastSelectedCard == null)
        {
            Debug.LogWarning("Nenhuma carta selecionada para construir a lista de movimentos");
            return;
        }
        moveList = GameManager.BuildUnitMoveList(lastSelectedCard, node);
        foreach (GridNodeS item in moveList)
        {
            if (item.occupyingUnit != null && item.occupyingUnit.ptype == lastSelectedNode.occupyingUnit.ptype)
            {
                item.TurnOffHighlight();
            }
            else
            {
                item.TurnOnMoveHighlight();

            }
        }
    }

    private void ResetMoveList()
    {
        foreach (GridNodeS item in moveList)
        {
            item.TurnOffHighlight();
        }
        moveList.Clear();
    }

    public void OnActivePlayerChange(string eventName, ActionParams data)
    {
        if (lastSelectedCard != null)
        {
            lastSelectedCard.photonView.RPC("TurnOffHighlight", RpcTarget.All);
            lastSelectedCard = null;
        }

        activePlayer = data.Get<int>("activePlayer");
        if (activePlayer == PhotonNetwork.LocalPlayer.ActorNumber)
        {
            OnitamaCard randomCard = CardManager.SelectRandomPlayerCard(activePlayer);
            if (randomCard != null)
            {
                photonView.RPC("SyncSelectedCard", RpcTarget.AllBuffered, randomCard.photonView.ViewID);
            }
            else
            {
                Debug.LogWarning($"Não foi possível selecionar uma carta para o jogador {activePlayer}");
            }
        }
    }

    [PunRPC]
    public void SyncSelectedCard(int cardViewID)
    {
        OnitamaCard selectedCard = PhotonView.Find(cardViewID)?.GetComponent<OnitamaCard>();

        if (selectedCard == null)
        {
            Debug.LogError("Carta não encontrada para o ViewID: " + cardViewID);
            return;
        }

        if (lastSelectedCard != null)
        {
            lastSelectedCard.photonView.RPC("TurnOffHighlight", RpcTarget.All);
        }
        lastSelectedCard = selectedCard;
        lastSelectedCard.photonView.RPC("TurnOnHighlight", RpcTarget.All);
    }

    private bool playerUnitCheck(UnitS.player id)
    {
        return (id == UnitS.player.p1 && activePlayer == 1) || (id == UnitS.player.p2 && activePlayer == 2);
    }

    private bool playerCardCheck(OnitamaCard card)
    {
        return (card.playerId == activePlayer);
    }

    public void EndPlayerMovement()
    {
        //verificar talvez mudar depois.
        if ((PhotonNetwork.LocalPlayer.ActorNumber == 1 && activePlayer != 1) ||
        (PhotonNetwork.LocalPlayer.ActorNumber == 2 && activePlayer != 2))
        {
            Debug.Log($"[InputManager] EndPlayerMovement bloqueado para {PhotonNetwork.LocalPlayer.NickName} (ActorNumber: {PhotonNetwork.LocalPlayer.ActorNumber}), activePlayer={activePlayer}");
            return;
        }

        if (lastSelectedCard != null)
        {
            lastSelectedCard.photonView.RPC("TurnOffHighlight", RpcTarget.All);
        }

        lastSelectedCard.photonView.RPC("TurnOffHighlight", RpcTarget.All);

        photonView.RPC("RPC_EndPlayerMovement", RpcTarget.MasterClient, activePlayer, lastSelectedCard.photonView.ViewID, lastSelectedNode != null ? lastSelectedNode.GetHashCode() : 0);
    }

    [PunRPC]
    private void RPC_EndPlayerMovement(int activePlayer, int cardViewID, int nodeHash)
    {
        if (!PhotonNetwork.IsMasterClient)
            return;

        PhotonView cardView = PhotonView.Find(cardViewID);
        OnitamaCard selectedCard = cardView != null ? cardView.GetComponent<OnitamaCard>() : null;
        GridNodeS selectedNode = null;

        ActionParams data = new ActionParams();
        data.Put("activePlayer", activePlayer);
        data.Put("lastSelectedCard", selectedCard);
        data.Put("lastSelectedNode", selectedNode);
        EventManager.TriggerEvent("EndPlayerMovement", data);
    }
}
