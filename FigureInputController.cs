using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FigureInputController : MonoBehaviour
{
    [SerializeField]
    float leftToRightRatio, topToBottomRatio;

    [SerializeField]
    FigureInfoBoxController infoBoxController;

    [SerializeField]
    FigureController figure;

    bool mouseOver;

    void Start()
    {
        
    }

    void Update()
    {
        if(Input.GetMouseButtonDown(1) && mouseOver)
            ShowInfoBox();
        else if(Input.GetMouseButtonUp(1))
            HideInfoBox();

        if (Input.GetMouseButtonDown(0) && mouseOver)
            EventManager.InvokeTileSelected(figure.Tile);
    }

    private void OnMouseExit()
    {
        HideInfoBox();
        mouseOver = false;
    }

    private void OnMouseEnter()
    {
        mouseOver = true;
    }

    public void ShowInfoBox()
    {
        Vector3 center = new Vector3(Screen.width * leftToRightRatio, Screen.height * (1 - topToBottomRatio), 0);

        Vector3 mousePos = Input.mousePosition;

        if (mousePos.x > center.x && mousePos.y > center.y)
            infoBoxController.Show(figure, 1);
        else if (mousePos.x < center.x && mousePos.y > center.y)
            infoBoxController.Show(figure, 2);
        else if (mousePos.x > center.x && mousePos.y < center.y)
            infoBoxController.Show(figure, 3);
        else if (mousePos.x < center.x && mousePos.y < center.y)
            infoBoxController.Show(figure, 4);
    }

    public void HideInfoBox()
    {
        infoBoxController.Hide();
    }
}
