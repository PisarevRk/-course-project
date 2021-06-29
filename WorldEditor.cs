using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;


public class WorldEditor : MonoBehaviour
{
    GameObject[,] mapRender;
    public Camera mainCamera;
    int[,] mapId;
    public Tiles[] tiles; // база данных с тайлами карты
    public int xMousPos; 
    public int yMousPos;
    public float yMousPosConvert;
    public int mapXsize;
    public int mapYsize;
    public float yTileOffset;
    public float cameraSpeed;
    int id = 0;
    public GameObject parent;
    public bool onUI;
    public string stringId;

    public void swithPref(int x)
    {
        id = x;
        
    }

    void Start()
    {
        mapRender = new GameObject[mapXsize, mapYsize];
        mapId = new int[mapXsize, mapYsize];

        for (int i = 0; i < mapXsize; i++)
        {
            for (int j = 0; j < mapYsize; j++)
            {
                mapId[i, j] = -1;
            }
        }

        string s = "";
        string Filepath = @"C:\Users\Poma\Desktop\Программы\Unity\Course Project v.3.1\Text.txt";
        if (File.Exists(Filepath))
            s = File.ReadAllText(Filepath);

        string[] stringId = s.Split(' ');

        for (int i = 0; i < mapXsize; i++)
        {
            for (int j = 0; j < mapYsize; j++)
            {
                mapId[i, j] = int.Parse(stringId[i * mapXsize + j]);
                if (mapId[i, j] != -1)
                    mapRender[i, j] = Instantiate(tiles[mapId[i, j]].gameObject, new Vector3(i, j - (yTileOffset * j), tiles[mapId[i, j]].transform.position.z), Quaternion.identity, parent.transform);
            }
        }
    }

    float Convert(float y)
    {
        float y1 = 0;
        while(y1 < y)
            y1 += (1 - yTileOffset);        
        float y2 = y1 - (1 - yTileOffset);
        if (y1-y > y-y2) return y2;    
        else return y1;
    }
    public void End()
    {
        stringId = "";
        for (int i = 0; i < mapXsize; i++)
        {
            for (int j = 0; j < mapYsize; j++)
            {
                stringId += mapId[i, j].ToString();
                stringId += " ";
            }
        }
        string Filepath = @"C:\Users\Poma\Desktop\Программы\Unity\Course Project v.3.1\Text.txt";
        File.WriteAllText(Filepath, stringId);
    }
    // Update is called once per frame
    void Update()
    {
        xMousPos = (int)Math.Round(Camera.main.ScreenToWorldPoint(Input.mousePosition).x);
        yMousPosConvert = Convert(Camera.main.ScreenToWorldPoint(Input.mousePosition).y);
        yMousPos = (int)Math.Round(yMousPosConvert / (1 - yTileOffset));

        if (!onUI)
        {
            if (Input.GetMouseButton(0))
            {
                if (xMousPos < mapXsize && xMousPos > 0 && yMousPos < mapYsize && yMousPos > 0)
                    if (mapRender[xMousPos, yMousPos] == null)
                    {
                        mapRender[xMousPos, yMousPos] = Instantiate(tiles[id].gameObject, new Vector3(xMousPos, yMousPosConvert, tiles[id].transform.position.z), Quaternion.identity, parent.transform);
                        mapId[xMousPos, yMousPos] = id;
                    }
            }
            if (Input.GetMouseButton(1))
            {
                if (xMousPos < mapXsize && xMousPos > 0 && yMousPos < mapYsize && yMousPos > 0)
                    if (mapRender[xMousPos, yMousPos] != null)
                    {
                        Destroy(mapRender[xMousPos, yMousPos].gameObject);
                        mapRender[xMousPos, yMousPos] = null;
                        mapId[xMousPos, yMousPos] = -1;
                    }
            }
            if (Input.GetMouseButton(2))
            {
                Vector3 targetPosition = new Vector3(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y, mainCamera.transform.position.z);
                mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, targetPosition, cameraSpeed * Time.deltaTime);  
            }
            mainCamera.orthographicSize -= Input.GetAxis("Mouse ScrollWheel") * 2;
            if (mainCamera.orthographicSize < 3)
            {
                mainCamera.orthographicSize = 3;
            }
            if (mainCamera.orthographicSize > 13)
            {
                mainCamera.orthographicSize = 13;
            }
        }   
    }
}
