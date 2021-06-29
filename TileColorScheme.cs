using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class TileColorScheme : ScriptableObject
{
    public Color borderDefaultColor, borderMouseOverColor, borderClickColor;

    public Color bodyDefaultColor, bodyHighliteColor, bodySelectedColor;
}
