using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

public class PrepareManager : MonoBehaviour
{
    [SerializeField] private GameObject prepareImages, buttons;

    [SerializeField] private GameObject previewPrefab, targetPrefab;
    [SerializeField] private GameObject backButtonPrefab;

    private TokenManager _tokenManager;

    private Queue<GameObject> previewPool, targetPool;
    private Queue<BackButton> backButtonPool;
    private List<GameObject> activePreviewList, activeTargetList;
    private List<BackButton> activeBackButtonList;

    private int tokenCount;

    private void Start()
    {
        _tokenManager = GameManager.Instance.TokenManager;
    }

    public void ResetSettings()
    {
        ResetVars();

        ResetImages();

        ResetButtons();
    }

    private void ResetVars()
    {
        if (_tokenManager.initialPositions1.Count > _tokenManager.initialPositions2.Count)
        {
            tokenCount = _tokenManager.initialPositions1.Count;
        }
        else
        {
            tokenCount = _tokenManager.initialPositions2.Count;
        }
    }

    private void ResetImages() // reset preview and target
    {
        previewPool = new();
        activePreviewList = new();

        GameObject newPreview;
        for (int i = 0; i < tokenCount + 4; i++)
        {
            newPreview = Instantiate(previewPrefab);

            newPreview.transform.SetParent(prepareImages.transform);
            newPreview.transform.localScale = Vector3.one;

            newPreview.gameObject.SetActive(false);

            previewPool.Enqueue(newPreview);
        }

        targetPool = new();
        activeTargetList = new();

        GameObject newTarget;
        for (int i = 0; i < 3; i++)
        {
            newTarget = Instantiate(targetPrefab);

            newTarget.transform.SetParent(prepareImages.transform);
            newTarget.transform.localScale = Vector3.one;

            newTarget.gameObject.SetActive(false);

            targetPool.Enqueue(newTarget);
        }
    }

    private void ResetButtons() // reset backButton
    {
        backButtonPool = new();
        activeBackButtonList = new();

        BackButton newBackButton;
        for (int i = 0; i < 3; i++)
        {
            newBackButton = Instantiate(backButtonPrefab).GetComponent<BackButton>();

            newBackButton.transform.SetParent(buttons.transform);
            newBackButton.transform.localScale = Vector3.one;

            newBackButton.gameObject.SetActive(false);

            backButtonPool.Enqueue(newBackButton);
        }
    }

    public void PreparePreviews(int steps) // when Yut Board Start
    {
        ActivatePreviews(steps);
    }

    public void OnMouseEnterTokenGroup(Token token, int steps) // put mouse on token group
    {
        DeactivatePreviews();

        ActivateTargets(token, steps);
    }

    public void OnMouseExitTokenGroup(Token token, int steps) // put mouse off on token group
    {
        DeactivateTarget();

        ActivatePreviews(steps);
    }

    public void OnMouseDownTokenGroup(Token token, int steps) // click token group
    {
        DeactivateTarget();

        if (steps == -1 && _tokenManager.GetPreviousIndices(token).Count != 1) ActivateBackButtons(token);
    }

    public void OnClickBackButton() // click back button
    {
        DeactivateBackButtons();
    }

    private void ActivatePreviews(int steps)
    {
        List<Vector2> previewPositionList = new();
        foreach (Token token in _tokenManager.winTokenList)
        {
            if (_tokenManager.AbleToClickToken(token))
            {
                previewPositionList.AddRange(GetMovePositionList(token, steps));
            }
        }

        GameObject preview;
        foreach (Vector2 PreviewPosition in previewPositionList)
        {
            preview = previewPool.Dequeue();

            preview.transform.position = PreviewPosition;

            preview.gameObject.SetActive(true);

            activePreviewList.Add(preview);
        }
    }

    private void ActivateTargets(Token token, int steps)
    {
        List<Vector2> targetPositionList = GetMovePositionList(token, steps);

        GameObject target;
        foreach (Vector2 targetPosition in targetPositionList)
        {
            target = targetPool.Dequeue();

            target.transform.position = targetPosition;

            target.gameObject.SetActive(true);

            activeTargetList.Add(target);
        }
    }

    private void ActivateBackButtons(Token token)
    {
        List<BoardPointIndex> moveIndexList = _tokenManager.GetPreviousIndices(token);

        BackButton backButton;
        Vector2 backButtonPosition;
        foreach (BoardPointIndex moveIndex in moveIndexList)
        {
            backButtonPosition = _tokenManager.boardPoints[(int)moveIndex].transform.position;

            backButton = backButtonPool.Dequeue();

            backButton.transform.position = backButtonPosition;
            backButton.token = token;
            backButton.boardPointIndex = moveIndex;

            backButton.gameObject.SetActive(true);

            activeBackButtonList.Add(backButton);
        }
    }

    private void DeactivatePreviews()
    {
        foreach (GameObject activePreview in activePreviewList)
        {
            activePreview.gameObject.SetActive(false);

            previewPool.Enqueue(activePreview);
        }

        activePreviewList.Clear();
    }

    private void DeactivateTarget()
    {
        foreach (GameObject activeTarget in activeTargetList)
        {
            activeTarget.gameObject.SetActive(false);

            targetPool.Enqueue(activeTarget);
        }

        activeTargetList.Clear();
    }

    private void DeactivateBackButtons()
    {
        foreach (BackButton activeBackButton in activeBackButtonList)
        {
            activeBackButton.gameObject.SetActive(false);

            backButtonPool.Enqueue(activeBackButton);
        }

        activeBackButtonList.Clear();
    }

    private List<Vector2> GetMovePositionList(Token token, int steps)
    {
        List<Vector2> movePositionList = new();

        if (steps == -1)
        {
            List<BoardPointIndex> moveIndexList = _tokenManager.GetPreviousIndices(token);
            foreach (BoardPointIndex moveIndex in moveIndexList)
            {
                if (moveIndex == BoardPointIndex.Initial) movePositionList.Add(token.initialPosition);
                else if (moveIndex == BoardPointIndex.Finished) movePositionList.Add(token.finishedPosition);
                else movePositionList.Add(_tokenManager.boardPoints[(int)moveIndex].transform.position);
            }
        }
        else
        {
            BoardPointIndex moveIndex = _tokenManager.GetIndexAfterMove(token, steps);
            if (moveIndex == BoardPointIndex.Initial)
            {
                movePositionList.Add(token.initialPosition);
            }
            else if (moveIndex == BoardPointIndex.Finished)
            {
                movePositionList.Add(token.finishedPosition);
            }
            else
            {
                movePositionList.Add(_tokenManager.boardPoints[(int)moveIndex].transform.position);
            }
        }

        return movePositionList;
    }
}