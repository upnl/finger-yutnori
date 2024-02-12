using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrepareButton : MonoBehaviour
{
    public TokenManager tokenManager;
    public PrepareManager prepareManager;
    public int steps;

    public void ClickPrepareButton()
    {
        prepareManager.DeleteButtonInCanvas();
        StartCoroutine(tokenManager.DebugHandleInput(steps));
    }
}
