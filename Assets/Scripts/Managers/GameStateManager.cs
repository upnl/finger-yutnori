using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
    ready,
    running,
    finished
}


public class GameStateManager : MonoBehaviour
{
    public GameState gameState { get; private set; }

    public void PrepareGame()
    {
        gameState = GameState.running;
    }
    
    
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
