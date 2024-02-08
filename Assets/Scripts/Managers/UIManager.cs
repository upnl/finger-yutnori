using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public FingerToggleGroup fingerToggleGroup;

    public Toggle ZeroButton;
    public Toggle OneButton;
    public Toggle TwoButton;
    public Toggle ThreeButton;
    public Toggle FourButton;
    public Toggle FiveButton;

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
        if (fingerToggleGroup.IsToggleClicked() == true) // check if player choose toggle
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

            switch (fingerToggleGroup.SelectedFinger)
            {
                case 0:
                    ZeroButton.isOn = false;
                    break;
                case 1:
                    OneButton.isOn = false;
                    break;
                case 2:
                    TwoButton.isOn = false;
                    break;
                case 3:
                    ThreeButton.isOn = false;
                    break;
                case 4:
                    FourButton.isOn = false;
                    break;
                case 5:
                    FiveButton.isOn = false;
                    break;
            }
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