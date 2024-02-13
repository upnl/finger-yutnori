using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public FingerToggleGroup fingerToggleGroup;

    [SerializeField] private List<Toggle> Buttons;
    private int player1selection = -1;
    private int player2selection = -1;
    bool IsItDraw = false;
    private int drawcount = 0;
    string player1name = "";
    string player2name = "";
    Sprite Player1Image;
    Sprite Player2Image;


    public GameObject TargetPlayer0Done;
    public GameObject TargetPlayer1Done;
    public GameObject TargetCanvas;
    [SerializeField] private GameObject WaitForResult;
    [SerializeField] private GameObject ResultCanvas;
    [SerializeField] private GameObject WinnerText;
    [SerializeField] private GameObject LoserText;
    [SerializeField] private GameObject WinnerImage;
    [SerializeField] private GameObject LoserImage;
    
    public int player1result { get; private set; }
    public int player2result { get; private set; }
    public bool TotalDraw { get; private set; }
    //These are the variables that are needed to be taken to another scenes(Results of this scenes).
    //TotalDraw becomes true when players draw 3 times. Then you don't need to use int results

    int curPlayer = 0;

    private void Start()
    {
        player1name = PlayerPrefs.GetString("player1Name");
        player2name = PlayerPrefs.GetString("player2Name");
        player1result = 0;
        player2result = 0;
        TotalDraw = false;
    }

    /// <summary>
    /// Depending on curpPlayer, set "Player0Done" or "Player1Done" to match Canvas and change curPlayer.
    /// change isOn of selected toggle to false.
    /// It also takes the sprites of fingers in order to show them in result canvas.
    /// </summary>
    public void OnClickDoneButton()
    {
        if (fingerToggleGroup.SelectedFinger != -1) // check if player choose toggle
        {
            if (curPlayer == 0)
            {
                TargetPlayer0Done.transform.position = TargetCanvas.transform.position;
                player1selection = fingerToggleGroup.SelectedFinger;
                Player1Image = Buttons[fingerToggleGroup.SelectedFinger].GetComponentInChildren<Image>().sprite;
            }
            else if (curPlayer == 1)
            {
                WaitForResult.transform.position = TargetCanvas.transform.position;
                player2selection = fingerToggleGroup.SelectedFinger;
                Player2Image = Buttons[fingerToggleGroup.SelectedFinger].GetComponentInChildren<Image>().sprite;
            }
            curPlayer = (curPlayer == 0) ? 1 : 0;

            Buttons[fingerToggleGroup.SelectedFinger].isOn = false;
            fingerToggleGroup.SelectedFinger = -1;
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
    /// set the position of "WaitForResult" to (3000, 0, 0)
    /// prepare for resultcanvas, changing texts and images
    /// </summary>
    public void OnClickWaitForResult()
    {
        string winnerresult = "";
        string loserresult = "";
        int result = CompareSelection(player1selection, player2selection);

        WaitForResult.transform.position = new Vector3(3000, 0, 0);
        ResultCanvas.transform.position = TargetCanvas.transform.position;

        if((result == 1)||(result == 3))
        {
            WinnerImage.GetComponent<Image>().sprite = Player2Image;
            LoserImage.GetComponent<Image>().sprite = Player1Image;
        }
        else
        {
            WinnerImage.GetComponent <Image>().sprite = Player1Image;
            LoserImage.GetComponent<Image>().sprite = Player2Image;
        }

        switch (result)
        {
            case 0:
                winnerresult = player1name + " wins!";
                loserresult = player2name + " loses!";
                player1result = player1selection;
                break;
           case 1:
                winnerresult = player2name + " wins!";
                loserresult = player1name + " loses!";
                player2result = player2selection;
                break;
            case 2:
                winnerresult = player1name + " wins with zero!";
                loserresult = player2name + " loses!";
                player1result = -1;
                break;
            case 3:
                winnerresult = player2name + " wins with zero!";
                loserresult = player1name + " loses!";
                player2result = -1;
                break;
            case 4:
                winnerresult = "It's a draw! (" + drawcount.ToString() + "/3)";
                loserresult = "It's a draw! (" + drawcount.ToString() + "/3)";
                IsItDraw = true;
                break;
            case 5:
                winnerresult = "The 3rd draw!";
                loserresult = "The 3rd draw!";
                TotalDraw = true;
                break;
            default:
                Debug.Log("Unprecedented case occurs. Error");
                break;
        }
        WinnerText.GetComponent<TMP_Text>().text = winnerresult;
        LoserText.GetComponent<TMP_Text>().text = loserresult;
        
    }
    
    /// <summary>
    /// set the position of "ResultCanvas" to (4000, 0, 0)
    /// It also resets the battle scene when players draw.
    /// </summary>
    public void OnClickResultCanvas()
    {
        ResultCanvas.transform.position = new Vector3(4000, 0, 0);
        if(IsItDraw)
        {
            curPlayer = 0;
            IsItDraw = false;
        }
        else
        {
            //This part is the linkage between this scene and the next scene(maybe yutnori)
        }
    }

    /// <summary>
    /// compares the selections of two players
    /// return 0 if player0 wins, 1 if player1 wins, 2 if player0 wins with zero, 3 if player1 wins with zero,
    /// 4 if players draw, 5 if it is the third draw
    /// return -1 if error occurs
    /// </summary>
    /// <param name="player0"></param>
    /// <param name="player1"></param>
    /// <returns></returns>
    private int CompareSelection(int player0, int player1)
    {
        int sub = player1 - player0;
        if(sub == 0)
        {
            if(drawcount == 2)
            {
                drawcount = 0;
                return 5;
            }
            drawcount++;
            return 4;
        }
        if ((player0 == 0)||(player1 == 0))
        {
            if((sub>3)||((sub<0)&&(sub>-4)))
            {
                return 2;
            }
            else
            {
                return 3;
            }
        }
        if(((sub < 0)&&(sub > -3))||(sub>2))
        {
            return 0;
        }
        else
        {
            return 1;
        }
    }
}