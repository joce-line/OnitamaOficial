using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public List<PlayerPiece> player1Units = new();
    public List<PlayerPiece> player2Units = new();
    public GameObject playerPawnPrefab;
    public GameObject playerKingPrefab;

    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private GameObject tileHighlightPrefab;

    public bool pturn = true;
    public int turnNum = -1;
    private bool p1KingCaptured = false;
    private bool p2KingCaptured = false;
    private BoardTile p1GoalNode;
    private BoardTile p2GoalNode;
    private GameObject goal1Tmp;

    public GameObject GoalText;

    public int activePlayer = 0;

    public bool aiPlayer = true;

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
                CreateUnit(1, i, 0, PlayerPiece.UnitType.king);
            }
            else
            {
                CreateUnit(1, i, 0, PlayerPiece.UnitType.pawn);
            }
        }
        for (int i = 0; i < 5; i++)
        {
            if (i == 2)
            {
                CreateUnit(2, i, 4, PlayerPiece.UnitType.king);
            }
            else
            {
                CreateUnit(2, i, 4, PlayerPiece.UnitType.pawn);
            }
        }
    }

    //para pegar os nós onde ficam os mestres
    public void InitGoalNodes()
    {
        p1GoalNode = BoardManager.GetNodeS(2, 4);
        p2GoalNode = BoardManager.GetNodeS(2, 0);
    }

    public void RunGame()
    {
        activePlayer = 1;
        StartCoroutine(SetStartingActivePlayer());
    }

    IEnumerator SetStartingActivePlayer()
    {
        yield return new WaitForSeconds(.5f);
        ActionParams data = new();
        data.Put("activePlayer", activePlayer);
        EventManager.TriggerEvent("ActivePlayer", data);
    }

    public void CreateUnit(int playerId, int x, int y, PlayerPiece.UnitType utype)
    {
        PlayerPiece temp;
        GameObject newGo;
        BoardTile node;

        if (utype == PlayerPiece.UnitType.pawn)
        {
            newGo = Instantiate(playerPawnPrefab, Vector3.zero, Quaternion.identity);
        }
        else
        {
            newGo = Instantiate(playerKingPrefab, Vector3.zero, Quaternion.identity);
        }

        temp = newGo.transform.GetComponent<PlayerPiece>();
        node = BoardManager.GetNodeS(x, y);
        node.occupyingUnit = temp;
        temp.node = node;

        temp.unitObj.transform.position = node.GetPosition() + new Vector3(0, 0.1f, 0);

        if (playerId == 1)
        {
            newGo.GetComponent<Renderer>().material.color = Color.red;
            temp.ptype = PlayerPiece.Player.p1;
            player1Units.Add(temp);
        }
        else
        {
            newGo.GetComponent<Renderer>().material.color = Color.blue;
            temp.ptype = PlayerPiece.Player.p2;
            player2Units.Add(temp);
        }
    }

    public void OnEndPlayerMovement(string eventName, ActionParams data)
    {
        int receivedPlayer = data.Get<int>("activePlayer");
        BoardTile receivedNode = data.Get<BoardTile>("lastSelectedNode");

        if (CheckForGameEnd(receivedNode))
        {
            ActionParams temp = new();
            temp.Put("activePlayer", activePlayer);
            EventManager.TriggerEvent("GameOver", temp);
        }
        else
        {
            activePlayer = (receivedPlayer == 1) ? 2 : 1;

            if (aiPlayer && activePlayer == 2)
            {
                EventManager.TriggerEvent("AITurn", new ActionParams());
            }
            else
            {
                ActionParams temp = new();
                temp.Put("activePlayer", activePlayer);
                EventManager.TriggerEvent("ActivePlayer", temp);
            }
        }
    }

    public bool CheckForGameEnd(BoardTile node)
    {
        //verifica se os reis foram capturados
        if (p1KingCaptured || p2KingCaptured)
        {
            return true;
        }

        //Verifica se o rei adversario chegou no campo do outro
        if (node == p1GoalNode)
        {
            if (p1GoalNode.occupyingUnit.usType == PlayerPiece.UnitType.king &&
                p1GoalNode.occupyingUnit.ptype == PlayerPiece.Player.p1)
            {
                return true;
            }
        }
        if (node == p2GoalNode)
        {
            if (p2GoalNode.occupyingUnit.usType == PlayerPiece.UnitType.king &&
                p2GoalNode.occupyingUnit.ptype == PlayerPiece.Player.p2)
            {
                return true;
            }
        }

        return false;
    }

    public static void RemoveUnitAtNode(BoardTile node)
    {
        if (node.occupyingUnit != null)
        {
            PlayerPiece temp = node.occupyingUnit;
            if (temp.usType == PlayerPiece.UnitType.king)
            {
                if (temp.ptype == PlayerPiece.Player.p1)
                {
                    instance.p1KingCaptured = true;
                }
                else if (temp.ptype == PlayerPiece.Player.p2)
                {
                    instance.p2KingCaptured = true;
                }
            }

            RemoveUnitFromPlayerList(temp);
            node.occupyingUnit = null;
            temp.Destroy();
        }
    }

    public static void RemoveUnitFromPlayerList(PlayerPiece unit)
    {
        if (unit.ptype == PlayerPiece.Player.p1)
        {
            instance.player1Units.Remove(unit);
        }
        else
        {
            instance.player2Units.Remove(unit);
        }
    }

    public static List<BoardTile> BuildUnitMoveList(OnitamaCard card, BoardTile node)
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

    private List<BoardTile> LegalUnitMoves(List<CardMove> moves, int x, int y, int mult)
    {
        List<BoardTile> rList = new();

        foreach (CardMove move in moves)
        {
            BoardTile temp = BoardManager.GetNodeS(move.x * mult + x, move.y * mult + y);
            if (temp != null)
            {
                rList.Add(temp);
            }
        }

        return rList;
    }

    public static List<PlayerPiece> GetPlayerUnitList(int playerId)
    {
        return (playerId == 1) ? instance.player1Units : instance.player2Units;
    }

    public void CleanUpPlayerUnits()
    {
        foreach (PlayerPiece unit in player1Units)
        {
            unit.Destroy();
        }
        foreach (PlayerPiece unit in player2Units)
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
