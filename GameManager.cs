using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public enum GameState { NotStarted, EndOfTurn, Movement }

public class GameManager : MonoBehaviour
{
    public NetManager net;
    public List<int> players;
    public int localPlayer;
    public int turnPlayer;

    public GameObject UnitPrefab;
    public Map myMap;

    public GameState gameState;

    public Unit activeUnit;

    private bool moveInputReset = true;

    // Start is called before the first frame update
    void Start()
    {
        net = GameObject.FindGameObjectWithTag("NetworkManager").GetComponent<NetManager>();
        myMap = GameObject.FindGameObjectWithTag("Map").GetComponent<Map>();
    }

    public bool searchMoveableUnit()
    {
        if (activeUnit && activeUnit.movePoints > 0.1)
            return true;

        GameObject[] units = GameObject.FindGameObjectsWithTag("Unit");
        for (int i = 0; (i < units.Length); i++)
        {
            if (units[i].GetComponent<Unit>().movePoints > 0.1 && units[i].GetComponent<Unit>().owner == localPlayer)
            {
                activeUnit = units[i].GetComponent<Unit>();
                GameObject.FindGameObjectWithTag("MainCamera").transform.position = new Vector3(activeUnit.transform.position.x, activeUnit.transform.position.y, -10);
                return true;
            }
        }

        return false;
    }

    public void makeVisibility()
    {
        foreach (Field field in myMap.fields)
        {
            field.show = false;
        }
        foreach (Field field in myMap.fields)
        {
            if (field.units.Count > 0 && field.units[0].owner == localPlayer)
            {
                field.show = true;
                for (int i = 0; (i < 8); i++)
                {
                    field.neighbor(i).show = true;
                }
            }
        }
    }

    public void doMovement()
    {
        //Yield Move Points on Spacebar
        if (Input.GetButtonUp("Yield"))
        {
            activeUnit.movePoints = 0;
            return;
        }

        //Actual Movement
        Vector2 input = new Vector2();
        input += new Vector2(0.0f, 1.0f) * Input.GetAxis("Vertical");
        input += new Vector2(1.0f, 0.0f) * Input.GetAxis("Horizontal");
        input += new Vector2(-1.0f, 1.0f) * Input.GetAxis("FirstDiagonal");
        input += new Vector2(1.0f, 1.0f) * Input.GetAxis("SecondDiagonal");

        input.y = -input.y;

        if (input.x > 0)
            input.x = 1.0f;
        if (input.x < 0)
            input.x = -1.0f;
        if (input.y > 0)
            input.y = 1.0f;
        if (input.y < 0)
            input.y = -1.0f;

        if (input.x == 0 && input.y == 0)
        {
            moveInputReset = true;
            return;
        }

        if (!moveInputReset)
            return;

        moveInputReset = false;

        MoveUnitAction move = new MoveUnitAction();
        move.x = (int)activeUnit.myField.pos.x;
        move.y = (int)activeUnit.myField.pos.y;
        move.unit = activeUnit.myField.UnitID(activeUnit);
        move.xdiff = (int)input.x;
        move.ydiff = (int)input.y;

        if (move.prepare())
            net.netSyncAction(move);
    }

    public void doEndOfTurn()
    {
        if (Input.GetAxis("Submit") != 0.0f)
        {
            gameState = GameState.Movement;

            GameObject[] units = GameObject.FindGameObjectsWithTag("Unit");
            for (int i = 0; (i < units.Length); i++)
            {
                units[i].GetComponent<Unit>().nextTurn();
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (gameState == GameState.NotStarted)
        {
            if (Input.GetAxis("Submit") != 0.0f && net.isHost)
            {
                NetManager.id = 0;
                foreach (int player in net.clients)
                    players.Add(player);
                net.netSyncAction(new StartGameAction(players));
            }
        }
        if (gameState == GameState.Movement)
        {
            if (!searchMoveableUnit())
            {
                gameState = GameState.EndOfTurn;
            }
            else
            {
                doMovement();
            }
        } else
        {
            doEndOfTurn();
        }
    }
}

[System.Serializable]
public class StartGameAction : Action
{
    public List<int> players;

    public StartGameAction(List<int> players)
    {
        this.players = players;
    }

    public override void perform()
    {
        manager().players = players;
        manager().gameState = GameState.Movement;
        manager().localPlayer = NetManager.id;

        GameObject[] units = GameObject.FindGameObjectsWithTag("Unit");
        for (int i = 0; (i < units.Length); i++)
        {
            units[i].GetComponent<Unit>().nextTurn();
        }

        Debug.Log(manager().players.Count);
        Debug.Log(manager().localPlayer);
    }
}
