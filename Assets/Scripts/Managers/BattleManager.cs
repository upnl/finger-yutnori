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
            { RSPState.draw, RSPState.twowinwithzero, RSPState.twowinwithzero, RSPState.twowinwithzero, RSPState.onewinwithzero, RSPState.onewinwithzero },
            { RSPState.onewinwithzero, RSPState.draw, RSPState.twowin, RSPState.twowin, RSPState.onewin, RSPState.onewin},
            { RSPState.onewinwithzero, RSPState.onewin, RSPState.draw, RSPState.twowin, RSPState.twowin, RSPState.onewin},
            { RSPState.onewinwithzero, RSPState.onewin, RSPState.onewin, RSPState.draw, RSPState.twowin, RSPState.twowin},
            { RSPState.twowinwithzero, RSPState.twowin, RSPState.onewin, RSPState.onewin, RSPState.draw, RSPState.twowin},
            { RSPState.twowinwithzero, RSPState.twowin, RSPState.twowin, RSPState.onewin, RSPState.onewin, RSPState.draw}
        };
    public int player1result { get; private set; }
    public int player2result { get; private set; }
    public bool TotalDraw { get; private set; }

    public UIManager uimanager { get; private set; }

    // Start is called before the first frame update
    public enum RSPState
    {
        onewin = -2,
        onewinwithzero = -1,
        draw = 0,
        twowinwithzero = 1,
        twowin = 2,
        error = 3
    }
    private void Awake()
    {
        uimanager = GetComponentInChildren<UIManager>();
    }
    void Start()
    {
        
        player1result = 0;
        player2result = 0;
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
}