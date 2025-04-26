using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
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
                            inSelectionMode = true;
                            lastSelectedNode = tempNode;
                            lastSelectedNode.TurnOnHighlight();
                            SetupMoveList(tempNode);
                        }
                    }
                }
                if (tempHit.CompareTag("OnitamaCard"))
                {
                    OnitamaCard selectedCard = tempHit.GetComponentInChildren<OnitamaCard>();
                    if (playerCardCheck(selectedCard))
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

    private bool CheckIfInMoveList(GridNodeS nodeS)
    {
        return moveList.Contains(nodeS);
    }

    private void SetupMoveList(GridNodeS node)
    {
        ResetMoveList();
        moveList = GameManager.BuildUnitMoveList(lastSelectedCard, node);
        foreach (GridNodeS item in moveList)
        {
            item.TurnOnMoveHighlight();
        }
    }

    private void ResetMoveList()
    {
        foreach (GridNodeS item in moveList)
        {
            item.TurnOffHighlight();
        }
    }

    public void OnActivePlayerChange(string eventName, ActionParams data)
    {
        if (lastSelectedCard != null)
        {
            lastSelectedCard.TurnOffHighlight();
            lastSelectedCard = null;
        }
        activePlayer = data.Get<int>("activePlayer");
        lastSelectedCard = CardManager.SelectRandomPlayerCard(activePlayer);
        lastSelectedCard.TurnOnHighlight();
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
        lastSelectedCard.TurnOffHighlight();
        ActionParams data = new ActionParams();
        data.Put("activePlayer", activePlayer);
        data.Put("lastSelectedCard", lastSelectedCard);
        data.Put("lastSelectedNode", lastSelectedNode);
        EventManager.TriggerEvent("EndPlayerMovement", data);
    }
}
