using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class MoveButton : MonoBehaviour
{
    public TokenManager tokenManager;

    public Token thisToken;
    public int steps;

    public void OnClickMoveButton()
    {
        tokenManager.OnClickMoveButton(thisToken, steps);
    }
}
