using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.UIElements;

public class PrepareManager : MonoBehaviour
{
    [SerializeField] private TokenManager tokenManager;
    [SerializeField] private GameObject buttons; // get buttons in this folder

    [SerializeField] private GameObject prepareButtonPrefab, moveButtonPrefab;

    private Queue<PrepareButton> prepareButtonPool;
    private Queue<MoveButton> moveButtonPool;
    private List<PrepareButton> activePrepareButtonList;
    private List<MoveButton> activeMoveButtonList;

    public void ResetSettings()
    {
        ResetButtons();
    }

    private void ResetButtons() // reset things about buttons
    {
        int buttonCount; // the maximum number of prepareButtons to create
        if (tokenManager.initialPositions1.Count > tokenManager.initialPositions2.Count)
        {
            buttonCount = tokenManager.initialPositions1.Count;
        }
        else
        {
            buttonCount = tokenManager.initialPositions2.Count;
        }

        prepareButtonPool = new();
        activePrepareButtonList = new();

        PrepareButton newPrepareButton;
        for (int i = 0; i < buttonCount; i++) // get prepareButtonPool ready
        {
            newPrepareButton = Instantiate(prepareButtonPrefab).GetComponent<PrepareButton>();

            newPrepareButton.transform.SetParent(buttons.transform);
            newPrepareButton.transform.localScale = Vector3.one;
            newPrepareButton.tokenManager = tokenManager;

            newPrepareButton.gameObject.SetActive(false);

            prepareButtonPool.Enqueue(newPrepareButton);
        }

        moveButtonPool = new();
        activeMoveButtonList = new();

        MoveButton newMoveButton;
        for (int i = 0; i < 6; i++) // get moveButtonPool ready
        {
            newMoveButton = Instantiate(moveButtonPrefab).GetComponent<MoveButton>();

            newMoveButton.transform.SetParent(buttons.transform);
            newMoveButton.transform.localScale = Vector3.one;
            newMoveButton.tokenManager = tokenManager;

            newMoveButton.gameObject.SetActive(false);

            moveButtonPool.Enqueue(newMoveButton);
        }
    }

    public void ActivePrepareButtons(int player, int steps)
    {
        List<Token> winTokens = (player == 1) ? tokenManager.tokens1 : tokenManager.tokens2; // get winplayer tokens

        List<Vector2> prepareButtonPositionList = GetPrepareButtonPositionList(winTokens, player);

        Token winToken;
        Vector2 prepareButtonPosition;
        for (int i = 0; i < prepareButtonPositionList.Count; i++)
        {
            winToken = winTokens[i];

            if (winToken.boardPointIndex == BoardPointIndex.Finished) continue;
            else if (steps == -1 && winToken.boardPointIndex == BoardPointIndex.Initial) continue;
            prepareButtonPosition = prepareButtonPositionList[i];
            ActivePrepareButton(winToken, prepareButtonPosition, steps);
        }

        if (activePrepareButtonList.Count == 0) tokenManager.FailAcitivePrepareButton(); // no token able to move
    }

    private void ActivePrepareButton(Token token, Vector2 position, int steps)
    {
        PrepareButton prepareButton = prepareButtonPool.Dequeue();

        prepareButton.transform.position = position;
        prepareButton.thisToken = token;
        prepareButton.steps = steps;

        prepareButton.gameObject.SetActive(true);

        activePrepareButtonList.Add(prepareButton);
    }

    public void ActiveMoveButton(Token token, int steps)
    {
        Vector2 moveButtonPosition = GetMoveButtonPosition(token, steps);

        MoveButton moveButton = moveButtonPool.Dequeue();

        moveButton.transform.position = moveButtonPosition;
        moveButton.thisToken = token;
        moveButton.steps = steps;

        moveButton.gameObject.SetActive(true);

        activeMoveButtonList.Add(moveButton);
    }

    public void DeactivePrepareButtons()
    {
        foreach (PrepareButton activePrepareButton in activePrepareButtonList)
        {
            activePrepareButton.gameObject.SetActive(false);

            prepareButtonPool.Enqueue(activePrepareButton);
        }

        activePrepareButtonList.Clear();
    }

    public void DeactiveMoveButtons()
    {
        foreach (MoveButton activeMoveButton in activeMoveButtonList)
        {
            activeMoveButton.gameObject.SetActive(false);

            moveButtonPool.Enqueue(activeMoveButton);
        }

        activeMoveButtonList.Clear();
    }

    private List<Vector2> GetPrepareButtonPositionList(List<Token> winTokens, int player)
    {
        List<Vector2> prepareButtonPositionList = new();

        Vector2 prepareButtonPosition;
        foreach (Token winToken in winTokens)
        {
            prepareButtonPosition = winToken.transform.position;
            if (prepareButtonPositionList.Contains(prepareButtonPosition) == false) // if in one position tokens more than 2 exist, create 1 prepareButton
            {
                prepareButtonPositionList.Add(prepareButtonPosition);
            }
        }

        return prepareButtonPositionList;
    }

    private Vector2 GetMoveButtonPosition(Token token, int steps)
    {
        Vector2 moveButtonPotition;

        BoardPointIndex moveButtonIndex = tokenManager.GetIndexAfterMove(token,steps);

        if (moveButtonIndex == BoardPointIndex.Initial) moveButtonPotition = token.initialPosition;
        else if (moveButtonIndex == BoardPointIndex.Finished) moveButtonPotition = tokenManager.finishedPosition;
        else moveButtonPotition = tokenManager.boardPoints[(int)moveButtonIndex].transform.position;

        return moveButtonPotition;
    }
}