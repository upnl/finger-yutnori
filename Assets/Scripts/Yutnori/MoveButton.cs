using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveButton : MonoBehaviour
{
    public PrepareButtonManager prepareButtonManager;

    public Token thisToken;
    public int steps;

    public void ClickMoveButton()
    {
        prepareButtonManager.ClickMoveButton(thisToken, steps);
    }
}
