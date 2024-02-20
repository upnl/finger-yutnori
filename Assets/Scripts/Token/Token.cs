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
    public bool IsStacked(Token otherToken)
    {
        return IsTokenAt(otherToken.transform.position);
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
    /// Pop visitedCorners until it pops targetIndex
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
        foreach (Token token in stackedTokens)
        {
            token.InstantMoveTo(transform.position);
            token.gameObject.SetActive(true);
            StartCoroutine(MoveTo(token.initialPosition));
        }
        stackedTokens.Clear();
    }
}
