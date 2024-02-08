using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UIManager : MonoBehaviour
{
    public GameObject TargetPlayer0Done;
    public GameObject TargetPlayer1Done;
    public GameObject TargetCanvas;

    int curPlayer = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// if curPlayer is 0, set "Player0Done" to match Canvas and change curPlayer to 1.
    /// if curPlayer is 1, set "Player1DOne" to match Canvas and change curPlayer to 0.
    /// </summary>
    public void OnClickDoneButton()
    {
        if (curPlayer == 0)
        {
            TargetPlayer0Done.transform.position = TargetCanvas.transform.position;
        }
        else
        {
            TargetPlayer1Done.transform.position = TargetCanvas.transform.position;
        }
        curPlayer = (curPlayer == 0) ? 1 : 0;
    }

    /// <summary>
    /// set the position of "Player0Done" to (2000,0,0)
    /// </summary>
    public void OnClickPlayer0DoneButton()
    {
        TargetPlayer0Done.transform.position = new Vector3(2000, 0, 0);
    }

    /// <summary>
    /// set the position of "Player1Done" to (2000,0,0)
    /// </summary>
    public void OnClickPlayer1DOneButton()
    {
        TargetPlayer1Done.transform.position = new Vector3(2000, 0, 0);
    }
}