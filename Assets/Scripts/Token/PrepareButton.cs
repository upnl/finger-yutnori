using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PrepareButton : MonoBehaviour
{
    public Token thisToken;
    public int steps;

    public void OnClickPrepareButton()
    {
        TokenManager tokenManager = GameObject.Find("TokenManager").GetComponent<TokenManager>();

        tokenManager.OnClickPrepareButton(this, thisToken, steps);
    }
}
