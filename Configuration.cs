using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public enum Audio { movpiece, neg1, other }

public class JSONSaver
{
    public static void Save<T>(string path, T obj)
    {
        File.WriteAllText(path, JsonUtility.ToJson(obj));
    }

    public static T Load<T>(string path) where T : class
    {
        if (File.Exists(path))
        {
            string dataAsJson = File.ReadAllText(path);
            return JsonUtility.FromJson<T>(dataAsJson);
        }
        else
        {
            return null;
        }
    }
}

[System.Serializable]
public class ClipController
{
    public static AudioClip[] clips;
    public string[] setClips;

    // Start is called before the first frame update
    public void Awake()
    {
        clips = Configuration.loadClipsFromNames(setClips);
    }
}

[System.Serializable]
public class UnitController
{

    public static Vector2[] healthPos;
    public Vector2[] setHealthPos;

    public static MoveType[] moveTypes;
    public MoveType[] setMoveTypes;

    public static UnitTypeValues[] values;
    public UnitTypeValues[] setValues;

    // Start is called before the first frame update
    public void Awake()
    {
        healthPos = setHealthPos;
        moveTypes = setMoveTypes;
        values = setValues;
    }
}

[System.Serializable]
public class TerrainController
{
    public static int[] groundMoveCost;
    public int[] setGroundMoveCost;

    // Start is called before the first frame update
    public void Awake()
    {
        groundMoveCost = setGroundMoveCost;
    }
}

[System.Serializable]
public class RuleController
{
    public static bool deterministicMoves;
    public bool setDeterministicMoves;

    // Start is called before the first frame update
    public void Awake()
    {
        deterministicMoves = setDeterministicMoves;
    }
}

[System.Serializable]
public class SpriteController
{
    public static Sprite[] terrainSprites;
    public string[] setTerrainSprites;

    public static Sprite[] backgroundSprites1;
    public string[] setbackgroundSprites1;

    public static Sprite[] backgroundSprites2;
    public string[] setbackgroundSprites2;

    public static Sprite[] backgroundSprites3;
    public string[] setbackgroundSprites3;

    public static Sprite[] backgroundSprites4;
    public string[] setbackgroundSprites4;

    public static Sprite[] bonus1Sprites;
    public string[] setBonus1Sprites;

    public static Sprite[] bonus2Sprites;
    public string[] setBonus2Sprites;

    public static Sprite[] riverSprites;
    public string[] setRiverSprites;

    public static Sprite[] forestSprites;
    public string[] setForestSprites;

    public static Sprite[] hillSprites;
    public string[] setHillSprites;

    public static Sprite[] mountainSprites;
    public string[] setMountainSprites;

    public static Sprite[] oceanSprites;
    public string[] setOceanSprites;

    public static Sprite[] riverMouthSprites;
    public string[] setRiverMouthSprites;

    public static Sprite[] unitSprites;
    public string[] setUnitSprites;

    // Start is called before the first frame update
    public void Awake()
    {
        terrainSprites = Configuration.loadSpritesFromNames(setTerrainSprites, false);
        backgroundSprites1 = Configuration.loadSpritesFromNames(setbackgroundSprites1, true);
        backgroundSprites2 = Configuration.loadSpritesFromNames(setbackgroundSprites2, true);
        backgroundSprites3 = Configuration.loadSpritesFromNames(setbackgroundSprites3, true);
        backgroundSprites4 = Configuration.loadSpritesFromNames(setbackgroundSprites4, true);
        riverSprites = Configuration.loadSpritesFromNames(setRiverSprites, false);
        forestSprites = Configuration.loadSpritesFromNames(setForestSprites, false);
        hillSprites = Configuration.loadSpritesFromNames(setHillSprites, false);
        mountainSprites = Configuration.loadSpritesFromNames(setMountainSprites, false);
        bonus1Sprites = Configuration.loadSpritesFromNames(setBonus1Sprites, false);
        bonus2Sprites = Configuration.loadSpritesFromNames(setBonus2Sprites, false);
        oceanSprites = Configuration.loadSpritesFromNames(setOceanSprites, false);
        riverMouthSprites = Configuration.loadSpritesFromNames(setRiverMouthSprites, false);
        unitSprites = Configuration.loadSpritesFromNames(setUnitSprites, false);
    }
}



public class Configuration : MonoBehaviour
{
    public RuleController rules;
    public TerrainController terrains;
    public UnitController units;
    public SpriteController sprites;
    public ClipController clips;

    public string path;

    public static string[] saveNames(Sprite[] sprites, string path)
    {
        string[] resultNames = new string[sprites.Length];
        for (int i = 0; (i < sprites.Length); i++)
        {
            Sprite currentMapping = sprites[i];
            string currentName = currentMapping.name;
            resultNames[i] = path + currentName;
        }

        return resultNames;
    }

    public static string[] saveNames(AudioClip[] clips, string path)
    {
        string[] resultNames = new string[clips.Length];
        for (int i = 0; (i < clips.Length); i++)
        {
            AudioClip currentMapping = clips[i];
            string currentName = currentMapping.name;
            resultNames[i] = path + currentName;
        }

        return resultNames;
    }

    public static Sprite[] loadSpritesFromNames(string[] names, bool skip)
    {
        Sprite[] results = new Sprite[names.Length];
        for (int i = 0; (i < names.Length); i++)
        {
            string currentName = names[i];
            string[] filename;
            if (!skip)
                filename = currentName.Split('_');
            else
            {
                results[i] = Resources.Load<Sprite>(currentName);
                continue;
            }

            if (filename.Length == 1)
            {
                results[i] = Resources.Load<Sprite>(filename[0]);
            }
            else
            {
                try
                {
                    results[i] = Resources.LoadAll<Sprite>(filename[0])[int.Parse(filename[1])];
                } catch
                {
                    Debug.Log(filename[0] + "_" + filename[1]);
                }
            }
        }
        return results;
    }

    public static AudioClip[] loadClipsFromNames(string[] names)
    {
        AudioClip[] results = new AudioClip[names.Length];
        for (int i = 0; (i < names.Length); i++)
        {
            string currentName = names[i];
            string[] filename = currentName.Split('_');

            if (filename.Length == 1)
            {
                results[i] = Resources.Load<AudioClip>(filename[0]);
            }
            else
            {
                results[i] = Resources.LoadAll<AudioClip>(filename[0])[int.Parse(filename[1])];
            }
        }
        return results;
    }

    public void Awake()
    {
        if (File.Exists(path + "Rules.json"))
        {
            rules = JSONSaver.Load<RuleController>(path + "Rules.json");
        }
        else
        {
            JSONSaver.Save<RuleController>(path + "Rules.json", rules);
        }

        if (File.Exists(path + "Terrains.json"))
        {
            terrains = JSONSaver.Load<TerrainController>(path + "Terrains.json");
        }
        else
        {
            JSONSaver.Save<TerrainController>(path + "Terrains.json", terrains);
        }

        if (File.Exists(path + "Units.json"))
        {
            units = JSONSaver.Load<UnitController>(path + "Units.json");
        }
        else
        {
            JSONSaver.Save<UnitController>(path + "Units.json", units);
        }

        if (File.Exists(path + "Sprites.json"))
        {
            sprites = JSONSaver.Load<SpriteController>(path + "Sprites.json");
        }
        else
        {
            JSONSaver.Save<SpriteController>(path + "Sprites.json", sprites);
        }

        if (File.Exists(path + "Clips.json"))
        {
            clips = JSONSaver.Load<ClipController>(path + "Clips.json");
        }
        else
        {
            JSONSaver.Save<ClipController>(path + "Clips.json", clips);
        }

        rules.Awake();
        terrains.Awake();
        units.Awake();
        sprites.Awake();
        clips.Awake();
    }

    public void forceSave()
    {
        JSONSaver.Save<RuleController>(path + "Rules.json", rules);
        JSONSaver.Save<TerrainController>(path + "Terrains.json", terrains);
        JSONSaver.Save<UnitController>(path + "Units.json", units);
        JSONSaver.Save<SpriteController>(path + "Sprites.json", sprites);
        JSONSaver.Save<ClipController>(path + "Clips.json", clips);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
