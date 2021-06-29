using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class FieldGenerator : MonoBehaviour
{
    // На данный момент не реализована проверка, связаны ли противоположные концы поля
    // Из-за этого препятствий генерируется не больше, чем количество строк - 1

    // Также можно добавить возможность обрезать углы по радиусу

    [SerializeField]
    GameObject tilePrefab;                      // Префаб, из которого состоит поле

    public GameObject parent;

    [SerializeField]
    int rowCount,                               // Количество строк в поле
        colCount;                               // Количество колонок в поле

    [SerializeField]
    bool evenAreSmaller = false,                // Четные строки короче нечетных на одну клетку
         cutCorners;                            // Обрезать ли углы у поля

    [SerializeField]
    float dx,                                   // Расстояние по x между центрами клеток
          dy;                                   // Расстояние по y между центрами клеток

    [SerializeField]
    int obstacleAmount,                         // Количество препятствий
        maxTriesToPutObstacles;                 // Максимальное количество попыток, чтобы поставить препятствие, на всякий случай
                                                // Чтобы при большом количестве препятствий генерация не занимала слишком много времени

    [SerializeField]
    Transform tilingStartPivot;                 // Точка, начиная от которой начинается заполнение клетками

    bool[,] tiles;                              // Ссылки на клетки

    public FieldInfo GenerateField()
    {
        //FieldPathfinder fieldPathfinder = GetComponent<FieldPathfinder>();

        // Здесь определяются, какие клетки будут содрежать препятствия
        // Также тут должна проводится проверка, чтобы противоположные концы поля не были отрезаны друг от друга
        SetObstaclePositions();

        // Генерация поля
        return InstantiateField();

        //fieldPathfinder.Initialize(tiles, evenAreSmaller);
        //CheckIfPathExists(fieldPathfinder);
    }

    private void SetObstaclePositions()
    {
        // Инициализирует двумерный массив tiles + присваивает полю IsEmpty каждого элемента значение true
        SetAllTilesAsNotEmpty();

        // Генерирует препятствия на поле
        GenerateObstaclePositions();

        // Если количество препятсвий больше количества строк, надо проверить, соединены ли противоположные концы поля
        // !!!НЕ СДЕЛАНО!!!
    }

    private void SetAllTilesAsNotEmpty()
    {
        tiles = new bool[rowCount, colCount];

        for(int row = 0; row < rowCount; row++)
        {
            for(int col = 0; col < colCount;col++)
            {
                tiles[row, col] = false;
            }
        }
    }

    private void GenerateObstaclePositions()
    {
        if (cutCorners) 
            CutCorners();

        // Этот цикл обрезает строки с меньшим количеством клеток
        for (int row = evenAreSmaller ? 0 : 1; row < rowCount; row += 2)
            tiles[row, colCount - 1] = true;

        int tries = -obstacleAmount;

        // Находит позицию для пустых клеток при помощи UnityEngine.Random
        for(int i = 0; i < obstacleAmount && tries < maxTriesToPutObstacles; i++, tries++)
        {
            // UnityEngine.Random.Range возвращает значение от min до max включительно
            int row = UnityEngine.Random.Range(0, rowCount);

            // Чтобы ни одна крайняя клетка слева не могла быть пустой, min = 1
            // Чтобы ни одна крайняя клетка справа не могла быть пустой, max = colCount - 2
            // Поскольку некоторые строки в зависимости от evenAreSmaller меньше, нужно для них в max добавлять 1
            int col = UnityEngine.Random.Range(1, colCount - 2 + (row%2==0 && evenAreSmaller?0:1));

            // Если клетка уже содержит препятсвие, найти другую
            if (!tiles[row, col])
                tiles[row, col] = true;
            else
                i--;
        }
    }

    private void CutCorners()
    {
        tiles[0, 0] = true;
        tiles[rowCount - 1, 0] = true;

        if(evenAreSmaller)
        {
            tiles[rowCount - 1, colCount - 2] = true;
            tiles[0, colCount - 2] = true;
        }
        else
        {
            tiles[rowCount - 1, colCount - 1] = true;
            tiles[0, colCount - 1] = true;
        }
    }

    private FieldInfo InstantiateField()
    {
        Vector3 startPos = tilingStartPivot.position;
        Quaternion rotation = tilingStartPivot.rotation;

        TileController[,] controls = new TileController[rowCount, colCount];

        for(int row = 0; row < rowCount; row++)
        {
            float k = ((row % 2 == 0) == evenAreSmaller) ? 0.5f : 0;

            for (int col = 0; col < colCount; col++)
            {
                Vector3 position = new Vector3(startPos.x + dx*(col+k), startPos.y - row*dy);

                if(!tiles[row,col])
                {
                    GameObject tile = Instantiate(tilePrefab, position, rotation, parent.transform);
                    controls[row, col] = tile.GetComponent<TileController>();
                    controls[row, col].SetCoords(col, row);
                }
            }
        }

        return new FieldInfo(controls, evenAreSmaller);
    }
}