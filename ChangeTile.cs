using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeTile : MonoBehaviour
{
    public GameObject scrollView;
    public World wd;
    public GameObject destroyObject;
    public MapGenerator generator;
    public int[,] map;
    public int x;
    public int y;

    public void Unfreez()
    {
        wd.freezTime = false;
        Destroy(destroyObject.gameObject);
    }

    public void Replace(int id)
    {
        map[x, y] = id;
        generator.ReRender(x, y);
        wd.freezTime = false;
        Destroy(destroyObject.gameObject);
    }
}
