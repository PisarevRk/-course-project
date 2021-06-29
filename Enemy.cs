using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
public class Enemy : MonoBehaviour
{
    public Player player;
    public World world;
    public MapGenerator mapGenerator;
    public Enemy[] enemies;

    public int moveAreaX1;
    public int moveAreaX2;
    public int moveAreaY1;
    public int moveAreaY2;
    void Start()
    {
        world.onTic += OnTic;
        transform.position = new Vector3(moveAreaX1, moveAreaY1*(1-mapGenerator.yTileOffset), transform.position.z);
    }
    public void OnTic()
    {
        Vector3 pt = player.transform.position;
        if (Math.Abs((int)pt.x - (int)transform.position.x) < 3 && Math.Abs((int)(pt.y / (1 - mapGenerator.yTileOffset)) - (int)(transform.position.y / (1 - mapGenerator.yTileOffset))) < 3)
        {
            print("Вижу");
            if ((int)pt.x - (int)transform.position.x < 0 
                && mapGenerator.tiles[mapGenerator.map[(int)transform.position.x - 1, (int)(transform.position.y / (1 - mapGenerator.yTileOffset))]].isMove 
                && (int)(transform.position.x - 1) >= moveAreaX1)
                transform.position = new Vector3(transform.position.x - 1, transform.position.y, -1);
            else if ((int)pt.x - (int)transform.position.x > 0
                && mapGenerator.tiles[mapGenerator.map[(int)transform.position.x + 1, (int)(transform.position.y / (1 - mapGenerator.yTileOffset))]].isMove
                && (int)(transform.position.x + 1) <= moveAreaX2)
                transform.position = new Vector3(transform.position.x + 1, transform.position.y, -1);
            else if ((int)(pt.y / (1 - mapGenerator.yTileOffset)) - (int)(transform.position.y / (1 - mapGenerator.yTileOffset)) > 0
                && mapGenerator.tiles[mapGenerator.map[(int)transform.position.x, (int)((transform.position.y + (1 - mapGenerator.yTileOffset)) / (1 - mapGenerator.yTileOffset))]].isMove
                && transform.position.y + (1 - mapGenerator.yTileOffset) <= moveAreaY2 * (1 - mapGenerator.yTileOffset))
                transform.position = new Vector3(transform.position.x, transform.position.y + (1 - mapGenerator.yTileOffset), -1);
            else if ((int)(pt.y / (1 - mapGenerator.yTileOffset)) - (int)(transform.position.y / (1 - mapGenerator.yTileOffset)) < 0
                && mapGenerator.tiles[mapGenerator.map[(int)transform.position.x, (int)((transform.position.y - (1 - mapGenerator.yTileOffset)) / (1 - mapGenerator.yTileOffset))]].isMove
                && transform.position.y - (1 - mapGenerator.yTileOffset) >= moveAreaY1 * (1 - mapGenerator.yTileOffset) + 0.1f)
                transform.position = new Vector3(transform.position.x, transform.position.y - (1 - mapGenerator.yTileOffset), -1);
        }
        else
        {
            System.Random rnd = new System.Random();

            bool[] move = new bool[] { false, false, false, false, true };
            if ((int)(transform.position.x + 1) <= moveAreaX2 && mapGenerator.tiles[mapGenerator.map[(int)transform.position.x + 1, (int)(transform.position.y / (1 - mapGenerator.yTileOffset))]].isMove)
                move[0] = true;
            if ((int)(transform.position.x - 1) >= moveAreaX1 && mapGenerator.tiles[mapGenerator.map[(int)transform.position.x - 1, (int)(transform.position.y / (1 - mapGenerator.yTileOffset))]].isMove)
                move[1] = true;
            if (transform.position.y + (1 - mapGenerator.yTileOffset) <= moveAreaY2 * (1 - mapGenerator.yTileOffset) && mapGenerator.tiles[mapGenerator.map[(int)transform.position.x, (int)((transform.position.y + (1 - mapGenerator.yTileOffset)) / (1 - mapGenerator.yTileOffset))]].isMove)
                move[2] = true;
            if (transform.position.y - (1 - mapGenerator.yTileOffset) >= moveAreaY1 * (1 - mapGenerator.yTileOffset)+0.1f && mapGenerator.tiles[mapGenerator.map[(int)transform.position.x, (int)((transform.position.y - (1 - mapGenerator.yTileOffset)) / (1 - mapGenerator.yTileOffset))]].isMove)
                move[3] = true;

            int x = rnd.Next(5);
            while (move[x] != true)
                x = rnd.Next(5);

            switch (x)
            {
                case 0:
                    print("Двигаюсь вправо");
                    transform.position = new Vector3(transform.position.x + 1, transform.position.y, -1);
                    break;
                case 1:
                    print("Двигаюсь влево");
                    transform.position = new Vector3(transform.position.x - 1, transform.position.y, -1);
                    break;
                case 2:
                    print("Двигаюсь вверх");
                    transform.position = new Vector3(transform.position.x, transform.position.y + (1 - mapGenerator.yTileOffset), -1);
                    break;
                case 3:
                    print("Двигаюсь вниз");
                    transform.position = new Vector3(transform.position.x, transform.position.y - (1 - mapGenerator.yTileOffset), -1);
                    break;
                case 4:
                    print("Не двигаюсь");
                    break;
            }

        }



    }
    // Update is called once per frame
    void Update()
    {
        Vector3 pt = player.transform.position;
        if (transform.position.x == pt.x && transform.position.y == pt.y)
        {
            SceneManager.LoadScene("SampleScene");
        }
    }
}
