using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UnitType { Settler, Engineer, Warrior, Phalanx, Bowman, Legion, Pike, Musketeer, Fanatic, Partisan, AlpineTroop, Infantry, Marine, Paratrooper, MechInf, Horseman, Chariot, Elephant, Crusader, Knight, Dragooner, Cavalry, Armor, Catapult, Canon, Artillery, Howitzer, Fighter, Bomber, Helicopter, StlthFighter, StlthBomber, Trireme, Caravele, Fregate, Galeon, Ironclad, Destroyer, Cruiser, AEGISCruiser, Battleship, Submarine, Carrier, Transporter, CruiseMissile, NuclearMissile, Diplomat, Spy, Caravan, Freight, Explorer, Captain}

[System.Serializable]
public struct UnitTypeValues
{
    public string longName;
    public int maxMovePoints;
    public int cost;
    public int attack;
    public int defense;
    public int health;
    public int fire;
}

public enum MoveType { Ground, Naval, Airborne, Explorer }

public class Unit : MonoBehaviour
{
    public Transform healthBar;
    private Vector3 healthBarStartPos;

    public SpriteRenderer myHealthBarRenderer;
    public SpriteRenderer myRenderer;

    public Field myField;

    public int owner;

    public MoveType moveType
    {
        get
        {
            return UnitController.moveTypes[(int)type];
        }
    }

    [SerializeField]
    private UnitType inspectType;
    public UnitType type
    {
        get { return inspectType; }
        set
        {
            inspectType = value;
            healthBar.localPosition = healthBarStartPos + new Vector3(UnitController.healthPos[(int)inspectType].x * 0.015625f, UnitController.healthPos[(int)inspectType].y * -0.015625f, 0.0f);
            if (myRenderer)
            {
                if ((int)inspectType > SpriteController.unitSprites.Length)
                {
                    Debug.Log("TerrainIDX: " + inspectType.ToString());
                    inspectType = UnitType.Settler;
                }
                else
                    myRenderer.sprite = SpriteController.unitSprites[(int)inspectType];
            }
        }
    }

    public float movePoints;
    public float maxMovePoints()
    {
        return 1.0f * UnitController.values[(int)type].maxMovePoints;
    }

    public void nextTurn()
    {
        movePoints = maxMovePoints();
    }

    // Start is called before the first frame update
    void Awake()
    {
        healthBarStartPos = healthBar.localPosition;
        type = inspectType;
    }

    // Update is called once per frame
    void Update()
    {
        myRenderer.enabled = myField.show;
        myHealthBarRenderer.enabled = myField.show;
    }
}


[System.Serializable]
public class CreateUnitAction : Action
{
    public int x;
    public int y;
    public int player;
    public UnitType unitType;
    public bool veteran;

    public override void perform()
    {
        Map myMap = GameObject.FindGameObjectWithTag("Map").GetComponent<Map>();
        Field myField = myMap.at(x, y);
        Unit myUnit = GameObject.Instantiate(manager().UnitPrefab).GetComponent<Unit>();

        myUnit.type = unitType;
        myUnit.myField = myField;
        myField.units.Add(myUnit);

        myUnit.owner = player;

        myUnit.transform.position = myField.transform.position;
        //manager().makeVisibility();
    }
}

[System.Serializable]
public class MoveUnitAction : Action
{
    public int x;
    public int y;
    public int unit;
    public int player;

    public int xdiff;
    public int ydiff;
    public float moveCost;

    public bool prepare()
    {
        Map myMap = GameObject.FindGameObjectWithTag("Map").GetComponent<Map>();
        Field myField = myMap.at(x, y);
        Unit myUnit = myField.units[unit];

        Field myTargetField = myField.neighbor(xdiff, ydiff);
        if (myTargetField == myMap.NoFieldPrefab)
        {
            if (local)
                myUnit.GetComponent<AudioSource>().PlayOneShot(ClipController.clips[(int)Audio.neg1]);
            this.moveCost = -1;
            return false;
        }

        float moveCost = myTargetField.moveCost(myUnit.myField, myUnit);
        if (moveCost > 999)
        {
            this.moveCost = -1;
            return false;
        }
        if (myUnit.movePoints != myUnit.maxMovePoints() && moveCost > myUnit.movePoints)
        {
            if (RuleController.deterministicMoves)
            {
                //if (local)
                    myUnit.GetComponent<AudioSource>().PlayOneShot(ClipController.clips[(int)Audio.neg1]);
                moveCost = -1;
                return false;
            }
            else
            {
                if (Random.value >= myUnit.movePoints / moveCost)
                {
                    manager().net.netSyncAction(new SkipMoveAction(x, y, unit, player));
                    moveCost = -1;
                    return false;
                }
            }
        }

        this.moveCost = moveCost;
        return true;
    }

    public override void perform()
    {
        Map myMap = GameObject.FindGameObjectWithTag("Map").GetComponent<Map>();
        Field myField = myMap.at(x, y);
        Unit myUnit = myField.units[unit];

        Field myTargetField = myField.neighbor(xdiff, ydiff);

        myUnit.movePoints = Mathf.Max(myUnit.movePoints - moveCost, 0.0f);

        myUnit.myField = myTargetField;
        myField.units.RemoveAt(unit);
        myTargetField.units.Add(myUnit);

        myUnit.transform.position = myTargetField.transform.position;
        myUnit.GetComponent<AudioSource>().PlayOneShot(ClipController.clips[(int)Audio.movpiece]);

        if (local)
            manager().makeVisibility();
    }
}

[System.Serializable]
public class SkipMoveAction : Action
{
    public int x;
    public int y;
    public int unit;
    public int player;

    public SkipMoveAction(int x, int y, int unit, int player)
    {
        this.x = x;
        this.y = y;
        this.unit = unit;
        this.player = player;
    }


    public override void perform()
    {
        Map myMap = GameObject.FindGameObjectWithTag("Map").GetComponent<Map>();
        Field myField = myMap.at(x, y);
        Unit myUnit = myField.units[unit];
        myUnit.movePoints = 0.0f;
    }
}


