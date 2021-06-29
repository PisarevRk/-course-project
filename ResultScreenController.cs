using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResultScreenController : MonoBehaviour
{
    [SerializeField]
    CasualtiesBoxController playerBoxController, enemyBoxController;

    [SerializeField]
    GameObject resultScreenPrefab;

    public void Show(List<FigureInfo> figures)
    {
        Instantiate(resultScreenPrefab);

        int playerCounter = 0, enemyCounter = 0;

        foreach(FigureInfo figure in figures)
        {
            if (figure.Player)
                playerBoxController.ShowAt(figure, playerCounter++);
            else
                enemyBoxController.ShowAt(figure, enemyCounter++);
        }
    }
}
