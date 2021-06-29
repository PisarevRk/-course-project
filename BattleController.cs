using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;
using System.IO;
public class BattleController : MonoBehaviour
{
    [SerializeField]
    ResultScreenController resultScreen;

    public static bool win = false;
    public GameObject panel;
    public Text winText;
    public GameObject gridFolder;
    public GameObject unitFolder;

    public static List<FigureController> figures;
    static List<FigureController> queue;
    public static List<FigureController> gotDamaged = new List<FigureController>();

    public static FigureController Selected
    {
        get { return queue[0]; }
    }

    public static int MovePointsRemaining
    {
        get { return Selected.figureInfo.movePointsRemaining; }
        set { Selected.figureInfo.movePointsRemaining = value; }
    }

    public static bool PlayerIsMoving
    {
        get { return Selected.figureInfo.Player; }
    }



    private void Start()
    {
        string Filepath = @"C:\Users\Poma\Desktop\Программы\Unity\Course Project v.3.1\Position.txt";
        File.WriteAllText(Filepath, "");

        panel.gameObject.SetActive(false);
        gridFolder.gameObject.SetActive(true);
        unitFolder.gameObject.SetActive(true);
        EventManager.BattleReady += OnBattleReady;
    }

    private void OnDisable()
    {
        EventManager.TileSelected -= OnTileSelected;
        EventManager.BattleReady -= OnBattleReady;
    }

    private void Update()
    {
        if (AI.CountPlayerFigures(figures) == 0)
        {
            panel.gameObject.SetActive(true);
            gridFolder.gameObject.SetActive(false);
            unitFolder.gameObject.SetActive(false);

            winText.text = "Game Over!";
            print("Game Over!");

            
        }
        else if (AI.CountEnemyFigures(figures) == 0)
        {
            string Filepath = @"C:\Users\Poma\Desktop\Программы\Unity\Course Project v.3.1\Position.txt";
            File.WriteAllText(Filepath, "1");

            win = true;

            panel.gameObject.SetActive(true);
            gridFolder.gameObject.SetActive(false);
            unitFolder.gameObject.SetActive(false);
            winText.text = "You Win!";
            print("You Win!");

        }
    }

    public void EndButton()
    {
        if (win)
            SceneManager.LoadScene("Main Scene");
        else
        {
            //UnityEditor.EditorApplication.isPlaying = false;
            Application.Quit();
        }
    }

    public static void RemoveFigure(FigureController figure)
    {
        figure.Tile.Figure = null;
        queue.RemoveAll((x) => x.Equals(figure));

        figures.Remove(figure);

        Destroy(figure.gameObject);


    }



    private static void OnBattleReady()
    {
        EventManager.TileSelected += OnTileSelected;
        EventManager.MoveEnded += OnMoveEnded;
        SelectFirst();
        ApplyPathfinderMask();
        EventManager.InvokeMoveUpdated(Selected);
    }

    private static void OnTileSelected(object sender)
    {
        if(PlayerIsMoving)
            Selected.MakeMove((TileController)sender);
    }

    private static void OnMoveEnded()
    {
        if (AI.CountPlayerFigures(figures) == 0)
        {
            print("Game Over!");
            Application.Quit();
        }
        else if (AI.CountEnemyFigures(figures) == 0)
        {
            SceneManager.LoadScene("Main Scene");
        }
        else
        {
            if (MovePointsRemaining == 0)
                SelectNext();

            ApplyPathfinderMask();
            EventManager.InvokeMoveUpdated(Selected);
        }
        
    }

    public static void OnWaitButtonClick()
    {
        if (PlayerIsMoving)
        {
            if (Selected.figureInfo.waited)
                return;

            Selected.figureInfo.waited = true;
            queue.Insert(figures.Count - figures.IndexOf(Selected), Selected);

            SelectNext();

            ApplyPathfinderMask();
            EventManager.InvokeMoveUpdated(Selected);
        }
    }

    public static void OnSkipButtonClick()
    {
        if (PlayerIsMoving)
        {
            SelectNext();

            ApplyPathfinderMask();
            EventManager.InvokeMoveUpdated(Selected);
        }
    }




    private static void SelectNext()
    {
        if(queue.Count == 1)
            SelectFirst();
        else
            queue.RemoveAt(0);

        foreach (FigureController figure in gotDamaged)
            if (figure.figureInfo.UnitCount < 1)
                RemoveFigure(figure);
        gotDamaged.Clear();

        if (!Selected.figureInfo.Player)
            AIMove();
    }

    public static void SelectFirst()
    {
        queue = figures.GetRange(0, figures.Count);

        foreach (FigureController figure in figures)
        {
            figure.figureInfo.ResetMovePoints();
            figure.figureInfo.canAttack = true;
            figure.figureInfo.waited = false;
        }

        EventManager.InvokeNewTurn();
    }

    private static void ApplyPathfinderMask()
    {
        List<FieldPathfinder.Mask> pairs = new List<FieldPathfinder.Mask>();
        foreach (FigureController control in figures)
            pairs.Add(new FieldPathfinder.Mask(control.Tile.X, control.Tile.Y, true, control.figureInfo.Player != Selected.figureInfo.Player));
        FieldPathfinder.ApplyNewMask(pairs);
    }

    private static void AIMove()
    {
        ApplyPathfinderMask();
        EventManager.InvokeMoveUpdated(Selected);

        if (AI.CountPlayerFigures(figures) == 0)
        {

        }
        else if (AI.CountEnemyFigures(figures) == 0)
        {


        }
        else
        {
            FieldPathfinder.Path path = AI.FindClosestEnemyPath(Selected, AI.GetPlayerFigures(figures));

            if (path.Length - 1 > MovePointsRemaining)
            {
                if (Selected.figureInfo.UnitInfo.ranged)
                    Selected.MakeMove(path.Nodes[0].Tile);
                else
                    Selected.MakeMove(path.GetSubpath(path.Length - MovePointsRemaining - 1).Nodes[0].Tile);
            }
            else
                Selected.MakeMove(path.Nodes[0].Tile);
        }
        
    }

}

public static class AI
{
    public static List<FigureController> GetPlayerFigures(List<FigureController> figures)
    {
        List<FigureController> playerFigures = new List<FigureController>();
        foreach (FigureController figure in figures)
            if (figure.figureInfo.Player)
                playerFigures.Add(figure);
        return playerFigures;
    }

    public static int CountPlayerFigures(List<FigureController> figures)
    {
        int i = 0;
        foreach (FigureController figure in figures)
            if (figure.figureInfo.Player)
                i++;
        return i;
    }
    public static int CountEnemyFigures(List<FigureController> figures)
    {
        int i = 0;
        foreach (FigureController figure in figures)
            if (!figure.figureInfo.Player)
                i++;
        return i;
    }

    public static FieldPathfinder.Path FindClosestEnemyPath(FigureController figure, List<FigureController> enemies)
    {
        FieldPathfinder.Path shortestPath = null;
        int shortestPathLength = int.MaxValue;

        foreach (FigureController enemy in enemies)
        {
            FieldPathfinder.Path path = FieldPathfinder.FindPath(figure.Tile.X, figure.Tile.Y, enemy.Tile.X, enemy.Tile.Y);

            if (shortestPathLength > path.Length)
            {
                shortestPath = path;
                shortestPathLength = path.Length;
            }
        }

        return shortestPath;
    }
}