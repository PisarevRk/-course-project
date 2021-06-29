using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class City : MonoBehaviour
{
    public Dropdown dropdown;
    public InputField inputField;
    public Player player;

    public void Buy()
    {
        int count;
        if (int.TryParse(inputField.text, out count))
        {
            if (dropdown.value == 0)
            {

            }
            else if (dropdown.value == 1)
            {

            }
        }
    }
}
