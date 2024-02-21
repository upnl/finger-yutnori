using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
    Player1Turn,
    Player2Turn,
    ReadyResult,
    BattleResult,
    MovToken,
    End
}


public class GameStateManager : MonoBehaviour
{
    public GameState GameState { get; private set; }

    public void turnToPlayer2()
    {
        GameState = GameState.Player2Turn;
    }

    public void readyBattleResult()
    {
        GameState = GameState.ReadyResult;
    }
    public void showBattleResult()
    {
        GameState = GameState.BattleResult;
    }
    
    void Start()
    {
        GameState = GameState.Player1Turn;
    }

    
    
}
