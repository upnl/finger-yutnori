using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneTemplate;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UI;

public class PrepareButtonManager : MonoBehaviour
{
    [SerializeField] private Canvas canvas;
    [SerializeField] private TokenManager tokenManager;

    [SerializeField] private GameObject buttonPrefab;
    [SerializeField] private GameObject moveButtonPrefab;

    private Queue<PrepareButton> buttonPool;
    private List<PrepareButton> activeButtonList;

    private Queue<MoveButton> moveButtonPool;
    private List<MoveButton> activeMoveButtonList;

    private int curPlayer;

    private List<Token> curTokenList;
    private List<Vector2> curInitialPositionList;

    private void Start()
    {
        buttonPool = new();
        activeButtonList = new();
        for (int i = 0; i < 6; i++)
        {
            PrepareButton button = Instantiate<GameObject>(buttonPrefab).GetComponent<PrepareButton>();
            button.transform.SetParent(canvas.transform, false);
            button.gameObject.SetActive(false);
            buttonPool.Enqueue(button);
        }

        moveButtonPool = new();
        activeMoveButtonList = new();
        for (int i = 0; i < 6; i++)
        {
            MoveButton button = Instantiate<GameObject>(moveButtonPrefab).GetComponent<MoveButton>();
            button.transform.SetParent(canvas.transform, false);
            button.gameObject.SetActive(false);
            moveButtonPool.Enqueue(button);
        }
    }

    public void GetInfo()
    {
        curTokenList = tokenManager.curTokenList;
        curInitialPositionList = tokenManager.curInitialPositionList;
    }

    public void ActiveButtons()
    {
        List<int> buttonIndexList= GetButtonIndexList();
        for (int i = 0; i < buttonIndexList.Count; i++)
        {
            if (buttonIndexList[i] != -2) AciveButton(i, buttonIndexList[i]);
        }
    }

    private List<int> GetButtonIndexList()
    {
        List<int> buttonIndexList = new();

        for (int i = 0; i < curInitialPositionList.Count; i++)
        {
            if (curTokenList[i].isFinished == false) buttonIndexList.Add(curTokenList[i].GetBoardPointIndex());
            else buttonIndexList.Add(-2);
        }

        return buttonIndexList;
    }

    private void AciveButton(int tokenNumber, int buttonIndex)
    {
        Vector2 buttonPosition;
        if (buttonIndex == -1) buttonPosition = curInitialPositionList[tokenNumber];
        else buttonPosition = tokenManager.boardPoints[buttonIndex].transform.position;

        PrepareButton button = buttonPool.Dequeue();
        button.transform.position = buttonPosition;
        button.tokenManager = tokenManager;
        button.thisToken = curTokenList[tokenNumber];
        button.gameObject.SetActive(true);
        activeButtonList.Add(button);
    }

    public void DeactiveButtons()
    {
        foreach (PrepareButton button in activeButtonList)
        {
            button.gameObject.SetActive (false);
            buttonPool.Enqueue(button);
        }
    }

    public IEnumerator ActiveMoveButtons(Token curToken)
    {
        if (curToken.GetBoardPointIndex() == -1) yield return tokenManager.FirstSetToken(curToken);

        List<int> stepsList = new() { -1, 1, 2, 3, 4, 5 };
        List<int> moveButtonIndexList = GetMoveButtonIndexList(curToken);
        for (int i = 0; i < moveButtonIndexList.Count; i++) ActiveMoveButton(stepsList[i], moveButtonIndexList[i]);
    }

    private List<int> GetMoveButtonIndexList(Token curToken)
    {
        List<int> buttonIndexList = new();

        int curTokenIndex = curToken.GetBoardPointIndex();

        int buttonIndex;

        if (curToken.previousIndexs.Count == 0)
        {
            int wayGoIndex = tokenManager.wayGo.LastIndexOf(curTokenIndex);
            buttonIndex = tokenManager.wayGo[wayGoIndex - 1];
            buttonIndexList.Add(buttonIndex);
        }
        else buttonIndexList.Add(curToken.PeekPreviousIndex());

        List<int> wayList = new();

        if (0 <= curTokenIndex && curTokenIndex <= 4) wayList = tokenManager.wayUp;
        else if (6 <= curTokenIndex && curTokenIndex <= 9) wayList = tokenManager.wayLeft;
        else if (11 <= curTokenIndex && curTokenIndex <= 14) wayList = tokenManager.wayDown;
        else if (15 <= curTokenIndex && curTokenIndex <= 19) wayList = tokenManager.wayRight;
        else if (tokenManager.wayLeftDiag.Contains(curTokenIndex)) wayList = tokenManager.wayLeftDiag;
        else if (tokenManager.wayRightdDiag.Contains(curTokenIndex)) wayList = tokenManager.wayRightdDiag;

        int wayListIndex = wayList.IndexOf(curTokenIndex);

        List<int> stepsList = new() { 1, 2, 3, 4, 5 };
        foreach (int steps in stepsList)
        {
            buttonIndex = wayList[wayListIndex + steps];
            if (!buttonIndexList.Contains(buttonIndex)) buttonIndexList.Add(buttonIndex);
        }

        return buttonIndexList;
    }
    
    private void ActiveMoveButton(int steps, int buttonIndex)
    {
        Vector2 buttonPosition = tokenManager.boardPoints[buttonIndex].transform.position;
        MoveButton button = moveButtonPool.Dequeue();
        button.transform.position = buttonPosition;
        button.tokenManager = tokenManager;
        button.steps = steps;
        button.gameObject.SetActive(true);
        activeMoveButtonList.Add(button);
    }
    
    public void DeactiveMoveButtons()
    {
        foreach (MoveButton button in activeMoveButtonList)
        {
            button.gameObject.SetActive(false);
            moveButtonPool.Enqueue(button);
        }
    }
}