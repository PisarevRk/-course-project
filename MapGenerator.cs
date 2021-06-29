using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class MapGenerator : MonoBehaviour
{
    public int mapXsize;
    public int mapYsize;

    public int renderXsize;
    public int renderYsize;

    public float yTileOffset;

    public int[,] map; // массив id, описывающий карту 
    public Tiles[] tiles; // база данных с тайлами карты
    public GameObject[,] mapRender; // массив тайлов в игре
    public GameObject parent;
    public Player player;
    public World world;
    void Start()
    {
        world.onTic += OnTic;
        map = new int[mapXsize, mapYsize];
        mapRender = new GameObject[renderXsize * 2 + 10, renderYsize * 2 + 10];

        //================================================================================================
        // Заполняем массив карты из файла
        string s = "";
        string Filepath = @"C:\Users\Poma\Desktop\Программы\Unity\Course Project v.3.1\Text.txt";
        if (File.Exists(Filepath))
            s = File.ReadAllText(Filepath);

        string[] stringId = s.Split(' ');

        for (int i = 0; i < mapXsize; i++)
        {
            for (int j = 0; j < mapYsize; j++)
            {
                map[i, j] = int.Parse(stringId[i * mapXsize + j]);
            }
        }

        OnTic();
        //================================================================================================
    }

    public void OnTic()
    {
        //================================================================================================
        // Вызываются раз в промежуток времени, заданный в классе world

        // Чистим отрендеренную карту
        for (int i = 0; i < renderXsize * 2 + 10; i++)
        {
            for (int j = 0; j < renderYsize * 2 + 10; j++)
            {
                if (mapRender[i, j] != null && (mapRender[i, j].transform.position.x < (player.transform.position.x - renderXsize) || mapRender[i, j].transform.position.x > (player.transform.position.x + renderXsize) || mapRender[i, j].transform.position.y < (player.transform.position.y - renderYsize) || mapRender[i, j].transform.position.y > (player.transform.position.y + renderYsize)))
                {
                    Destroy(mapRender[i, j].gameObject);
                    mapRender[i, j] = null;
                }
            }
        }
        
        // определяем границы рендера
        int renderXstart = (int)(player.transform.position.x - renderXsize);
        int renderYstart = (int)(player.transform.position.y / (1 - yTileOffset) - renderYsize );
        int renderXend = (int)(player.transform.position.x + renderXsize);
        int renderYend = (int)(player.transform.position.y / (1 - yTileOffset) + renderYsize );

        if (renderXstart < 0) renderXstart = 0;
        else if (renderXend > mapXsize) renderXend = mapXsize;

        if (renderYstart < 0) renderYstart = 0;
        else if (renderYend > mapYsize) renderYend = mapYsize;

        // рендерим карту
        for (int i = renderXstart; i < renderXend; i++)
        {
            for (int j = renderYstart; j < renderYend; j++)
            {
                if (map[i, j] != -1 && mapRender[i, j] == null)
                    mapRender[i, j] = Instantiate(tiles[map[i, j]].gameObject, new Vector3(i, j - (yTileOffset * j), tiles[map[i, j]].transform.position.z), Quaternion.identity, parent.transform);
            }
        }
        //================================================================================================
    }

    public void ReRender(int i, int j)
    {
        Destroy(mapRender[i, j].gameObject);
        mapRender[i, j] = Instantiate(tiles[map[i, j]].gameObject, new Vector3(i, j - (yTileOffset * j), tiles[map[i, j]].transform.position.z), Quaternion.identity, parent.transform);
    }
    void Update()
    {
        
    }
}
