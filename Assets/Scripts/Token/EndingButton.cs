using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndingButton : MonoBehaviour
{
    TokenManager tokenManager;

    private void Awake()
    {
        tokenManager = GameObject.Find("TokenManager").GetComponent<TokenManager>();
    }

    public void OnClickRestartButton()
    {
        tokenManager.OnClickRestartButton();
    }

    public void OnClickNewGameButton()
    {
        tokenManager.OnClickNewGameButton();
    }
}
