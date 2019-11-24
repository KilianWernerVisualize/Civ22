using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Map : MonoBehaviour
{
    public static int seed;
    public bool flat = false;

    public GameObject FieldPrefab;
    public GameObject NoFieldPrefab;

    [HideInInspector]
    public Field[,] fields;

    public Field at(int x, int y)
    {
        if (flat)
        {
            if (x >= 0 && x < fields.GetLength(0) && y >= 0 && y < fields.GetLength(1))
            {
                return fields[x, y];
            }
            return NoFieldPrefab.GetComponent<Field>();
        } else
        {
            if (y >= 0 && y < fields.GetLength(1))
            {
                while (x < 0)
                    x += fields.GetLength(0);
                while (x >= fields.GetLength(0))
                    x -= fields.GetLength(0);
                return fields[x, y];
            }
            return NoFieldPrefab.GetComponent<Field>();
        }
    }

    public void loadTerrain(string file)
    {
        byte[] bytes = File.ReadAllBytes(file);
        int width = bytes[0]/2;
        int height = bytes[2];
        seed = bytes[7] * 16 * 16 + bytes[8];

        fields = new Field[width, height];

        for (int y = 0; (y < height); y++)
        {
            for (int x = 0; (x < width); x++)
            {
                GameObject field = GameObject.Instantiate(FieldPrefab, transform);
                field.transform.position = new Vector3(x + (y % 2) / 2.0f, -y / 4.0f, 0);
                int c = bytes[(x + y * width) * 6 + 98];
                int river = c / 128;
                c %= 128;
                field.GetComponent<Field>().terrain = (Terrain)c;
                field.GetComponent<Field>().river = (river > 0);
                field.GetComponent<Field>().pos = new Vector2(x, y);
                fields[x, y] = field.GetComponent<Field>();
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        loadTerrain(Application.dataPath+"/map.MP");

        if (GameObject.FindGameObjectWithTag("NetworkManager").GetComponent<NetManager>().isHost)
        {
            CreateUnitAction create = new CreateUnitAction();
            create.x = 20;
            create.y = 20;
            create.unitType = UnitType.Armor;

            GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>().net.netSyncAction(create);

            create.x = 25;
            create.y = 25;
            create.unitType = UnitType.Engineer;

            GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>().net.netSyncAction(create);

            create.x = 35;
            create.y = 35;
            create.unitType = UnitType.Dragooner;

            GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>().net.netSyncAction(create);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
