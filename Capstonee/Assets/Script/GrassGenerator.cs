using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GrassGenerator : MonoBehaviour
{
    public int mapChunkSize = 239;

    public Grass[] grass;

    public void GenerateGrass(float[,] noise)
    {
        for (int y = 0; y < mapChunkSize; y++)
        {
            for (int x = 0; x < mapChunkSize; x++)
            { 
                float currentHeight = noise[x, y];
                foreach(var obj in grass) 
                {
                    if (currentHeight >= obj.height)
                    {
                        GameObject gra = obj.Grassobj[1];
                        /*Instantiate(gra);*/
                        /*gra.transform.position = new Vector2();*/
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }

    }

}

[System.Serializable]
public struct Grass
{
    public float height;
    public GameObject[] Grassobj;
}
