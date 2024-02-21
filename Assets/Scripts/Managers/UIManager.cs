using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    
    public FingerToggleGroup fingerToggleGroup;
    Sprite Player1Image;
    Sprite Player2Image;

    [SerializeField] private TMP_Text playerText;
    [SerializeField] private List<Toggle> Buttons;

    [SerializeField] private GameObject WaitForResult;
    [SerializeField] private GameObject ResultCanvas;
    [SerializeField] private GameObject player1Text;
    [SerializeField] private GameObject player2Text;
    [SerializeField] private GameObject player1Image;
    [SerializeField] private GameObject player2Image;
    [SerializeField] private GameObject battlemanagerobj;

    string player1name = "";
    string player2name = "";

    private int drawcount = 0;
    public GameObject TargetPlayer0Done;
    public GameObject TargetPlayer1Done;
    public GameObject TargetCanvas;
    
    public BattleManager battlemanager { get; private set; }
    private GameStateManager _gameStateManager;

    int curPlayer = 0;
    

    void Start()
    {
        player1name = PlayerPrefs.GetString("player1Name");
        player2name = PlayerPrefs.GetString("player2Name");
        _gameStateManager = GameManager.Instance.GameStateManager;
        battlemanager = battlemanagerobj.GetComponent<BattleManager>();
    }

    private void Update()
    {
        switch (_gameStateManager.GameState)
        {
            case GameState.Player1Turn:
                playerText.text = GameManager.Instance.player1.playerName;
                break;
            case GameState.Player2Turn:
                playerText.text = GameManager.Instance.player2.playerName;
                break;
            default:
                playerText.text = "";
                break;
        }
    }

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
                GameManager.Instance.player1.latestChoice = fingerToggleGroup.SelectedFinger;
                TargetPlayer0Done.transform.position = TargetCanvas.transform.position;

                TargetPlayer0Done.transform.Find("Text").GetComponent<TMP_Text>().text
                    = "화면을 누르면\n" + GameManager.Instance.player2.playerName + "이 손가락을 선택합니다." ;

                
                Player1Image = Buttons[fingerToggleGroup.SelectedFinger].GetComponentInChildren<Image>().sprite;
            }
            else if (curPlayer == 1)
            {
                GameManager.Instance.player2.latestChoice = fingerToggleGroup.SelectedFinger;
                TargetPlayer1Done.transform.position = TargetCanvas.transform.position;
                WaitForResult.transform.position = TargetCanvas.transform.position;
                
                Player2Image = Buttons[fingerToggleGroup.SelectedFinger].GetComponentInChildren<Image>().sprite;
            }
            curPlayer++;
            Buttons[fingerToggleGroup.SelectedFinger].isOn = false;
            fingerToggleGroup.SelectedFinger = -1;
            
            if (curPlayer == 2)
            {
                curPlayer -= 2;
                _gameStateManager.readyBattleResult();
            }
            
        }
    }

    /// <summary>
    /// set the position of "Player0Done" to (2000,0,0)
    /// change state to player2Turn
    /// </summary>
    public void OnClickPlayer0DoneButton()
    {
        TargetPlayer0Done.transform.position = new Vector3(2000, 0, 0);
        _gameStateManager.turnToPlayer2();
    }

    /// <summary>
    /// set the position of "Player1Done" to (2000,0,0)
    /// change state to BattleResult
    /// </summary>
    public void OnClickPlayer1DOneButton()
    {
        TargetPlayer1Done.transform.position = new Vector3(2000, 0, 0);
        _gameStateManager.showBattleResult();
        //Debug.Log(GameManager.Instance.player1.latestChoice);
        //Debug.Log(GameManager.Instance.player2.latestChoice);
    }

    /// set the position of "WaitForResult" to (3000, 0, 0)
    /// prepare for resultcanvas, changing texts and images
    /// </summary>
    public void OnClickWaitForResult()
    {
        _gameStateManager.showBattleResult();
        string player1result = "";
        string player2result = "";
        int player1Choice = GameManager.Instance.player1.latestChoice;
        int player2Choice = GameManager.Instance.player2.latestChoice;
        BattleManager.RSPState result = battlemanager.CompareSelection(player1Choice, player2Choice);
        
        ResultCanvas.transform.position = TargetCanvas.transform.position;

        player1Image.GetComponent<Image>().sprite = Player1Image;
        player2Image.GetComponent<Image>().sprite = Player2Image;

        switch (result)
        {
            case BattleManager.RSPState.oneWin:
                player1result= player1name + " wins!";
                player2result = player2name + " loses!";
                break;
            case BattleManager.RSPState.twoWin:
                player2result = player2name + " wins!";
                player1result = player1name + " loses!";
                break;
            case BattleManager.RSPState.oneWinWithZero:
                player1result = player1name + " wins with zero!";
                player2result = player2name + " loses!";
                break;
            case BattleManager.RSPState.twoWinWithZero:
                player2result = player2name + " wins with zero!";
                player1result = player1name + " loses!";
                break;
            case BattleManager.RSPState.draw:
                if(drawcount >= 2)
                {
                    player1result = "The 3rd draw!";
                    player2result = "The 3rd draw!";
                    break;
                }
                player1result = "It's a draw! (" + drawcount.ToString() + "/3)";
                player2result = "It's a draw! (" + drawcount.ToString() + "/3)";
                drawcount++;
                break;
            default:
                Debug.Log("Unprecedented case occurs. Error");
                break;
        }
        player1Text.GetComponent<TMP_Text>().text = player1result;
        player2Text.GetComponent<TMP_Text>().text = player2result;

    }
}