using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class MoveButton : MonoBehaviour
{
    public Token thisToken;
    public int steps;

    public void OnClickMoveButton()
    {
        TokenManager tokenManager = GameObject.Find("TokenManager").GetComponent<TokenManager>();

        tokenManager.OnClickMoveButton(thisToken, steps);
    }
}
