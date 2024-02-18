using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    
    public FingerToggleGroup fingerToggleGroup;
    
    [SerializeField] private TMP_Text playerText;
    [SerializeField] private List<Toggle> Buttons;

    public GameObject TargetPlayer0Done;
    public GameObject TargetPlayer1Done;
    public GameObject TargetCanvas;

    private GameStateManager _gameStateManager;

    int curPlayer = 0;

    void Start()
    {
        _gameStateManager = GameManager.Instance.GameStateManager;
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
                TargetPlayer0Done.transform.position = TargetCanvas.transform.position;
            }
            else if (curPlayer == 1)
            {
                TargetPlayer1Done.transform.position = TargetCanvas.transform.position;
            }
            curPlayer = (curPlayer == 0) ? 1 : 0;
            Buttons[fingerToggleGroup.SelectedFinger].isOn = false;
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
        //Debug.Log(_gameStateManager.GameState);
        //Debug.Log(GameManager.Instance.player2.playerName);
    }

    /// <summary>
    /// set the position of "Player1Done" to (2000,0,0)
    /// change state to BattleResult
    /// </summary>
    public void OnClickPlayer1DOneButton()
    {
        TargetPlayer1Done.transform.position = new Vector3(2000, 0, 0);
        _gameStateManager.showBattleResult();
    }
    
}