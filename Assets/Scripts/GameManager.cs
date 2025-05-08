using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Assets.scripts.InfoPlayer;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public List<UnitS> player1Units = new List<UnitS>();
    public List<UnitS> player2Units = new List<UnitS>();
    public GameObject playerPawnPrefab;
    public GameObject playerKingPrefab;

    public bool pturn = true;
    public int turnNum = -1;
    private bool p1KingCaptured = false;
    private bool p2KingCaptured = false;
    private GridNodeS p1GoalNode;
    private GridNodeS p2GoalNode;
    private GameObject goal1Tmp;

    public GameObject GoalText;

    public int activePlayer = 0;

    public static GameManager instance;
    public static GameManager GetInstance()
    {
        if (instance == null)
        {
            instance = (GameManager)FindFirstObjectByType(typeof(GameManager));
        }
        return instance;
    }

    public void Awake()
    {
        if (instance == null)
        {
            instance = (GameManager)FindFirstObjectByType(typeof(GameManager));
            if (instance == null)
            {
                instance = this;
            }
        }
    }

    public void Start()
    {
        InitPlayers();
        InitGoalNodes();
        EventManager.StartListening("EndPlayerMovement", OnEndPlayerMovement);
        RunGame();
    }

    public void InitPlayers()
    {
        for (int i = 0; i < 5; i++)
        {
            if (i == 2)
            {
                CreateUnit(1, i, 0, UnitS.unitType.king);
            }
            else
            {
                CreateUnit(1, i, 0, UnitS.unitType.pawn);
            }
        }
        for (int i = 0; i < 5; i++)
        {
            if (i == 2)
            {
                CreateUnit(2, i, 4, UnitS.unitType.king);
            }
            else
            {
                CreateUnit(2, i, 4, UnitS.unitType.pawn);
            }
        }
    }

    //para pegar os nós onde ficam os mestres
    public void InitGoalNodes()
    {
        p1GoalNode = GridManagerS.GetNodeS(2, 4);
        p2GoalNode = GridManagerS.GetNodeS(2, 0);
    }

    public void RunGame()
    {
        activePlayer = 1;
        StartCoroutine(setStartingActivePlayer());
    }

    IEnumerator setStartingActivePlayer()
    {
        yield return new WaitForSeconds(.5f);
        ActionParams data = new ActionParams();
        data.Put("activePlayer", activePlayer);
        EventManager.TriggerEvent("ActivePlayer", data);
    }

    public void CreateUnit(int playerId, int x, int y, UnitS.unitType utype)
    {
        UnitS temp;
        GameObject newGo;
        GridNodeS node;

        if (utype == UnitS.unitType.pawn)
        {
            newGo = Instantiate(playerPawnPrefab, Vector3.zero, Quaternion.identity);
        }
        else
        {
            newGo = Instantiate(playerKingPrefab, Vector3.zero, Quaternion.identity);
        }

        temp = newGo.transform.GetComponent<UnitS>();
        node = GridManagerS.GetNodeS(x, y);
        node.occupyingUnit = temp;
        temp.node = node;

        temp.unitObj.transform.position = node.GetPosition() + new Vector3(0, 0.1f, -1);

        //TODO: renderizando as sprites padrao, modificar ao puxar do BD
        SpriteRenderer spriteRenderer = newGo.GetComponent<SpriteRenderer>();
        spriteRenderer.transform.localScale = new Vector3(0.6f, 0.6f, 1);

        if (playerId == 1)
        {
            spriteRenderer.sprite = Resources.Load<Sprite>("Skins/PretoPeao");
            if (utype == UnitS.unitType.king)
            {
                spriteRenderer.sprite = Resources.Load<Sprite>("Skins/PretoRei");
            }
            temp.ptype = UnitS.player.p1;
            player1Units.Add(temp);
        }
        else
        {
            spriteRenderer.sprite = Resources.Load<Sprite>("Skins/BrancoPeao");
            if (utype == UnitS.unitType.king)
            {
                spriteRenderer.sprite = Resources.Load<Sprite>("Skins/BrancoRei");
            }
            temp.ptype = UnitS.player.p2;
            player2Units.Add(temp);
        }
    }

    public void OnEndPlayerMovement(string eventName, ActionParams data)
    {
        int receivedPlayer = data.Get<int>("activePlayer");
        GridNodeS receivedNode = data.Get<GridNodeS>("lastSelectedNode");

        if (CheckForGameEnd(receivedNode))
        {
            DadosJogo.vencedor = receivedPlayer;  
            DadosJogo.perdedor = (receivedPlayer == 1) ? 2 : 1;


            ActionParams temp = new ActionParams();
            temp.Put("activePlayer", activePlayer);
            EventManager.TriggerEvent("GameOver", temp);

            SceneManager.LoadScene("FimDoJogo");
        }
        else
        {
            activePlayer = (receivedPlayer == 1) ? 2 : 1;


            ActionParams temp = new ActionParams();
            temp.Put("activePlayer", activePlayer);
            EventManager.TriggerEvent("ActivePlayer", temp);

        }
    }

    public bool CheckForGameEnd(GridNodeS node)
    {
        //verifica se os reis foram capturados
        if (p1KingCaptured || p2KingCaptured)
        {
            return true;
        }

        //Verifica se o rei adversario chegou no campo do outro
        if (node == p1GoalNode)
        {
            if (p1GoalNode.occupyingUnit.usType == UnitS.unitType.king &&
                p1GoalNode.occupyingUnit.ptype == UnitS.player.p1)
            {
                return true;
            }
        }
        if (node == p2GoalNode)
        {
            if (p2GoalNode.occupyingUnit.usType == UnitS.unitType.king &&
                p2GoalNode.occupyingUnit.ptype == UnitS.player.p2)
            {
                return true;
            }
        }

        return false;
    }

    public static void RemoveUnitAtNode(GridNodeS node)
    {
        if (node.occupyingUnit != null)
        {
            UnitS temp = node.occupyingUnit;
            if (temp.usType == UnitS.unitType.king)
            {
                if (temp.ptype == UnitS.player.p1)
                {
                    instance.p1KingCaptured = true;
                }
                else if (temp.ptype == UnitS.player.p2)
                {
                    instance.p2KingCaptured = true;
                }
            }

            RemoveUnitFromPlayerList(temp);
            node.occupyingUnit = null;
            temp.Destroy();
        }
    }

    public static void RemoveUnitFromPlayerList(UnitS unit)
    {
        if (unit.ptype == UnitS.player.p1)
        {
            instance.player1Units.Remove(unit);
        }
        else
        {
            instance.player2Units.Remove(unit);
        }
    }

    public static List<GridNodeS> BuildUnitMoveList(OnitamaCard card, GridNodeS node)
    {
        List<CardMove> unitmoves = card.cardinfo.moveset;
        int x = node.x;
        int y = node.y;

        if (card.playerId == 1)
        {
            return instance.LegalUnitMoves(unitmoves, x, y, 1);
        }
        else
        {
            return instance.LegalUnitMoves(unitmoves, x, y, -1);
        }
    }

    private List<GridNodeS> LegalUnitMoves(List<CardMove> moves, int x, int y, int mult)
    {
        List<GridNodeS> rList = new List<GridNodeS>();

        foreach (CardMove move in moves)
        {
            /*GridNodeS temp = GridManagerS.GetNodeS(move.x + x, move.y * mult + y);
            if (temp != null)
            {
                rList.Add(temp);
            }*/
            int newX = move.x + x;
            int newY = move.y * mult + y;

            // Verifica se a nova posição está dentro do tabuleiro 5x5
            if (newX >= 0 && newX < GridManagerS.instance.dimensionX &&
                newY >= 0 && newY < GridManagerS.instance.dimensionY)
            {
                GridNodeS temp = GridManagerS.GetNodeS(newX, newY);
                if (temp != null) // Mantém a robustez
                {
                    rList.Add(temp);
                }
            }
        }

        return rList;
    }

    public static List<UnitS> GetPlayerUnitList(int playerId)
    {
        return (playerId == 1) ? instance.player1Units : instance.player2Units;
    }

    public void CleanUpPlayerUnits()
    {
        foreach (UnitS unit in player1Units)
        {
            unit.Destroy();
        }
        foreach (UnitS unit in player2Units)
        {
            unit.Destroy();
        }

        player1Units.Clear();
        player2Units.Clear();
    }

    public static void RestartGame()
    {
        CardManager.RestartGame();
        instance.CleanUpPlayerUnits();
        instance.InitPlayers();

        instance.p1KingCaptured = false;
        instance.p2KingCaptured = false;
        instance.RunGame();
    }
}
