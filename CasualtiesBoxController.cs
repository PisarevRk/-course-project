using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CasualtiesBoxController : MonoBehaviour
{
    [SerializeField]
    List<Transform> positions;

    [SerializeField]
    GameObject figurePrefab;

    public void ShowAt(FigureInfo figureInfo, int pos)
    {
        GameObject res = Instantiate(figurePrefab, positions[pos]);
        //res.GetComponent<FigureController>().SetFigureInfo(figureInfo);
    }
}
