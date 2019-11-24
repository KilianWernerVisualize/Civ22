using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonusResource : MonoBehaviour
{
    public static bool[,] hutIdx;
    public static bool[,] hut2Idx;
    public static int[,] bonus1Idx;
    public static int[,] bonus2Idx;
    public static int[,] shieldIdx;

    public static Vector2 makePatternCoords(int patternX, int patternY, int freqY, int width, int xmod, int yoff, int xoff)
    {
        patternY -= (8 * ((Map.seed - 1) / 2)) + 20 * ((Map.seed - 1) % 2);
        patternX -= 6 * ((Map.seed - 1) % 2);

        int firstY = (20 + yoff) % freqY;

        patternY -= firstY;

        int row = Mathf.RoundToInt((float)patternY / freqY);

        if (row < 0)
        {
            row += (xmod / 4) * 1000;
        }
        row %= xmod / 4;

        if (patternY < 0)
        {
            patternY += freqY * 1000;
        }
        patternY %= freqY;

        int firstX = (-row * 4 + xoff);

        if (firstX < 0)
        {
            firstX += xmod;
        }
        firstX %= xmod;

        patternX -= firstX;
        if (patternX < 0)
        {
            patternX += width * 1000;
        }
        patternX %= width;

        return new Vector2(patternX, patternY);
    }

    public static void initIdx()
    {
        if (hutIdx == null)
        {
            hutIdx = new bool[16, 32];
            hutIdx[8, 0] = true;
            hutIdx[11, 2] = true;
            hutIdx[1, 4] = true;
            hutIdx[4, 6] = true;
            hutIdx[7, 6] = true;
            hutIdx[10, 8] = true;
            hutIdx[0, 10] = true;
            hutIdx[3, 12] = true;
        }
        if (hut2Idx == null)
        {
            hut2Idx = new bool[16, 32];
            hut2Idx[1, 0] = true;
            hut2Idx[4, 2] = true;
            hut2Idx[7, 2] = true;
            hut2Idx[10, 4] = true;
            hut2Idx[0, 6] = true;
            hut2Idx[3, 8] = true;
            hut2Idx[6, 8] = true;
            hut2Idx[9, 10] = true;
        }

        if (shieldIdx == null)
        {
            shieldIdx = new int[4, 2];
            shieldIdx[0, 0] = 1;
            shieldIdx[1, 0] = 1;
            shieldIdx[2, 1] = 1;
            shieldIdx[3, 1] = 1;
        }

        if (bonus1Idx == null)
        {
            bonus1Idx = new int[8, 32];
            int[] pattern = new int[] { 0, 0, 0, 0, 0, 0, 0, 0 };
            int seed = Map.seed % 64;
            if (seed / 16 < 1)
                pattern = new int[] { 1, 1, 1, 1, 1, 1, 1, 1 };
            else if (seed / 16 < 2)
                pattern = new int[] { 2, 2, 1, 1, 1, 1, 2, 2 };
            else if (seed / 16 < 3)
                pattern = new int[] { 2, 2, 1, 1, 1, 1, 2, 2 };
            else switch (seed)
                {
                    case 48:
                        pattern = new int[] { 2, 1, 1, 1, 2, 2, 1, 2 };
                        break;
                    case 49:
                        pattern = new int[] { 1, 1, 1, 2, 2, 1, 2, 2 };
                        break;
                    case 50:
                        pattern = new int[] { 2, 2, 1, 2, 2, 1, 1, 1 };
                        break;
                    case 51:
                        pattern = new int[] { 2, 2, 2, 1, 1, 2, 1, 1 };
                        break;
                    case 52:
                        pattern = new int[] { 2, 1, 1, 1, 2, 2, 1, 2 };
                        break;
                    case 53:
                        pattern = new int[] { 1, 2, 1, 1, 2, 2, 2, 1 };
                        break;
                    case 54:
                        pattern = new int[] { 2, 2, 1, 2, 2, 1, 1, 1 };
                        break;
                    case 55:
                        pattern = new int[] { 2, 2, 2, 1, 1, 2, 1, 1 };
                        break;
                    case 56:
                        pattern = new int[] { 1, 1, 2, 1, 1, 2, 2, 2 };
                        break;
                    case 57:
                        pattern = new int[] { 1, 2, 1, 1, 2, 2, 2, 1 };
                        break;
                    case 58:
                        pattern = new int[] { 1, 2, 2, 2, 1, 1, 2, 1 };
                        break;
                    case 59:
                        pattern = new int[] { 2, 2, 2, 1, 1, 2, 1, 1 };
                        break;
                    case 60:
                        pattern = new int[] { 1, 1, 2, 1, 1, 2, 2, 2 };
                        break;
                    case 61:
                        pattern = new int[] { 1, 1, 1, 2, 2, 1, 2, 2 };
                        break;
                    case 62:
                        pattern = new int[] { 1, 2, 2, 2, 1, 1, 2, 1 };
                        break;
                    case 63:
                        pattern = new int[] { 2, 1, 2, 2, 1, 1, 1, 2 };
                        break;
                    default:
                        break;
                }
            bonus1Idx[3, 0] = pattern[0];
            bonus1Idx[1, 2] = pattern[1];
            bonus1Idx[6, 2] = pattern[2];
            bonus1Idx[4, 4] = pattern[3];
            bonus1Idx[2, 6] = pattern[4];
            bonus1Idx[0, 8] = pattern[5];
            bonus1Idx[5, 8] = pattern[6];
            bonus1Idx[3, 10] = pattern[7];
        }

        if (bonus2Idx == null)
        {
            bonus2Idx = new int[8, 32];
            int[] pattern = new int[] { 0, 0, 0, 0, 0, 0, 0, 0 };
            int seed = Map.seed % 64;

            if (seed / 16 < 1)
                pattern = new int[] { 2, 2, 2, 2, 2, 2, 2, 2 };
            else if (seed / 16 < 2)
                pattern = new int[] { 1, 1, 2, 1, 2, 1, 2, 2 };
            else if (seed / 16 < 3)
                pattern = new int[] { 2, 1, 2, 1, 1, 2, 1, 2 };
            else switch (seed)
                {
                    case 48:
                        pattern = new int[] { 1, 2, 1, 1, 1, 2, 2, 2 };
                        break;
                    case 49:
                        pattern = new int[] { 1, 1, 2, 2, 1, 2, 2, 1 };
                        break;
                    case 50:
                        pattern = new int[] { 1, 2, 2, 1, 2, 2, 1, 1 };
                        break;
                    case 51:
                        pattern = new int[] { 2, 2, 2, 1, 1, 1, 2, 1 };
                        break;
                    case 52:
                        pattern = new int[] { 2, 1, 1, 2, 1, 1, 2, 2 };
                        break;
                    case 53:
                        pattern = new int[] { 1, 1, 2, 2, 1, 2, 2, 1 };
                        break;
                    case 54:
                        pattern = new int[] { 2, 1, 2, 2, 2, 1, 1, 1 };
                        break;
                    case 55:
                        pattern = new int[] { 2, 2, 2, 1, 1, 1, 2, 1 };
                        break;
                    case 56:
                        pattern = new int[] { 2, 1, 1, 2, 1, 1, 2, 2 };
                        break;
                    case 57:
                        pattern = new int[] { 1, 1, 1, 2, 2, 2, 1, 2 };
                        break;
                    case 58:
                        pattern = new int[] { 2, 1, 2, 2, 2, 1, 1, 1 };
                        break;
                    case 59:
                        pattern = new int[] { 2, 2, 1, 1, 2, 1, 1, 2 };
                        break;
                    case 60:
                        pattern = new int[] { 1, 2, 1, 1, 1, 2, 2, 2 };
                        break;
                    case 61:
                        pattern = new int[] { 1, 1, 1, 2, 2, 2, 1, 2 };
                        break;
                    case 62:
                        pattern = new int[] { 1, 2, 2, 1, 2, 2, 1, 1 };
                        break;
                    case 63:
                        pattern = new int[] { 2, 2, 1, 1, 2, 1, 1, 2 };
                        break;
                    default:
                        break;
                }

            bonus2Idx[1, 0] = pattern[0];
            bonus2Idx[4, 2] = pattern[1];
            bonus2Idx[2, 4] = pattern[2];
            bonus2Idx[0, 6] = pattern[3];
            bonus2Idx[5, 6] = pattern[4];
            bonus2Idx[3, 8] = pattern[5];
            bonus2Idx[1, 10] = pattern[6];
            bonus2Idx[4, 12] = pattern[7];
        }
    }

    public static bool spawnHut(Terrain terrain, int patternX, int patternY)
    {
        bool hut = false;

        if (terrain == Terrain.Ocean)
            return hut;

        Vector2 patternCoords = makePatternCoords(patternX, patternY, 32, 16, 16, -12, 7);

        hut = hutIdx[(int)patternCoords.x, (int)patternCoords.y];

        patternCoords = makePatternCoords(patternX, patternY, 32, 16, 16, 5, 13);

        hut = hut || hut2Idx[(int)patternCoords.x, (int)patternCoords.y];

        return (hut);

    }

    public static int spawnBonus(Terrain terrain, int patternX, int patternY)
    {
        int bonus = 0;

        if (terrain == Terrain.Grassland)
        {
            int x = patternX % 4;
            int row = (patternY / 2) % 4;
            int y = patternY % 2;
            x -= row;
            if (x < 0)
                x += 4;
            bonus += shieldIdx[x, y];
        }
        else
        {
            Vector2 patternCoords = makePatternCoords(patternX, patternY, 32, 8, 8, 5, 7);

            bonus = bonus1Idx[(int)patternCoords.x, (int)patternCoords.y];

            patternCoords = makePatternCoords(patternX, patternY, 32, 8, 8, -12, 2);

            bonus += bonus2Idx[(int)patternCoords.x, (int)patternCoords.y];
        }

        return bonus;
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
