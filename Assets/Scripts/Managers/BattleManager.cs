using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    public int player1selection = -1;
    public int player2selection = -1;
    bool IsItDraw = false;
    
    
    
    public RSPState[,] BattleIndex =
            {
            { RSPState.draw, RSPState.twoWin, RSPState.twoWin, RSPState.twoWin, RSPState.oneWinWithZero, RSPState.oneWinWithZero },
            { RSPState.oneWin, RSPState.draw, RSPState.twoWin, RSPState.twoWin, RSPState.oneWin, RSPState.oneWin},
            { RSPState.oneWin, RSPState.oneWin, RSPState.draw, RSPState.twoWin, RSPState.twoWin, RSPState.oneWin},
            { RSPState.oneWin, RSPState.oneWin, RSPState.oneWin, RSPState.draw, RSPState.twoWin, RSPState.twoWin},
            { RSPState.twoWinWithZero, RSPState.twoWin, RSPState.oneWin, RSPState.oneWin, RSPState.draw, RSPState.twoWin},
            { RSPState.twoWinWithZero, RSPState.twoWin, RSPState.twoWin, RSPState.oneWin, RSPState.oneWin, RSPState.draw}
        };

    public Player player1;
    public Player player2;
    public bool TotalDraw { get; private set; }

    public UIManager UIManager { get; private set; }

    public enum RSPState
    {
        oneWin = -2,
        oneWinWithZero = -1,
        draw = 0,
        twoWinWithZero = 1,
        twoWin = 2,
        error = 3
    }
    private void Awake()
    { 
        UIManager = GetComponentInChildren<UIManager>();
    }
    void Start()
    {
        player1 = GameManager.Instance.player1;
        player2 = GameManager.Instance.player2;
        TotalDraw = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public RSPState CompareSelection(int player1, int player2)
    {
        RSPState result = RSPState.error;
        result = BattleIndex[player1, player2];
        return result;
    }

    public RSPState GetRSPState()
    {
        int player1Choice = GameManager.Instance.player1.latestChoice;
        int player2Choice = GameManager.Instance.player2.latestChoice;
        return CompareSelection(player1Choice, player2Choice);
    }

    public int GetWinner()
    {
        switch (GetRSPState())
        {
            case RSPState.draw:
                return 0;

            case RSPState.oneWin:
            case RSPState.oneWinWithZero:
                return 1;

            case RSPState.twoWin:
            case RSPState.twoWinWithZero:
                return 2;

            default:
                return -1;
        }
    }

    public int GetSteps()
    {
        switch (GetRSPState())
        {
            case RSPState.oneWin:
                return GameManager.Instance.player1.latestChoice;

            case RSPState.twoWin:
                return GameManager.Instance.player2.latestChoice;

            case RSPState.oneWinWithZero:
            case RSPState.twoWinWithZero:
                return -1;

            default:
                return 0;
        }
    }
}