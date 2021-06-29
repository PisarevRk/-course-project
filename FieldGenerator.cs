using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class FieldGenerator : MonoBehaviour
{
    // �� ������ ������ �� ����������� ��������, ������� �� ��������������� ����� ����
    // ��-�� ����� ����������� ������������ �� ������, ��� ���������� ����� - 1

    // ����� ����� �������� ����������� �������� ���� �� �������

    [SerializeField]
    GameObject tilePrefab;                      // ������, �� �������� ������� ����

    public GameObject parent;

    [SerializeField]
    int rowCount,                               // ���������� ����� � ����
        colCount;                               // ���������� ������� � ����

    [SerializeField]
    bool evenAreSmaller = false,                // ������ ������ ������ �������� �� ���� ������
         cutCorners;                            // �������� �� ���� � ����

    [SerializeField]
    float dx,                                   // ���������� �� x ����� �������� ������
          dy;                                   // ���������� �� y ����� �������� ������

    [SerializeField]
    int obstacleAmount,                         // ���������� �����������
        maxTriesToPutObstacles;                 // ������������ ���������� �������, ����� ��������� �����������, �� ������ ������
                                                // ����� ��� ������� ���������� ����������� ��������� �� �������� ������� ����� �������

    [SerializeField]
    Transform tilingStartPivot;                 // �����, ������� �� ������� ���������� ���������� ��������

    bool[,] tiles;                              // ������ �� ������

    public FieldInfo GenerateField()
    {
        //FieldPathfinder fieldPathfinder = GetComponent<FieldPathfinder>();

        // ����� ������������, ����� ������ ����� ��������� �����������
        // ����� ��� ������ ���������� ��������, ����� ��������������� ����� ���� �� ���� �������� ���� �� �����
        SetObstaclePositions();

        // ��������� ����
        return InstantiateField();

        //fieldPathfinder.Initialize(tiles, evenAreSmaller);
        //CheckIfPathExists(fieldPathfinder);
    }

    private void SetObstaclePositions()
    {
        // �������������� ��������� ������ tiles + ����������� ���� IsEmpty ������� �������� �������� true
        SetAllTilesAsNotEmpty();

        // ���������� ����������� �� ����
        GenerateObstaclePositions();

        // ���� ���������� ���������� ������ ���������� �����, ���� ���������, ��������� �� ��������������� ����� ����
        // !!!�� �������!!!
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

        // ���� ���� �������� ������ � ������� ����������� ������
        for (int row = evenAreSmaller ? 0 : 1; row < rowCount; row += 2)
            tiles[row, colCount - 1] = true;

        int tries = -obstacleAmount;

        // ������� ������� ��� ������ ������ ��� ������ UnityEngine.Random
        for(int i = 0; i < obstacleAmount && tries < maxTriesToPutObstacles; i++, tries++)
        {
            // UnityEngine.Random.Range ���������� �������� �� min �� max ������������
            int row = UnityEngine.Random.Range(0, rowCount);

            // ����� �� ���� ������� ������ ����� �� ����� ���� ������, min = 1
            // ����� �� ���� ������� ������ ������ �� ����� ���� ������, max = colCount - 2
            // ��������� ��������� ������ � ����������� �� evenAreSmaller ������, ����� ��� ��� � max ��������� 1
            int col = UnityEngine.Random.Range(1, colCount - 2 + (row%2==0 && evenAreSmaller?0:1));

            // ���� ������ ��� �������� ����������, ����� ������
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