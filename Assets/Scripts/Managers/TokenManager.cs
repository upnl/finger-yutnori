using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
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

    public int curPlayer = 0;
    public List<Token> tokenList1, tokenList2, curTokenList;
    public Token curToken;
    public List<Vector2> curInitialPositionList;

    public List<int> wayGo = new() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 0 };
    public List<int> wayUp = new() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
    public List<int> wayLeft = new() { 6, 7, 8, 9, 10, 11, 12, 13, 14 };
    public List<int> wayDown = new() { 11, 12, 13, 14, 15, 16, 17, 18, 19 };
    public List<int> wayRight = new() { 15, 16, 17, 18, 19, 0, 0, 0, 0, 0 };
    public List<int> wayLeftDiag = new() { 10, 25, 26, 22, 27, 28, 0, 0, 0, 0, 0 };
    public List<int> wayRightdDiag = new() { 5, 20, 21, 22, 23, 24, 15, 16, 17, 18, 19 };

    void Start()
    {
        tokenList1 = new();
        tokenList2 = new();

        for (int i = 0; i < initialPositions1.Count; i++)
        {
            Token newToken = Instantiate(tokenPrefab1).GetComponent<Token>();
            newToken.tokenManager = this;
            newToken.initialPosition = initialPositions1[i];
            newToken.transform.position = newToken.initialPosition;
            tokenList1.Add(newToken);
        }
        for (int i = 0; i < initialPositions2.Count; i++)
        {
            Token newToken = Instantiate(tokenPrefab2).GetComponent<Token>();
            newToken.tokenManager = this;
            newToken.initialPosition = initialPositions2[i];
            newToken.transform.position = newToken.initialPosition;
            tokenList2.Add(newToken);
        }

        curTokenList = (curPlayer == 0) ? tokenList1 : tokenList2;

        curInitialPositionList = (curPlayer == 0) ? initialPositions1 : initialPositions2;

        prepareButtonManager.GetInfo();

        prepareButtonManager.ActiveButtons();
    }
    
    public IEnumerator FirstSetToken(Token curToken)
    {
        yield return curToken.MoveTo(boardPoints[0]);
    }

    public IEnumerator CheckGetScore()
    {
        if (curToken.GetBoardPointIndex() == 0)
        {
            curToken.isFinished = true;
            yield return curToken.MoveTo(curToken.initialPosition);
        }
    }

    public void ClickPrepareButton(Token token)
    {
        curToken = token;
        prepareButtonManager.DeactiveButtons();
        StartCoroutine(prepareButtonManager.ActiveMoveButtons(curToken));
    }

    public void ClickMoveButton(int steps)
    {
        prepareButtonManager.DeactiveMoveButtons();
        StartCoroutine(MoveToken(steps));
    }
    public IEnumerator MoveToken(int steps)
    {
        int curTokenIndex = curToken.GetBoardPointIndex();

        int moveIndex;

        if (steps == -1)
        {
            if (curToken.previousIndexs.Count == 0)
            {
                int wayGoIndex = wayGo.LastIndexOf(curTokenIndex);
                moveIndex = wayGo[wayGoIndex - 1];
            }
            else moveIndex = curToken.PoppreviousIndex();

            yield return curToken.MoveTo(moveIndex);
        }
        else
        {
            List<int> wayList = new();
            int wayListIndex;

            if (0 <= curTokenIndex && curTokenIndex <= 4) wayList = wayUp;
            else if (6 <= curTokenIndex && curTokenIndex <= 9) wayList = wayLeft;
            else if (11 <= curTokenIndex && curTokenIndex <= 14) wayList = wayDown;
            else if (15 <= curTokenIndex && curTokenIndex <= 19) wayList = wayRight;
            else if (wayLeftDiag.Contains(curTokenIndex)) wayList = wayLeftDiag;
            else if (wayRightdDiag.Contains(curTokenIndex)) wayList = wayRightdDiag;

            wayListIndex = wayList.IndexOf(curTokenIndex);
            moveIndex = wayList[wayListIndex + 1];

            curToken.RecordpreviousIndex();
            yield return curToken.MoveTo(moveIndex);
            StartCoroutine(CheckGetScore());

            for (int step = 1; step < steps; step++)
            {
                curTokenIndex = curToken.GetBoardPointIndex();
                if (curTokenIndex == 22)
                {
                    if (moveIndex == 21) moveIndex = 23;
                    else moveIndex = 27;
                }
                else
                {
                    if (20 <= curTokenIndex && curTokenIndex <= 24) wayList = wayRightdDiag;
                    else if (25 <= curTokenIndex && curTokenIndex <= 28) wayList = wayLeftDiag;
                    else wayList = wayGo;
                }
                wayListIndex = wayList.IndexOf(curTokenIndex);
                moveIndex = wayList[wayListIndex + 1];

                curToken.RecordpreviousIndex();
                yield return curToken.MoveTo(moveIndex);
                StartCoroutine(CheckGetScore());
            }
        }

        prepareButtonManager.GetInfo();
        prepareButtonManager.ActiveButtons();
    }
}
