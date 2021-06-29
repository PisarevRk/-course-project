using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardController : MonoBehaviour
{
    public Player player;

    public KeyCode moveUp;
    public KeyCode moveDown;
    public KeyCode moveRight;
    public KeyCode moveLeft;
    public KeyCode Change;


    void Update()
    {
        if (Input.GetKey(moveUp))
        {
            player.MoveUp();
        }
        if (Input.GetKey(moveDown))
        {
            player.MoveDown();
        }
        if (Input.GetKey(moveRight))
        {
            player.MoveRight();
        }
        if (Input.GetKey(moveLeft))
        {
            player.MoveLeft();
        }
        if (Input.GetKey(Change))
        {
            player.ChangeWorld();
        }
    }
}
