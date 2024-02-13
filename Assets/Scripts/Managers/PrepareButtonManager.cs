using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class PrepareButtonManager : MonoBehaviour
{
    [SerializeField] private GameObject preparebuttons;
    [SerializeField] private GameObject moveButtons;
    [SerializeField] private TokenManager tokenManager;

    [SerializeField] private GameObject prepareButtonPrefab;
    [SerializeField] private GameObject moveButtonPrefab;

    private Queue<PrepareButton> prepareButtonPool;
    private Queue<MoveButton> moveButtonPool;
    private List<PrepareButton> activePrepareButtonList;
    private List<MoveButton> activeMoveButtonList;

    private int curPlayer;
    private List<Token> curTokenList;
    private List<Vector2> curInitialPositionList;

    private List<int> wayGo = new() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10,
                                    11, 12, 13, 14, 15, 16, 17, 18, 19, 0 };
    private List<int> wayUp = new() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
    private List<int> wayLeft = new() { 6, 7, 8, 9, 10, 11, 12, 13, 14 };
    private List<int> wayDown = new() { 11, 12, 13, 14, 15, 16, 17, 18, 19 };
    private List<int> wayRight = new() { 15, 16, 17, 18, 19, 0, 0, 0, 0, 0 };
    private List<int> wayLeftDiag = new() { 10, 25, 26, 22, 27, 28, 0, 0, 0, 0, 0 };
    private List<int> wayRightDiag = new() { 5, 20, 21, 22, 23, 24, 15, 16, 17, 18, 19 };

    private void Start()
    {
        GetInfo();

        prepareButtonPool = new();
        activePrepareButtonList = new();
        for (int i = 0; i < curInitialPositionList.Count; i++)
        {
            PrepareButton button = Instantiate<GameObject>(prepareButtonPrefab).GetComponent<PrepareButton>();
            button.transform.SetParent(preparebuttons.transform, false);
            button.gameObject.SetActive(false);
            prepareButtonPool.Enqueue(button);
        }

        moveButtonPool = new();
        activeMoveButtonList = new();
        for (int i = 0; i < 6; i++)
        {
            MoveButton button = Instantiate<GameObject>(moveButtonPrefab).GetComponent<MoveButton>();
            button.transform.SetParent(moveButtons.transform, false);
            button.gameObject.SetActive(false);
            moveButtonPool.Enqueue(button);
        }
    }

    public void GetInfo()
    {
        curPlayer = 0;
        curTokenList = (curPlayer == 0) ? tokenManager.tokens1 : tokenManager.tokens2;
        curInitialPositionList = (curPlayer == 0) ? tokenManager.initialPositions1 : tokenManager.initialPositions2;
    }

    public int GetBoardPointIndex(Token token, Vector2 tokenPosition)
    {
        for (int index = 0; index < tokenManager.boardPoints.Count; index++)
        {
            if (token.IsTokenAt(tokenPosition)) return index;
        }
        return -1;
    }

    public void ClickPrepareButton(Token curToken)
    {
        StartCoroutine(ClickPrepareButtonCoroutine(curToken));
    }

    public IEnumerator ClickPrepareButtonCoroutine(Token curToken)
    {
        DeactivePrepareButtons();

        yield return curToken.MoveTo(tokenManager.boardPoints[0]);

        ActiveMoveButtons(curToken);
    }

    public void ClickMoveButton(Token curToken, int steps)
    {
        DeactiveMoveButtons();

        tokenManager.GetTokenSteps(curToken, steps);

        //ActivePrepareButtons();
    }
    
    public void ActivePrepareButtons()
    {
        List<int> prepareButtonIndexList = GetPrepareButtonIndexList();
        for (int i = 0; i < prepareButtonIndexList.Count; i++)
        {
            if (prepareButtonIndexList[i] != -2)
            {
                ActivePrepareButton(i, prepareButtonIndexList[i]);
            }
        }
    }

    /// <summary>
    /// initialposition : -1
    /// IsFinished == false : -2
    /// else : token's index
    /// </summary>
    private List<int> GetPrepareButtonIndexList()
    {
        List<int> buttonIndexList = new();

        foreach (Token curToken in curTokenList)
        {
            if (curToken.IsFinished == false) buttonIndexList.Add(tokenManager.GetBoardPointIndex(curToken));
            else buttonIndexList.Add(-2);
        }

        return buttonIndexList;
    }

    private void ActivePrepareButton(int tokenNumber, int buttonIndex)
    {
        Vector2 buttonPosition;
        if (buttonIndex == -1) buttonPosition = curInitialPositionList[tokenNumber];
        else buttonPosition = tokenManager.boardPoints[buttonIndex].transform.position;

        PrepareButton prepareButton = prepareButtonPool.Dequeue();
        prepareButton.prepareButtonManager = this;
        prepareButton.thisToken = curTokenList[tokenNumber];
        prepareButton.transform.position = buttonPosition;
        prepareButton.gameObject.SetActive(true);
        activePrepareButtonList.Add(prepareButton);
    }

    public void DeactivePrepareButtons()
    {
        foreach(PrepareButton prepareButton in activePrepareButtonList)
        {
            prepareButton.gameObject.SetActive(false);
            prepareButtonPool.Enqueue(prepareButton);
        }

        activePrepareButtonList.Clear();
    }
    
    public void ActiveMoveButtons(Token curToken)
    {
        List<int> stepsList = new() { -1, 1, 2, 3, 4, 5 };
        List<int> moveButtonIndexList = GetMoveButtonIndexList(curToken);
        for (int i = 0; i < moveButtonIndexList.Count; i++)
        {
            ActiveMoveButton(curToken, stepsList[i], moveButtonIndexList[i]);
        }
    }

    private List<int> GetMoveButtonIndexList(Token curToken)
    {
        List<int> buttonIndexList = new();

        int curTokenIndex = tokenManager.GetBoardPointIndex(curToken);

        int buttonIndex;

        if (curToken.CountPreviousPositions() == 0)
        {
            int wayGoIndex = wayGo.LastIndexOf(curTokenIndex);
            buttonIndex = wayGo[wayGoIndex - 1];
        }
        else
        {
            Vector2 previousPosition = curToken.PeekPreviousPositions();
            buttonIndex = GetBoardPointIndex(curToken, previousPosition);
        }

        buttonIndexList.Add(buttonIndex);

        List<int> wayList = new();

        if (0 <= curTokenIndex && curTokenIndex <= 4) wayList = wayUp;
        else if (6 <= curTokenIndex && curTokenIndex <= 9) wayList = wayLeft;
        else if (11 <= curTokenIndex && curTokenIndex <= 14) wayList = wayDown;
        else if (15 <= curTokenIndex && curTokenIndex <= 19) wayList = wayRight;
        else if (wayLeftDiag.Contains(curTokenIndex)) wayList = wayLeftDiag;
        else if (wayRightDiag.Contains(curTokenIndex)) wayList = wayRightDiag;

        int wayListIndex = wayList.IndexOf(curTokenIndex);

        List<int> stepsList = new() { 1, 2, 3, 4, 5 };
        foreach (int steps in stepsList)
        {
            buttonIndex = wayList[wayListIndex + steps];
            if (!buttonIndexList.Contains(buttonIndex))
            {
                buttonIndexList.Add(buttonIndex);
            }

        }

        return buttonIndexList;
    }

    private void ActiveMoveButton(Token curToken, int steps, int buttonIndex)
    {
        Vector2 buttonPosition = tokenManager.boardPoints[buttonIndex].transform.position;

        MoveButton moveButton = moveButtonPool.Dequeue();
        moveButton.prepareButtonManager = this;
        moveButton.thisToken = curToken;
        moveButton.steps = steps;
        moveButton.transform.position = buttonPosition;
        moveButton.gameObject.SetActive(true);
        activeMoveButtonList.Add(moveButton);
    }

    public void DeactiveMoveButtons()
    {
        foreach (MoveButton moveButton in activeMoveButtonList)
        {
            moveButton.gameObject.SetActive(false);
            moveButtonPool.Enqueue(moveButton);
        }

        activeMoveButtonList.Clear();
    }
}