using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using JetBrains.Annotations;
using System.Collections;

public class Token : MonoBehaviour
{
    public TokenManager tokenManager;

    public Vector2 initialPosition;
    public bool isFinished;    /// Whether the token has completed a lap
    public Stack<int> previousIndexs;     /// Stores the token's previous positions for backward movement

    private void Awake()
    {
        isFinished = false;
        previousIndexs = new();
    }

    /// <summary>
    /// Moves token to newPosition instantly
    /// </summary>
    /// <param name="newPosition"></param>
    public void InstantMoveTo(Vector2 newPosition)
    {
        transform.position = newPosition;
    }

    /// <summary>
    /// Moves token to boardPoint instantly
    /// </summary>
    /// <param name="boardPoint"></param>
    public void InstantMoveTo(GameObject boardPoint)
    {
        InstantMoveTo(boardPoint.transform.position);
    }

    public void InstantMoveTo(int index)
    {
        InstantMoveTo(tokenManager.boardPoints[index]);
    }

    /// <summary>
    /// Moves token to newPosition smoothly; Use with StartCoroutine()
    /// </summary>
    /// <param name="newPosition"></param>
    public IEnumerator MoveTo(Vector2 newPosition)
    {
        yield return transform.DOMove(newPosition, 0.5f).WaitForCompletion();
    }

    /// <summary>
    /// Moves token to boardPoint smoothly; Use with StartCoroutine()
    /// </summary>
    /// <param name="boardPoint"></param>
    public IEnumerator MoveTo(GameObject boardPoint)
    {
        yield return MoveTo(boardPoint.transform.position);
    }

    public IEnumerator MoveTo(int index)
    {
        yield return MoveTo(tokenManager.boardPoints[index]);
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

    public bool IsTokenAt(int index)
    {
        return IsTokenAt(tokenManager.boardPoints[index]);
    }

    public int GetBoardPointIndex()
    {
        for (int index = 0; index < tokenManager.boardPoints.Count; index++)
        {
            if (IsTokenAt(tokenManager.boardPoints[index])) return index;
        }
        return -1;
    }

    public void ClearpreviousIndexs()
    {
        previousIndexs.Clear();
    }

    /// <summary>
    /// Pushes Token's current position into previousPositions
    /// </summary>
    public void RecordpreviousIndex()
    {
        previousIndexs.Push(GetBoardPointIndex());
    }

    public int CountpreviousIndexs()
    {
        return previousIndexs.Count;
    }

    /// <summary>
    /// Pops and returns top of previousPositions
    /// </summary>
    /// <returns></returns>
    public int PoppreviousIndex()
    {
        return previousIndexs.Pop();
    }

    /// <summary>
    /// Returns top of previousPositions
    /// </summary>
    /// <returns></returns>
    public int PeekPreviousIndex()
    {
        return previousIndexs.Peek();
    }
}
