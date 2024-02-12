using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveButton : MonoBehaviour
{
    public TokenManager tokenManager;

    public int steps;

    public void ClickMoveButton()
    {
        tokenManager.ClickMoveButton(steps);
    }
}
