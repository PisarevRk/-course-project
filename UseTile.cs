using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UseTile : MonoBehaviour
{
    public GameObject panel;
    public World wd;
    public GameObject destroyObject;

    public void Unfreez()
    {
        wd.freezTime = false;
        Destroy(destroyObject.gameObject);
    }
}
