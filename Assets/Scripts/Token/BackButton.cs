using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackButton : MonoBehaviour
{
    public Token token;
    public BoardPointIndex boardPointIndex;

    public void OnClick()
    {
        TokenManager tokenManager = GameObject.Find("TokenManager").GetComponent<TokenManager>();

        tokenManager.OnClickBackButton(token, boardPointIndex);
    }
}
