using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private List<Toggle> Buttons;
    private FingerToggleGroup fingerToggleGroup;

    [SerializeField] private GameObject baseCanvas;
    [SerializeField] private GameObject selectScreen;
    [SerializeField] private GameObject changeTurnScreen;
    [SerializeField] private GameObject waitForResultScreen;
    [SerializeField] private GameObject showResultScreen;
    [SerializeField] private GameObject endScreen;
    [SerializeField] private TMP_Text player1NameBox;
    [SerializeField] private TMP_Text player2NameBox;
    

    public Vector2 selectScreenPosition;
    public Vector2 changeTurnScreenPosition;
    public Vector2 waitForResultScreenPosition;
    public Vector2 showResultScreenPosition;
    public Vector2 endScreenPosition;

    private Sprite player1ImageSprite;
    private Sprite player2ImageSprite;

    [SerializeField] private Sprite winEffect;
    [SerializeField] private Sprite loseEffect;
    [SerializeField] private Sprite NoneEffect;

    private string player1Name, player2Name;
    
    private int drawCount;

    private BattleManager _battleManager;
    private GameStateManager _gameStateManager;
    private TokenManager _tokenManager;
    private string player1Hand;
    private string player2Hand;
    void Awake()
    {
        drawCount = 0;

        fingerToggleGroup = selectScreen.GetComponentInChildren<FingerToggleGroup>();

        player1Name = PlayerPrefs.GetString("player1Name");
        player2Name = PlayerPrefs.GetString("player2Name");

        player1Hand = PlayerPrefs.GetString("player1Hand");
        player2Hand = PlayerPrefs.GetString("player2Hand");

        _battleManager = GameManager.Instance.BattleManager;
        _gameStateManager = GameManager.Instance.GameStateManager;
        _tokenManager = GameManager.Instance.TokenManager;

        selectScreenPosition = selectScreen.transform.position;
        changeTurnScreenPosition = changeTurnScreen.transform.position;
        waitForResultScreenPosition = waitForResultScreen.transform.position;
        showResultScreenPosition = showResultScreen.transform.position;
        endScreenPosition = endScreen.transform.position;
        
        player1NameBox.text = player1Name;
        player2NameBox.text = player2Name;
    }

    private void Update()
    {

    }

    public void YutnoriScreen()
    {
        showResultScreen.transform.position = showResultScreenPosition;
        _tokenManager.StartTurn(_battleManager.GetWinner(), _battleManager.GetSteps());
    }

    public void Player1TurnScreen()
    {
        showResultScreen.transform.position = showResultScreenPosition;
        selectScreen.transform.position = baseCanvas.transform.position;
        TMP_Text playerText = selectScreen.transform.Find("PlayerText").GetComponent<TMP_Text>();
        playerText.text = GameManager.Instance.player1.playerName;

        Transform toggleTransform;
        if(player1Hand == "LeftHand")
        {
            for (int i = 0; i < 6; i++)
            {
                toggleTransform = Buttons[i].GetComponent<Transform>();
                toggleTransform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);
            }
        }
        else
        {
            for (int i = 0; i < 6; i++)
            {
                toggleTransform = Buttons[i].GetComponent<Transform>();
                toggleTransform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            }
        }
    }

    public void ChangeTurnScreen()
    {
        selectScreen.transform.position = selectScreenPosition;
        changeTurnScreen.transform.position = baseCanvas.transform.position;

        GameManager.Instance.player1.latestChoice = fingerToggleGroup.selectedFinger;
        changeTurnScreen.transform.Find("Text").GetComponent<TMP_Text>().text
            = "화면을 누르면\n" + GameManager.Instance.player2.playerName + "이(가) 손가락을 선택합니다.";

        player1ImageSprite = Buttons[fingerToggleGroup.selectedFinger].GetComponentInChildren<Image>().sprite;
        Transform player1Scale = showResultScreen.transform.Find("Player1Image").GetComponent<Transform>();
        player1Scale.localScale = Buttons[fingerToggleGroup.selectedFinger].GetComponent<Transform>().localScale;
    }

    public void Player2TurnScreen()
    {
        changeTurnScreen.transform.position = changeTurnScreenPosition;
        selectScreen.transform.position = baseCanvas.transform.position;

        TMP_Text playerText = selectScreen.transform.Find("PlayerText").GetComponent<TMP_Text>();
        playerText.text = GameManager.Instance.player2.playerName;

        Transform toggleTransform;
        if (player2Hand == "LeftHand")
        {
            for (int i = 0; i < 6; i++)
            {
                toggleTransform = Buttons[i].GetComponent<Transform>();
                toggleTransform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);
            }
        }
        else
        {
            for (int i = 0; i < 6; i++)
            {
                toggleTransform = Buttons[i].GetComponent<Transform>();
                toggleTransform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            }
        }
    }

    public void WaitForResultScreen()
    {
        selectScreen.transform.position = selectScreenPosition;
        waitForResultScreen.transform.position = baseCanvas.transform.position;

        GameManager.Instance.player2.latestChoice = fingerToggleGroup.selectedFinger;
        player2ImageSprite = Buttons[fingerToggleGroup.selectedFinger].GetComponentInChildren<Image>().sprite;

        Transform player2Scale = showResultScreen.transform.Find("Player1Image").GetComponent<Transform>();
        player2Scale.localScale = Buttons[fingerToggleGroup.selectedFinger].GetComponent<Transform>().localScale;
    }

    public void ShowResultScreen()
    {
        waitForResultScreen.transform.position = waitForResultScreenPosition;
        showResultScreen.transform.position = baseCanvas.transform.position;

        string player1result = "";
        string player2result = "";
        
        BattleManager.RSPState result = _battleManager.GetRSPState();
        if (result != BattleManager.RSPState.draw) drawCount = 0;

        Image player1Image = showResultScreen.transform.Find("Player1Image").GetComponent<Image>();
        Image player2Image = showResultScreen.transform.Find("Player2Image").GetComponent<Image>();

        Image player1Effect = showResultScreen.transform.Find("Player1Effect").GetComponent<Image>();
        Image player2Effect = showResultScreen.transform.Find("Player2Effect").GetComponent<Image>();

        player1Image.sprite = player1ImageSprite;
        player2Image.sprite = player2ImageSprite;

        switch (result)
        {
            case BattleManager.RSPState.oneWin:
                player1result = player1Name + " wins!";
                player1Effect.sprite = winEffect;
                player2result = player2Name + " loses!";
                player2Effect.sprite = loseEffect;
                break;
            case BattleManager.RSPState.twoWin:
                player2result = player2Name + " wins!";
                player2Effect.sprite = winEffect;
                player1result = player1Name + " loses!";
                player1Effect.sprite = loseEffect;
                break;
            case BattleManager.RSPState.oneWinWithZero:
                player1result = player1Name + " wins with zero!";
                player1Effect.sprite = winEffect;
                player2result = player2Name + " loses!";
                player2Effect.sprite = loseEffect;
                break;
            case BattleManager.RSPState.twoWinWithZero:
                player2result = player2Name + " wins with zero!";
                player2Effect.sprite = winEffect;
                player1result = player1Name + " loses!";
                player1Effect.sprite = loseEffect;
                break;
            case BattleManager.RSPState.draw:
                player1Effect.sprite = NoneEffect;
                player2Effect.sprite = NoneEffect;

                if (drawCount >= 2)
                {
                    player1result = "The 3rd draw!";
                    player2result = "The 3rd draw!";
                    break;
                }
                player1result = "It's a draw! (" + drawCount.ToString() + "/3)";
                player2result = "It's a draw! (" + drawCount.ToString() + "/3)";
                drawCount++;
                break;
            default:
                Debug.Log("Unprecedented case occurs. Error");
                break;
        }
        showResultScreen.transform.Find("Player1Text").GetComponent<TMP_Text>().text = player1result;
        showResultScreen.transform.Find("Player2Text").GetComponent<TMP_Text>().text = player2result;

    }

    public void ShowEndScreen()
    {
        endScreen.transform.position = baseCanvas.transform.position;
        Text text = endScreen.GetComponentInChildren<Text>();
        Player winner = (_tokenManager.DecideWinner() == 1) ? GameManager.Instance.player1 : GameManager.Instance.player2;
        text.text = winner.playerName + " won!";
    }

    public void OnClickSelectButton()
    {
        if (fingerToggleGroup.selectedFinger != -1) // check if player choose toggle
        {
            if (_gameStateManager.gameState == GameState.Player1Turn)
            {
                _gameStateManager.StartChangeTurn();
            }
            else if (_gameStateManager.gameState == GameState.Player2Turn)
            {
                _gameStateManager.StartWaitForResult();
            }
            else Debug.Log("Error: button pressed not on turn");
            Buttons[fingerToggleGroup.selectedFinger].isOn = false;
            fingerToggleGroup.selectedFinger = -1;            
        }
    }

    public void OnClickChangeTurnButton()
    {
        _gameStateManager.StartPlayer2Turn();
    }

    public void OnClickShowResultButton()
    {
        _gameStateManager.StartShowResult();
    }

    public void OnClickStartYutnoriButton()
    {
        if (_battleManager.GetRSPState() == BattleManager.RSPState.draw /* && drawCount < 3 */)
            _gameStateManager.StartPlayer1Turn();
        else _gameStateManager.StartYutnori();
    }

    public void OnClickRestartButton()
    {
        SceneManager.LoadScene("Yutnori");
    }

    public void OnClickNewGameButton()
    {
        SceneManager.LoadScene("Login");
    }
}