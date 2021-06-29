using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileController : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer border, body;

    FigureController figure;
    bool mouseOver;

    public TileColorScheme colorScheme;

    int x;
    int y;

    public int X
    {
        get { return x; }
    }

    public int Y
    {
        get { return y; }
    }

    public FigureController Figure
    {
        get { return figure; }
        set { figure = value; }
    }


    private void Start()
    {
        border.color = colorScheme.borderDefaultColor;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && mouseOver)
            border.color = colorScheme.borderClickColor;
        else if (Input.GetMouseButtonUp(0))
        {
            if (mouseOver)
            {
                border.color = colorScheme.borderMouseOverColor;
                EventManager.InvokeTileSelected(this);
            }
            else
                border.color = colorScheme.borderDefaultColor;
        }

        if (Input.GetMouseButtonDown(1) && mouseOver)
            if (figure)
                figure.InputController.ShowInfoBox();

        if (Input.GetMouseButtonUp(1))
            if (figure)
                figure.InputController.HideInfoBox();
    }

    private void OnMouseEnter()
    {
        border.color = colorScheme.borderMouseOverColor;
        mouseOver = true;
    }

    private void OnMouseExit()
    {
        border.color = colorScheme.borderDefaultColor;
        mouseOver = false;

        if(figure)
            figure.InputController.HideInfoBox();
    }



    public void SetCoords(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public void Highlite()
    {
        body.color = colorScheme.bodyHighliteColor;
    }

    public void Select()
    {
        body.color = colorScheme.bodySelectedColor;
    }

    public void ClearColor()
    {
        body.color = colorScheme.bodyDefaultColor;
    }

    public void SetFigure(FigureController figure)
    {
        if (figure is null)
        {
            this.figure = null;
            return;
        }

        if(figure != null)
            this.figure.Tile = null;
        this.figure = figure;
        this.figure.Tile = this;
    }
}