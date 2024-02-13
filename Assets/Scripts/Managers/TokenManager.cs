using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public enum boardPointIndex
{
    LowerRight,
    Right1,
    Right2,
    Right3,
    Right4,
    UpperRight,
    Upper1,
    Upper2,
    Upper3,
    Upper4,
    UpperLeft,
    Left1,
    Left2,
    Left3,
    Left4,
    LowerLeft,
    Lower1,
    Lower2,
    Lower3,
    Lower4,
    RightDiag1,
    RightDiag2,
    Center,
    RightDiag3,
    RightDiag4,
    LeftDiag1,
    LeftDiag2,
    LeftDiag3,
    LeftDiag4
}

public class TokenManager : MonoBehaviour
{
    [SerializeField] private PrepareButtonManager prepareButtonManager;
    public List<GameObject> boardPoints;

    [SerializeField] private GameObject tokenPrefab1, tokenPrefab2;
    public List<Vector2> initialPositions1, initialPositions2;

    public List<Token> tokens1, tokens2;

    private void Start()
    {
        tokens1 = new List<Token>();
        tokens2 = new List<Token>();

        for (int i = 0; i < initialPositions1.Count; i++)
        {
            var newToken = Instantiate<GameObject>(tokenPrefab1).GetComponent<Token>();
            newToken.initialPosition = initialPositions1[i];
            newToken.transform.position = newToken.initialPosition;
            tokens1.Add(newToken.GetComponent<Token>());
        }
        for (int i = 0; i < initialPositions1.Count; i++)
        {
            var newToken = Instantiate<GameObject>(tokenPrefab2).GetComponent<Token>();
            newToken.initialPosition = initialPositions2[i];
            newToken.transform.position = newToken.initialPosition;
            tokens2.Add(newToken.GetComponent<Token>());
        }

        prepareButtonManager.GetInfo();
        prepareButtonManager.ActivePrepareButtons();
    }

    /// <summary>
    /// Gets index of the boardPoint that token is on; -1 if none
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public int GetBoardPointIndex(Token token)
    {
        for (int index = 0; index < boardPoints.Count; index++)
        {
            if (token.IsTokenAt(boardPoints[index])) return index;
        }
        return -1;
    }

    public IEnumerator CheckFinish(Token curToken)
    {
        if (curToken.IsTokenAt(boardPoints[0]))
        {
            curToken.IsFinished = true;
            curToken.ClearPreviousPositions();
            yield return curToken.MoveTo(curToken.initialPosition);
        }
    }
    
    /// <summary>
    /// Actually moves token according to GetNextPosition(); Use with StartCoroutine()
    /// </summary>
    /// <param name="token"></param>
    /// <param name="distance"></param>
    /// <returns></returns>
    private IEnumerator MoveToken(Token token, int steps)
    {
        List<int> tokenIndexList = GetTokenIndexList(token, steps);
        foreach (int tokenIndex in tokenIndexList)
        {
            yield return token.MoveTo(boardPoints[tokenIndex]);
        }

        StartCoroutine(CheckFinish(token));

        prepareButtonManager.GetInfo();
        prepareButtonManager.ActivePrepareButtons();
    }

    private List<int> GetTokenIndexList(Token curToken, int steps)
    {
        List<int> tokenIndexList = new();

        int curTokenIndex = GetBoardPointIndex(curToken);

        int tokenIndex;

        if (steps == -1)
        {
            if (curToken.CountPreviousPositions() == 0)
            {
                int wayGOIndex = prepareButtonManager.wayGo.LastIndexOf(curTokenIndex);
                tokenIndex = prepareButtonManager.wayGo[wayGOIndex - 1];
            }
            else
            {
                Vector2 previousPosition = curToken.PopPreviousPositions();
                tokenIndex = prepareButtonManager.GetBoardPointIndex(previousPosition);
            }

            tokenIndexList.Add(tokenIndex);
        }
        else
        {
            List<int> wayList = new();

            if (0 <= curTokenIndex && curTokenIndex <= 4) wayList = prepareButtonManager.wayUp;
            else if (6 <= curTokenIndex && curTokenIndex <= 9) wayList = prepareButtonManager.wayLeft;
            else if (11 <= curTokenIndex && curTokenIndex <= 14) wayList = prepareButtonManager.wayDown;
            else if (15 <= curTokenIndex && curTokenIndex <= 19) wayList = prepareButtonManager.wayRight;
            else if (prepareButtonManager.wayLeftDiag.Contains(curTokenIndex)) wayList = prepareButtonManager.wayLeftDiag;
            else if (prepareButtonManager.wayRightDiag.Contains(curTokenIndex)) wayList = prepareButtonManager.wayRightDiag;

            int wayListIndex = wayList.IndexOf(curTokenIndex);

            List<int> stepsList = new() { 1, 2, 3, 4, 5 };
            for (int i = 0; i < steps; i++)
            {
                tokenIndex = wayList[wayListIndex + stepsList[i]];
                
                tokenIndexList.Add(tokenIndex);
            }

            curToken.RecordPosition(curToken.transform.position);
            for (int i = 0; i < steps - 1; i++)
            {
                curToken.RecordPosition(boardPoints[tokenIndexList[i]].transform.position);
            }
        }

        return tokenIndexList;
    }

    /// <summary>
    /// Starts a coroutine that moves token by distance
    /// </summary>
    /// <param name="token"></param>
    /// <param name="distance"></param>
    public void StartMove(Token curToken, int steps)
    {
        StartCoroutine(MoveToken(curToken, steps));
    }
}