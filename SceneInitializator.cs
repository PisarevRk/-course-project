using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SceneInitializator : MonoBehaviour
{

    [SerializeField]
    GameObject figurePrefab;

    [SerializeField]
    List<FigureInfo> figureInfos;

    [SerializeField]
    Transform figureCreationPoint;

    List<FigureController> controls;

    [SerializeField]
    List<Sprite> groundSprites;

    [SerializeField]
    int allFiguresAmount;

    [SerializeField]
    Sprite groundSprite;

    [SerializeField]
    Tilemap tilemap;

    [SerializeField]
    Vector3Int position;

    [SerializeField]
    int x1, y1, x2, y2;


    private void Start()
    {
        EventManager.FieldGenerated += OnFieldGenerated;
        FillBackground();

        EventManager.InvokeSceneReady();
    }

    private void OnDisable()
    {
        EventManager.FieldGenerated -= OnFieldGenerated;
    }



    private List<GameObject> GenerateTestFigures()
    {
        List<GameObject> figures = new List<GameObject>();

        foreach (FigureInfo unit in figureInfos)
            figures.Add(CreateFigure(unit));

        return figures;
    }

    private GameObject CreateFigure(FigureInfo figure)
    {
        GameObject obj = Instantiate(figurePrefab, figureCreationPoint);
        FigureController control = obj.GetComponent<FigureController>();
        control.figureInfo = figure;
        control.figureInfo.SetHealth(allFiguresAmount);
        return obj;
    }



    private void OnFieldGenerated(object sender)
    {
        FieldInfo fieldInfo = (FieldInfo)sender;
        List<GameObject> figures = GenerateTestFigures();

        controls = new List<FigureController>();

        foreach (GameObject figure in figures)
        {
            controls.Add(figure.GetComponent<FigureController>());
        }

        BattleController.figures = controls;
        BattleController.SelectFirst();

        PutFigures(fieldInfo);
        EventManager.InvokeBattleReady();
        Destroy(this);
    }

    private void PutFigures(FieldInfo fieldInfo)
    {
        int playerCount = 0, nonPlayerCount = 0;
        int row, col;

        for(int i = 0; i < controls.Count; i++)
        {
            if(controls[i].figureInfo.Player)
            {
                row = fieldInfo.RowCount / 2 - playerCount + 2;
                col = 0;
                playerCount++;
            }
            else
            {
                row = fieldInfo.RowCount / 2 - nonPlayerCount + 2;
                col = fieldInfo.ColCount - 1 - ((fieldInfo.EvenAreSmaller == (row % 2 == 0)) ? 1:0);
                nonPlayerCount++;
            }

            controls[i].MoveToTile(fieldInfo.Tiles[row, col]);
        }
    }

    private void FillBackground()
    {
        Tile tile = ScriptableObject.CreateInstance<Tile>();
        tile.sprite = groundSprite;
        tilemap.size = new Vector3Int(20, 20, 0);
        tilemap.ResizeBounds();
        tilemap.BoxFill(position, tile, x1, y1, x2, y2);
        Destroy(tile);
    }
}
