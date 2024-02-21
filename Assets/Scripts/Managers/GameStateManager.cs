using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
    Yutnori,
    Player1Turn,
    ChangeTurn,
    Player2Turn,
    WaitForResult,
    ShowResult,
    End
}


public class GameStateManager : MonoBehaviour
{
    private UIManager _UIManager;
    public GameState gameState { get; private set; }

    void Start()
    {
        _UIManager = GameManager.Instance.UIManager;
        StartPlayer1Turn();
    }

    public void StartYutnori()
    {
        gameState = GameState.Yutnori;
        _UIManager.YutnoriScreen();
    }

    public void StartPlayer1Turn()
    {
        gameState = GameState.Player1Turn;
        _UIManager.Player1TurnScreen();
    }

    public void StartChangeTurn()
    {
        gameState = GameState.ChangeTurn;
        _UIManager.ChangeTurnScreen();
    }

    public void StartPlayer2Turn()
    {
        gameState = GameState.Player2Turn;
        _UIManager.Player2TurnScreen();
    }

    public void StartWaitForResult()
    {
        gameState = GameState.WaitForResult;
        _UIManager.WaitForResultScreen();
    }
    public void StartShowResult()
    {
        gameState = GameState.ShowResult;
        _UIManager.ShowResultScreen();
    }
}
