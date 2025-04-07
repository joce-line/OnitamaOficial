using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    private BoardTile lastSelectedNode = null;
    private OnitamaCard lastSelectedCard = null;
    private bool inSelectionMode = false;

    private int layerMask = 1 << 9;

    private List<BoardTile> moveList = new();

    private int activePlayer;

    void Start()
    {
        layerMask = ~layerMask;
        EventManager.StartListening("ActivePlayer", OnActivePlayerChange);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero, Mathf.Infinity, layerMask);

            if (hit.collider != null)
            {
                GameObject tempHit = hit.collider.gameObject;

                if (tempHit.CompareTag("GridNode"))
                {
                    BoardTile tempNode = BoardManager.GetNodeS(tempHit);

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
                        if (tempNode.occupyingUnit != null && PlayerUnitCheck(tempNode.occupyingUnit.ptype))
                        {
                            inSelectionMode = true;
                            lastSelectedNode = tempNode;
                            lastSelectedNode.TurnOnHighlight();
                            SetupMoveList(tempNode);
                        }
                    }

                    // Debug.Log(tempNode.GetXZ()[0] + "," + tempNode.GetXZ()[1]);
                }

                if (tempHit.CompareTag("OnitamaCard"))
                {
                    OnitamaCard selectedCard = tempHit.GetComponentInChildren<OnitamaCard>();
                    if (PlayerCardCheck(selectedCard))
                    {
                        if (inSelectionMode)
                        {
                            lastSelectedCard.TurnOffHighlight();
                            lastSelectedCard = selectedCard;
                            lastSelectedCard.TurnOnHighlight();
                            ResetMoveList();
                            SetupMoveList(lastSelectedNode);
                        }
                        else
                        {
                            if (lastSelectedCard != null)
                                lastSelectedCard.TurnOffHighlight();

                            lastSelectedCard = selectedCard;
                            lastSelectedCard.TurnOnHighlight();
                        }
                    }
                }
            }
        }
    }

    private bool CheckIfInMoveList(BoardTile nodeS)
    {
        return moveList.Contains(nodeS);
    }

    private void SetupMoveList(BoardTile node)
    {
        ResetMoveList();
        moveList = GameManager.BuildUnitMoveList(lastSelectedCard, node);
        foreach (BoardTile item in moveList)
        {
            item.TurnOnMoveHighlight();
        }
    }

    private void ResetMoveList()
    {
        foreach (BoardTile item in moveList)
        {
            item.TurnOffHighlight();
        }
    }

    public void OnActivePlayerChange(string eventName, ActionParams data)
    {
        activePlayer = data.Get<int>("activePlayer");
        lastSelectedCard = CardManager.SelectRandomPlayerCard(activePlayer);
        lastSelectedCard.TurnOnHighlight();
    }

    private bool PlayerUnitCheck(PlayerPiece.Player id)
    {
        return (id == PlayerPiece.Player.p1 && activePlayer == 1) || (id == PlayerPiece.Player.p2 && activePlayer == 2);
    }

    private bool PlayerCardCheck(OnitamaCard card)
    {
        return (card.playerId == activePlayer);
    }

    public void EndPlayerMovement()
    {
        lastSelectedCard.TurnOffHighlight();
        ActionParams data = new();
        data.Put("activePlayer", activePlayer);
        data.Put("lastSelectedCard", lastSelectedCard);
        data.Put("lastSelectedNode", lastSelectedNode);
        EventManager.TriggerEvent("EndPlayerMovement", data);
    }

}
