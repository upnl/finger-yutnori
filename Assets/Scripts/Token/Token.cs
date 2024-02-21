using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using JetBrains.Annotations;
using System.Collections;
using Unity.VisualScripting.Antlr3.Runtime;

public class Token : MonoBehaviour
{
    public Vector2 initialPosition;
    public BoardPointIndex boardPointIndex;
    public Stack<BoardPointIndex> visitedCorners;
    public List<Token> stackedTokens;

    private void Awake()
    {
        visitedCorners = new Stack<BoardPointIndex>();
        stackedTokens = new List<Token>();
    }

    private void Update()
    {

    }

    /// <summary>
    /// Moves Token to newPosition instantly
    /// </summary>
    /// <param name="newPosition"></param>
    public void InstantMoveTo(Vector2 newPosition)
    {
        transform.position = newPosition;
    }

    /// <summary>
    /// Moves Token to boardPoint instantly
    /// </summary>
    /// <param name="boardPoint"></param>
    public void InstantMoveTo(GameObject boardPoint)
    {
        InstantMoveTo(boardPoint.transform.position);
    }

    /// <summary>
    /// Moves Token to newPosition smoothly; Use with StartCoroutine()
    /// </summary>
    /// <param name="newPosition"></param>
    public IEnumerator MoveTo(Vector2 newPosition)
    {
        yield return transform.DOMove(newPosition, 0.5f).WaitForCompletion();
    }

    /// <summary>
    /// Moves Token to boardPoint smoothly; Use with StartCoroutine()
    /// </summary>
    /// <param name="boardPoint"></param>
    public IEnumerator MoveTo(GameObject boardPoint)
    {
        yield return MoveTo(boardPoint.transform.position);
    }

    /// <summary>
    /// Returns whether the Token is at checkPosition
    /// </summary>
    /// <param name="checkPosition"></param>
    /// <returns></returns>
    public bool IsTokenAt(Vector2 checkPosition)
    {
        return transform.position.x == checkPosition.x && transform.position.y == checkPosition.y;
    }

    /// <summary>
    /// Returns whether the Token is at the same position as boardPoint
    /// </summary>
    /// <param name="boardPoint"></param>
    /// <returns></returns>
    public bool IsTokenAt(GameObject boardPoint)
    {
        return IsTokenAt(boardPoint.transform.position);
    }

    /// <summary>
    /// Returns whether the Token is at the same position as otherToken
    /// </summary>
    /// <param name="otherToken"></param>
    /// <returns></returns>
    public bool IsStackable(Token otherToken)
    {
        return IsTokenAt(otherToken.transform.position);
    }

    public bool isAtCorner()
    {
        return boardPointIndex == BoardPointIndex.LowerRight ||
               boardPointIndex == BoardPointIndex.UpperRight ||
               boardPointIndex == BoardPointIndex.UpperLeft ||
               boardPointIndex == BoardPointIndex.LowerLeft ||
               boardPointIndex == BoardPointIndex.Center;
    }

    public bool isValid()
    {
        return !isAtCorner() || visitedCorners.Contains(boardPointIndex);
    }

    /// <summary>
    /// If boardPointIndex is corner point, push it into visitedCorners
    /// </summary>
    /// <param name="boardPointIndex"></param>
    public void PushVisitedCorners(BoardPointIndex boardPointIndex)
    {
        if (boardPointIndex == BoardPointIndex.LowerRight ||
            boardPointIndex == BoardPointIndex.UpperRight ||
            boardPointIndex == BoardPointIndex.UpperLeft ||
            boardPointIndex == BoardPointIndex.LowerLeft ||
            boardPointIndex == BoardPointIndex.Center) visitedCorners.Push(boardPointIndex);
    }
    
    /// <summary>
    /// Pop visitedCorners until targetIndex is at top
    /// </summary>
    /// <param name="targetIndex"></param>
    public void PopVisitedCornersUntil(BoardPointIndex targetIndex)
    {
        if (visitedCorners.Contains(targetIndex))
        {
            while (visitedCorners.Peek() != targetIndex)
            {
                visitedCorners.Pop();
            }
        }
        foreach (Token stackedToken in stackedTokens) stackedToken.PopVisitedCornersUntil(targetIndex);
    }

    public BoardPointIndex GetPreviousCornerAtCorner()
    {
        if (visitedCorners.Count < 2) return BoardPointIndex.Initial;

        BoardPointIndex tempIndex = visitedCorners.Pop();
        BoardPointIndex result = visitedCorners.Peek();
        visitedCorners.Push(tempIndex);
        return result;
    }

    /// <summary>
    /// Stacks otherToken onto this Token
    /// </summary>
    /// <param name="otherToken"></param>
    public void Stack(Token otherToken)
    {
        stackedTokens.Add(otherToken);
        stackedTokens.AddRange(otherToken.stackedTokens);
        otherToken.stackedTokens.Clear();
        otherToken.gameObject.SetActive(false);
    }

    /// <summary>
    /// Unstacks all stackedTokens and moves them to initialPosition
    /// </summary>
    public void Unstack()
    {
        foreach (Token stackedToken in stackedTokens)
        {
            stackedToken.gameObject.SetActive(true);
            StartCoroutine(stackedToken.MoveTo(stackedToken.initialPosition));
            stackedToken.boardPointIndex = BoardPointIndex.Initial;
            stackedToken.visitedCorners.Clear();
        }
        stackedTokens.Clear();
    }

    private void OnMouseEnter()
    {
        TokenManager tokenManager = GameObject.Find("TokenManager").GetComponent<TokenManager>();

        tokenManager.OnMouseEnterTokenGroup(this);
    }

    private void OnMouseExit()
    {
        TokenManager tokenManager = GameObject.Find("TokenManager").GetComponent<TokenManager>();

        tokenManager.OnMouseExitTokenGroup(this);
    }

    private void OnMouseDown()
    {
        TokenManager tokenManager = GameObject.Find("TokenManager").GetComponent<TokenManager>();

        tokenManager.OnMouseDownTokenGroup(this);
    }
}
