using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PrepareButton : MonoBehaviour
{
    public TokenManager tokenManager;

    public Token thisToken;
    public int steps;

    public void OnClickPrepareButton()
    {
        tokenManager.OnClickPrepareButton(this, thisToken, steps);
    }
}
