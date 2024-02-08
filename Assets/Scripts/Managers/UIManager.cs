using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UIManager : MonoBehaviour
{
    public GameObject targetObject_Canvas;
    public GameObject targetObject_Blocker;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Set the position of Blocker to match that of Canvas.
    /// </summary>
    public void OnClickDoneButton()
    {
        targetObject_Blocker.transform.position = targetObject_Canvas.transform.position;
    }

    /// <summary>
    /// Set the position of Blocker to (2000,0,0).
    /// </summary>
    public void OnClickBlackButton()
    {
        targetObject_Blocker.transform.position = new Vector3(2000, 0, 0);
    }
}