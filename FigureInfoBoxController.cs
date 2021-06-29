using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FigureInfoBoxController : MonoBehaviour
{
    [SerializeField]
    GameObject figureInfoBoxPrefab;
    [SerializeField]
    TMPro.TextMeshProUGUI textBox;
    [SerializeField]
    SpriteRenderer spriteRenderer;
    [SerializeField]
    float screenToSceneRatio;
    [SerializeField]
    Vector3 boxHalf;

    GameObject figureInfoBox;

    public void Show(FigureController figure, int quarter)
    {
        Vector3 position = Input.mousePosition;
        switch(quarter)
        {
            case 1:
                position = PixelPosToNormalPos(position) + new Vector3(-boxHalf.x, -boxHalf.y, 0);
                break;
            case 2:
                position = PixelPosToNormalPos(position) + new Vector3(boxHalf.x, -boxHalf.y, 0);
                break;
            case 3:
                position = PixelPosToNormalPos(position) + new Vector3(boxHalf.x, boxHalf.y, 0);
                break;
            case 4:
                position = PixelPosToNormalPos(position) + new Vector3(-boxHalf.x, boxHalf.y, 0);
                break;
        }

        textBox.text = figure.figureInfo.GetTextRepresentation();
        spriteRenderer.sprite = figure.SpriteRenderer.sprite;
        figureInfoBox = Instantiate(figureInfoBoxPrefab, position, new Quaternion());
    }

    public void Hide()
    {
        if(figureInfoBox != null)
            Destroy(figureInfoBox);
    }

    private Vector3 PixelPosToNormalPos(Vector3 pixelPos)
    {
        return (pixelPos - new Vector3(Screen.currentResolution.width, Screen.currentResolution.height, 0) / 2) * screenToSceneRatio;
    }
}
