using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using System.IO;


public class Player : MonoBehaviour
{
    public World world;
    public Enemy enemy;
    public Camera mainCamera;
    public GameObject Uiparent;
    public GameObject prefabMarkToChangeWorld;
    GameObject[] marksToChangeWorld = new GameObject[5];
    public Text statusText;
    GameObject activUi;
    public MapGenerator mapGenerator;
    public float cameraSpeed;
    public Vector2 targetPosition;
    public Tiles target = null;
    private bool use = false;
    private bool gate = false;
    private bool changeWorld = false;

    

    void Start()
    {
        //================================================================================================
        // OnTic ловит ивент из класса world, ставим героя в координаты (0;0)
        world.onTic += OnTic;

        string s = "";
        string Filepath = @"C:\Users\Poma\Desktop\Программы\Unity\Course Project v.3.1\Position.txt";
        if (File.Exists(Filepath))
            s = File.ReadAllText(Filepath);

        if (s.Length == 1)
        {
            transform.position = new Vector3(enemy.transform.position.x, enemy.transform.position.y, transform.position.z);
            enemy.gameObject.SetActive(false);

            File.WriteAllText(Filepath, "");
        }
        else
        {
            transform.position = new Vector3(1, 1 - mapGenerator.yTileOffset, transform.position.z);
        }   
        //================================================================================================
    }
    //================================================================================================
    // Вызываются при нажатии кнопок управления
    public void MoveUp()
    {
        //================================================================================================
        // Нажата клавиша "вверх" 

        // проверка на границу карты
        if (transform.position.y + (1 - mapGenerator.yTileOffset) != mapGenerator.mapYsize)
        {
            target = mapGenerator.tiles[mapGenerator.map[(int)(transform.position.x), (int)((transform.position.y + (1 - mapGenerator.yTileOffset)) / (1 - mapGenerator.yTileOffset))]];

            // если по цели можно пройти
            if (target.isMove)
            {
                targetPosition = new Vector2(transform.position.x, transform.position.y + (1 - mapGenerator.yTileOffset));
                use = false;
                gate = false;
                statusText.text = "Двигаюсь вверх";
            }

            // если цель можно использовать
            else if (target.isUse)
            {
                use = true;
                targetPosition = new Vector2(0, 0);
                statusText.text = "Использую " + target.Name;
            }

            // если в цель можно войти
            else if (target.isGate)
            {
                gate = true;
                targetPosition = new Vector2(0, 0);
                statusText.text = "Захожу в " + target.Name;
            }
        }

        //================================================================================================
    }
    public void MoveDown()
    {
        //================================================================================================
        // Нажата клавиша "вниз" 

        // *смотри функцию MoveUp()
        if (transform.position.y - (1 - mapGenerator.yTileOffset) >= 0.1)
        {
            target = mapGenerator.tiles[mapGenerator.map[(int)(transform.position.x), (int)((transform.position.y - (1 - mapGenerator.yTileOffset)) / (1 - mapGenerator.yTileOffset))]];
            if (target.isMove)
            {
                targetPosition = new Vector2(transform.position.x, transform.position.y - (1 - mapGenerator.yTileOffset));
                use = false;
                gate = false;
                statusText.text = "Двигаюсь вниз";
            }
            else if (target.isUse)
            {
                use = true;
                targetPosition = new Vector2(0, 0);
                statusText.text = "Использую " + target.Name;
            }
            else if (target.isGate)
            {
                gate = true;
                targetPosition = new Vector2(0, 0);
                statusText.text = "Захожу в " + target.Name;
            }
        }
        //================================================================================================
    }
    public void MoveRight()
    {
        //================================================================================================
        // Нажата клавиша "вправо" 

        // *смотри функцию MoveUp()
        if ((int)(transform.position.x + 1) != mapGenerator.mapXsize)
        {
            target = mapGenerator.tiles[mapGenerator.map[(int)(transform.position.x + 1), (int)(transform.position.y / (1 - mapGenerator.yTileOffset))]];
            if (target.isMove)
            {
                targetPosition = new Vector2(transform.position.x + 1, transform.position.y);
                use = false;
                gate = false;
                statusText.text = "Двигаюсь вправо";
            }
            else if (target.isUse)
            {
                use = true;
                targetPosition = new Vector2(0, 0);
                statusText.text = "Использую " + target.Name;
            }
            else if (target.isGate)
            {
                gate = true;
                targetPosition = new Vector2(0, 0);
                statusText.text = "Захожу в " + target.Name;
            }
        }
        //================================================================================================
    }
    public void MoveLeft()
    {
        //================================================================================================
        // Нажата клавиша "влево" 

        // *смотри функцию MoveUp()
        if ((int)(transform.position.x - 1) != 0)
        {
            target = mapGenerator.tiles[mapGenerator.map[(int)(transform.position.x - 1), (int)(transform.position.y / (1 - mapGenerator.yTileOffset))]];
            if (target.isMove)
            {
                targetPosition = new Vector2(transform.position.x - 1, transform.position.y);
                use = false;
                gate = false;
                statusText.text = "Двигаюсь влево";
            }
            else if (target.isUse)
            {
                use = true;
                targetPosition = new Vector2(0, 0);
                statusText.text = "Использую " + target.Name;
            }
            else if (target.isGate)
            {
                gate = true;
                targetPosition = new Vector2(0, 0);
                statusText.text = "Захожу в " + target.Name;
            }
        }
        //================================================================================================
    }
    //================================================================================================
    public void ChangeWorld()
    {
        changeWorld = true;
    }
    
    public void OnTic()
    {
        //================================================================================================
        // Вызываются раз в промежуток времени, заданный в классе 
        // если выбрана цель для пути
        if (targetPosition.x != 0 || targetPosition.y != 0)
        {            
            transform.position = new Vector3(targetPosition.x, targetPosition.y, transform.position.z);
            targetPosition = new Vector2(0,0);
        }

        // если выбрана цель и ее можно использовать
        else if (use)
        {

            UseTile ut = target.GetComponent<UseTile>();
            activUi = Instantiate(ut.panel, Uiparent.transform);

            activUi.transform.localScale = new Vector3(0.7f, 0.7f, 1);

            world.freezTime = true;
            ut.wd = world;
            ut.destroyObject = activUi;

            use = false;
        }

        // если выбрана цель и в нее можно войти
        else if (gate)
        {
            Gate gt = target.GetComponent<Gate>();
            gt.Use();
            gate = false;

        }
        else if (changeWorld)
        {
            world.freezTime = true;
            if ((int)(transform.position.x + 1) != mapGenerator.mapXsize && mapGenerator.tiles[mapGenerator.map[(int)transform.position.x + 1, (int)(transform.position.y / (1 - mapGenerator.yTileOffset))]].isChange)
                marksToChangeWorld[0] = Instantiate(prefabMarkToChangeWorld, new Vector3(transform.position.x + 1, transform.position.y, transform.position.z), Quaternion.identity);
            if ((int)(transform.position.x - 1) != 0 && mapGenerator.tiles[mapGenerator.map[(int)transform.position.x - 1, (int)(transform.position.y / (1-mapGenerator.yTileOffset))]].isChange)
                marksToChangeWorld[1] = Instantiate(prefabMarkToChangeWorld, new Vector3(transform.position.x - 1, transform.position.y, transform.position.z), Quaternion.identity);
            if (transform.position.y + (1 - mapGenerator.yTileOffset) != mapGenerator.mapYsize && mapGenerator.tiles[mapGenerator.map[(int)transform.position.x, (int)((transform.position.y + (1 - mapGenerator.yTileOffset)) / (1-mapGenerator.yTileOffset))]].isChange)
                marksToChangeWorld[2] = Instantiate(prefabMarkToChangeWorld, new Vector3(transform.position.x, transform.position.y + (1 - mapGenerator.yTileOffset), transform.position.z), Quaternion.identity);
            if (transform.position.y - (1 - mapGenerator.yTileOffset) >= 0.1 && mapGenerator.tiles[mapGenerator.map[(int)transform.position.x, (int)((transform.position.y - (1 - mapGenerator.yTileOffset)) / (1-mapGenerator.yTileOffset))]].isChange)
                marksToChangeWorld[3] = Instantiate(prefabMarkToChangeWorld, new Vector3(transform.position.x, transform.position.y - (1 - mapGenerator.yTileOffset), transform.position.z), Quaternion.identity);

            bool f = false;
            for (int i = 0; i < marksToChangeWorld.Length; i++)
            {
                if (marksToChangeWorld[i] != null) f = true;                   
            }
            if (!f)
            {
                world.freezTime = false;
                changeWorld = false;
            }

        }
        statusText.text = "";
    //================================================================================================
    }
    float Convert(float y)
    {
        float y1 = 0;
        while (y1 < y)
            y1 += (1 - mapGenerator.yTileOffset);
        float y2 = y1 - (1 - mapGenerator.yTileOffset);
        if (y1 - y > y - y2) return y2;
        else return y1;
    }
    void Update()
    {
        //================================================================================================
        // Двигаем камеру
        Vector3 targetPosition = new Vector3(transform.position.x, transform.position.y, mainCamera.transform.position.z);
        mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, targetPosition, cameraSpeed * Time.deltaTime);
        //================================================================================================

        if (changeWorld)
        {
            if (Input.GetMouseButtonDown(0))
            {
                int xMousPos = (int)Math.Round(Camera.main.ScreenToWorldPoint(Input.mousePosition).x);
                float yMousPosConvert = Convert(Camera.main.ScreenToWorldPoint(Input.mousePosition).y);
                int yMousPos = (int)Math.Round(yMousPosConvert / (1 - mapGenerator.yTileOffset));
                GameObject useTile = null;

                for (int i = 0; i < marksToChangeWorld.Length; i++)
                {
                    if (marksToChangeWorld[i] != null && xMousPos == (int)marksToChangeWorld[i].transform.position.x && yMousPos == (int)Math.Round(marksToChangeWorld[i].transform.position.y / (1 - mapGenerator.yTileOffset)))
                    {
                        useTile = mapGenerator.mapRender[(int)marksToChangeWorld[i].transform.position.x, (int)(marksToChangeWorld[i].transform.position.y / (1 - mapGenerator.yTileOffset))] ;
                        break;
                    }
                }

                changeWorld = false;

                for (int i = 0; i < marksToChangeWorld.Length; i++)
                {
                    if (marksToChangeWorld[i] != null)
                    {
                        Destroy(marksToChangeWorld[i].gameObject);
                        marksToChangeWorld[i] = null;
                    }
                }

                if (useTile != null)
                {
                    ChangeTile ct = useTile.GetComponent<ChangeTile>();;

                    activUi = Instantiate(ct.scrollView, Uiparent.transform);
                    activUi.transform.localScale = new Vector3(0.3f, 0.3f, 1);
                    ct.wd = world;
                    ct.generator = mapGenerator;
                    ct.map = mapGenerator.map;
                    ct.destroyObject = activUi;
                    ct.x = (int)ct.transform.position.x;
                    ct.y = (int)Math.Round(ct.transform.position.y / (1 - mapGenerator.yTileOffset));
                }
                else
                {
                    world.freezTime = false;
                }
            }
        }
    }
}
