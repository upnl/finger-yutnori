using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveButton : MonoBehaviour
{
    public PrepareButtonManager prepareButtonManager;

    public Token thisToken; // token to move
    public int steps; // how to move

    public void ClickMoveButton()
    {
        prepareButtonManager.ClickMoveButton(thisToken, steps);
    }
}
