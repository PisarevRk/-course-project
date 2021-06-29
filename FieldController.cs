using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldController : MonoBehaviour
{
    [SerializeField]
    FieldGenerator fieldGenerator;

    FieldInfo fieldInfo;

    private void Start()
    {
        EventManager.SceneReady += OnSceneReady;
        EventManager.MoveUpdated += OnMoveUpdated;
    }

    private void OnDisable()
    {
        EventManager.SceneReady -= OnSceneReady;
    }



    private void OnSceneReady()
    {
        fieldInfo = fieldGenerator.GenerateField();

        FieldPathfinder.Initialize(fieldInfo.Tiles, fieldInfo.EvenAreSmaller);

        EventManager.InvokeFieldGenerated(fieldInfo);
    }



    private void OnMoveUpdated(object selected)
    {
        ClearAllHighlites();

        FigureController control = (FigureController)selected;
        HighliteAroundFigure(control);

        if (control.figureInfo.UnitInfo.ranged)
            HighliteEnemies(control);
    }

    private void HighliteEnemies(FigureController control)
    {
        foreach(FigureController figure in BattleController.figures)
        {
            if(figure.figureInfo.Player != control.figureInfo.Player)
            {
                figure.Tile.Highlite();
            }
        }
    }

    private void HighliteAroundFigure(FigureController control)
    {
        TileController tile = control.Tile;

        tile.Select();
        foreach (Pair pair in FieldPathfinder.GetTilesInRadius(tile.X, tile.Y, control.figureInfo.movePointsRemaining))
        {
            if (fieldInfo.Tiles[pair.x, pair.y] != null)
                fieldInfo.Tiles[pair.x, pair.y].Highlite();
        }
    }

    private void ClearAllHighlites()
    {
        foreach (TileController tile in fieldInfo.Tiles)
            if (tile != null)
                tile.ClearColor();
    }
}

public class FieldInfo
{
    TileController[,] tiles;
    bool evenAreSmaller;

    public TileController[,] Tiles
    {
        get { return tiles; }
    }

    public int RowCount
    {
        get { return tiles.GetUpperBound(0) + 1; }
    }

    public int ColCount
    {
        get
        {
            int rows = tiles.GetUpperBound(0) + 1;
            return tiles.Length / rows;
        }
    }

    public bool EvenAreSmaller
    {
        get { return evenAreSmaller; }
    }

    public FieldInfo(TileController[,] tiles, bool evenAreSmaller)
    {
        this.tiles = tiles;
        this.evenAreSmaller = evenAreSmaller;
    }
}