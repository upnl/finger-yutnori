using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrepareButton : MonoBehaviour
{
    public TokenManager tokenManager;
    public Token thisToken;

    public void ClickPrepareButton()
    {
        tokenManager.ClickPrepareButton(thisToken);
    }
}
