using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build.Reporting;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UI;

public class PrepareManager : MonoBehaviour
{
    [SerializeField] private Canvas canvas;
    [SerializeField] private TokenManager tokenManager;

    [SerializeField] private GameObject buttonPrefab;

    private List <PrepareButton> buttons = new();

    private int curPlayer;

    private List<Token> tokens1, tokens2, curtokens;

    private List<int> wayUp = new() { 19, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
    private List<int> wayLeft = new() { 5, 6, 7, 8, 9, 10, 11, 12, 13, 14 };
    private List<int> wayDown = new() { 10, 11, 12, 13, 14, 15, 16, 17, 18, 19 };
    private List<int> wayRight = new() { 14, 15, 16, 17, 18, 19, 0, 0, 0, 0, 0 };
    private List<int> wayLeftDiag = new() { 9, 10, 25, 26, 22, 27, 28, 0, 0, 0, 0, 0 };
    private List<int> wayRightdDiag = new() { 4, 5, 20, 21, 22, 23, 24, 15, 16, 17, 18, 19 };

    public void CreateButtons()
    {
        GetInfo();

        foreach (Token token in curtokens)
        {
            int tokenIndex = tokenManager.GetBoardPointIndex(token);
            if (tokenIndex != -1)
            {
                List<int> stepses = new() { -1, 1, 2, 3, 4, 5 };
                List<int> buttonIndexs = GetButtonIndexs(tokenIndex);
                for (int i = 0; i < buttonIndexs.Count; i++) CreateButton(stepses[i], buttonIndexs[i]);
            }
        }
    }

    private void GetInfo()
    {
        curPlayer = 0; //TODO
        tokens1 = tokenManager.tokens1;
        tokens2 = tokenManager.tokens2;
        curtokens = (curPlayer == 0) ? tokens1 : tokens2;
    }

    private List<int> GetButtonIndexs(int tokenIndex)
    {
        List<int> buttonIndexs = new();

        List<int> wayList = new();
        int wayListIndex;

        if (0 <= tokenIndex && tokenIndex <= 4) wayList = wayUp;
        else if (6 <= tokenIndex && tokenIndex <= 9) wayList = wayLeft;
        else if (11 <= tokenIndex && tokenIndex <= 14) wayList = wayDown;
        else if (15 <= tokenIndex && tokenIndex <= 19) wayList = wayRight;
        else if (wayLeftDiag.Contains(tokenIndex)) wayList = wayLeftDiag;
        else if (wayRightdDiag.Contains(tokenIndex)) wayList = wayRightdDiag;

        wayListIndex = wayList.IndexOf(tokenIndex);

        List<int> stepses = new() { -1, 1, 2, 3, 4, 5 };
        foreach (int steps in stepses)
        {
            int buttonindex = wayList[wayListIndex + steps];
            if (!buttonIndexs.Contains(buttonindex)) buttonIndexs.Add(buttonindex);
        }

        return buttonIndexs;
    }

    private void CreateButton(int steps, int buttonIndex)
    {
        Vector2 buttonPosition = tokenManager.boardPoints[buttonIndex].transform.position;
        GameObject buttonObject = Instantiate<GameObject>(buttonPrefab, buttonPosition, Quaternion.identity);
        PrepareButton button = buttonObject.GetComponent<PrepareButton>();
        button.transform.SetParent(canvas.transform, false);
        button.transform.position = buttonPosition;
        button.tokenManager = tokenManager;
        button.prepareManager = this;
        button.steps = steps;
        buttons.Add(button);
    }

    public void DeleteButtonInCanvas()
    {
        foreach (PrepareButton button in buttons)
        {

            button.transform.SetParent(null);
        }
    }

    public void DestroyButtons()
    {
        foreach (PrepareButton button in buttons)
        {
            Destroy(button.gameObject);
        }

        buttons.Clear();
    }
}