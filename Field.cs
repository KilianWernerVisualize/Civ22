using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Terrain { Desert, Plains, Grassland, Forest, Hill, Mountain, Tundra, Glacier, Swamp, Jungle, Ocean }

public class Field : MonoBehaviour, PathNode
{
    public GameObject hutPrefab;

    public SpriteRenderer back1;
    public SpriteRenderer back2;
    public SpriteRenderer back3;
    public SpriteRenderer back4;
    public SpriteRenderer ocean1;
    public SpriteRenderer ocean2;
    public SpriteRenderer ocean3;
    public SpriteRenderer ocean4;
    public SpriteRenderer topping;
    public SpriteRenderer riverTopping;

    [SerializeField]
    private Terrain inspectTerrain;
    public Terrain terrain
    {
        get { return inspectTerrain; }
        set {
            inspectTerrain = value;
            if (myRenderer)
            {
                if ((int)terrain > SpriteController.terrainSprites.Length)
                {
                    Debug.Log("TerrainIDX: " + terrain.ToString());
                    inspectTerrain = Terrain.Grassland;
                }
                else
                    myRenderer.sprite = SpriteController.terrainSprites[(int)terrain];
            }
        }
    }

    public SpriteRenderer myRenderer;

    private Map myMap;
    public Vector2 pos;

    public int road = 0;
    public bool river = false;
    public int bonus = 0;



    public bool hut = false;
    public List<Unit> units;
    public bool show = false;

    void OnMouseEnter()
    {
        Debug.Log(PathFinder.manhattanDistance(myMap.at(20, 20), this));
    }

    //Movement
    public int UnitID(Unit target)
    {
        for (int i = 0; (i < units.Count); i++)
        {
            if (target == units[i])
                return i;
        }
        return -1;
    }

    public float moveCost(Field startField, Unit unit)
    {
        if (bonus < 0)
            return 1000.0f;

        if (unit.moveType == MoveType.Naval)
        {
            if (terrain == Terrain.Ocean)
                return 1.0f;
            else
                return 1000.0f;
        }

        if (unit.moveType == MoveType.Airborne)
            return 1.0f;

        if (unit.moveType == MoveType.Explorer)
        {
            if (terrain != Terrain.Ocean)
            {
                return 0.333332f;
            } else
            {
                return 1000.0f;
            }
        }

        if ((startField.river && river) || (startField.road != 0 && road == 1) || (startField.road == 1 && road != 0))
            return 0.333332f;
        if (startField.road == 2 && road == 2)
            return 0.0f;

        return TerrainController.groundMoveCost[(int)terrain];
    }

    public Field neighbor(int xdiff, int ydiff)
    {
        if (xdiff < 0 && ydiff < 0)
            return neighbor(0);
        if (xdiff == 0 && ydiff < 0)
            return neighbor(1);
        if (xdiff > 0 && ydiff < 0)
            return neighbor(2);
        if (xdiff > 0 && ydiff == 0)
            return neighbor(3);
        if (xdiff > 0 && ydiff > 0)
            return neighbor(4);
        if (xdiff == 0 && ydiff > 0)
            return neighbor(5);
        if (xdiff < 0 && ydiff > 0)
            return neighbor(6);
        if (xdiff < 0 && ydiff == 0)
            return neighbor(7);

        return this;
    }

    public Field neighbor(int idx)
    {
        switch (idx)
        {
            case 0:
                return myMap.at((int)pos.x - ((int)pos.y + 1) % 2, (int)pos.y - 1);
            case 1:
                return myMap.at((int)pos.x, (int)pos.y - 2);
            case 2:
                return myMap.at((int)pos.x + (int)pos.y % 2, (int)pos.y - 1);
            case 3:
                return myMap.at((int)pos.x + 1, (int)pos.y);
            case 4:
                return myMap.at((int)pos.x + (int)pos.y % 2, (int)pos.y + 1);
            case 5:
                return myMap.at((int)pos.x, (int)pos.y + 2);
            case 6:
                return myMap.at((int)pos.x - ((int)pos.y + 1) % 2, (int)pos.y + 1);
            case 7:
                return myMap.at((int)pos.x - 1, (int)pos.y);
        }
        return this;
    }

    public int x()
    {
        return (int)this.pos.x;
    }

    public int y()
    {
        return (int)this.pos.y;
    }

    public PathNode neighborp(int idx)
    {
        return neighbor(idx);
    }

    public float moveCost(Unit walker, int destinationNeighbor)
    {
        return moveCost(neighbor(destinationNeighbor), walker);
    }

    //Rendering
    void drawBackgrounds()
    {
        back1.sprite = SpriteController.backgroundSprites1[(int)neighbor(0).terrain];
        back2.sprite = SpriteController.backgroundSprites2[(int)neighbor(2).terrain];
        back3.sprite = SpriteController.backgroundSprites3[(int)neighbor(4).terrain];
        back4.sprite = SpriteController.backgroundSprites4[(int)neighbor(6).terrain];
    }

    void drawRiver()
    {
        if (!river)
            return;
        int idx = 0;
        int mult = 1;
        for (int i = 0; (i < 8); i += 2)
        {
            idx += (neighbor(i).river || neighbor(i).terrain == Terrain.Ocean) ? mult : 0;
            mult *= 2;
        }
        riverTopping.sprite = SpriteController.riverSprites[idx];
    }

    void drawTopping()
    {
        if (!(terrain == Terrain.Forest || terrain == Terrain.Hill || terrain == Terrain.Mountain))
            return;
        int idx = 0;
        int mult = 1;
        for (int i = 0; (i < 8); i += 2)
        {
            idx += (neighbor(i).terrain == terrain) ? mult : 0;
            mult *= 2;
        }
        topping.sprite = (terrain == Terrain.Forest) ? SpriteController.forestSprites[idx] : (terrain == Terrain.Hill) ? SpriteController.hillSprites[idx] : SpriteController.mountainSprites[idx];
    }

    void drawShores()
    {
        if (terrain != Terrain.Ocean)
        {
            ocean1.enabled = false;
            ocean2.enabled = false;
            ocean3.enabled = false;
            ocean4.enabled = false;
            return;
        }

        for (int i = 0; (i < 8); i += 2)
        {
            if (neighbor(i).river)
            {
                GameObject riverMouth = GameObject.Instantiate(hutPrefab, transform);
                riverMouth.GetComponent<SpriteRenderer>().sprite = SpriteController.riverMouthSprites[i/2];
            }
        }

        myRenderer.enabled = false;

        int idx = 0;
        int mult = 1;
        for (int i = 0; (i < 3); i++)
        {
            idx += (neighbor(i).terrain != Terrain.Ocean) ? mult : 0;
            mult *= 2;
        }
        ocean1.sprite = SpriteController.oceanSprites[idx];

        idx = 0;
        mult = 1;
        for (int i = 0; (i < 3); i++)
        {
            idx += (neighbor(i+4).terrain != Terrain.Ocean) ? mult : 0;
            mult *= 2;
        }
        ocean2.sprite = SpriteController.oceanSprites[8+idx];

        idx = 0;
        mult = 1;
        for (int i = 0; (i < 3); i++)
        {
            idx += (neighbor((i + 6) % 8).terrain != Terrain.Ocean) ? mult : 0;
            mult *= 2;
        }
        ocean3.sprite = SpriteController.oceanSprites[16 + idx];

        idx = 0;
        mult = 1;
        for (int i = 0; (i < 3); i++)
        {
            idx += (neighbor(i + 2).terrain != Terrain.Ocean) ? mult : 0;
            mult *= 2;
        }
        ocean4.sprite = SpriteController.oceanSprites[24 + idx];
    }

    // Start is called before the first frame update
    void Start()
    {
        if (bonus < 0)
            return;
        myMap = transform.parent.GetComponent<Map>();

        myRenderer.sprite = SpriteController.terrainSprites[(int)terrain];

        drawShores();
        drawTopping();
        drawRiver();
        drawBackgrounds();

        BonusResource.initIdx();

        hut = BonusResource.spawnHut(terrain, (int)pos.x, (int)pos.y);
        if (hut)
            GameObject.Instantiate(hutPrefab, transform);
        else
            bonus = BonusResource.spawnBonus(terrain, (int)pos.x, (int)pos.y);

        if (bonus != 0)
        {
            GameObject tmp = GameObject.Instantiate(hutPrefab, transform);
            tmp.GetComponent<SpriteRenderer>().sprite = (bonus == 1) ? SpriteController.bonus1Sprites[(int)inspectTerrain] : SpriteController.bonus2Sprites[(int)inspectTerrain];
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
