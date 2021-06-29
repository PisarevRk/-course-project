using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public delegate void emptyEventHolder();
    public delegate void objectEventHolder(object sender);

    public static event objectEventHolder TileSelected;
    public static event objectEventHolder FieldGenerated;
    public static event objectEventHolder MoveUpdated;
    public static event objectEventHolder NewFigureSelected;

    public static event emptyEventHolder SceneReady;
    public static event emptyEventHolder BattleReady;
    public static event emptyEventHolder NewTurn;
    public static event emptyEventHolder MoveEnded;

    public static void InvokeTileSelected(object sender)
    {
        TileSelected?.Invoke(sender);
    }

    public static void InvokeFieldGenerated(object sender)
    {
        FieldGenerated?.Invoke(sender);
    }

    public static void InvokeMoveUpdated(object selected)
    {
        MoveUpdated?.Invoke(selected);
    }

    public static void InvokeNewFigureSelected(object selected)
    {
        NewFigureSelected?.Invoke(selected);
    }



    public static void InvokeSceneReady()
    {
        SceneReady?.Invoke();
    }

    public static void InvokeBattleReady()
    {
        BattleReady?.Invoke();
    }

    public static void InvokeNewTurn()
    {
        NewTurn?.Invoke();
    }

    public static void InvokeMoveEnded()
    {
        MoveEnded?.Invoke();
    }
}
