using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public FingerToggleGroup fingerToggleGroup;

    public GameObject TargetPlayer0Done;
    public GameObject TargetPlayer1Done;
    public GameObject TargetCanvas;

    int curPlayer = 0;

    /// <summary>
    /// Depending on curpPlayer, set "Player0Done" or "Player1Done" to match Canvas and change curPlayer.
    /// change isOn of selected toggle to false.
    /// </summary>
    public void OnClickDoneButton()
    {
        if (fingerToggleGroup.SelectedFinger != -1) // check if player choose toggle
        {
            if (curPlayer == 0)
            {
                TargetPlayer0Done.transform.position = TargetCanvas.transform.position;
            }
            else if (curPlayer == 1)
            {
                TargetPlayer1Done.transform.position = TargetCanvas.transform.position;
            }
            curPlayer = (curPlayer == 0) ? 1 : 0;

            List<Toggle> Buttons = new();
            Buttons.AddRange(fingerToggleGroup.GetComponentsInChildren<Toggle>());
            Buttons[fingerToggleGroup.SelectedFinger].isOn = false;
        }
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