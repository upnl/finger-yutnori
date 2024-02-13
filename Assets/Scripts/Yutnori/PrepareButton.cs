using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrepareButton : MonoBehaviour
{
    public PrepareButtonManager prepareButtonManager;

    public Token thisToken;

    public void ClickPrepareButton()
    {
        prepareButtonManager.ClickPrepareButton(thisToken);
    }
}
