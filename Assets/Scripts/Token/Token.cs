using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using JetBrains.Annotations;
using System.Collections;

public class Token : MonoBehaviour
{
    private bool isFinished;    /// Whether the token has completed a lap
    private Stack<Vector2> previousPositions;     /// Stores the token's previous positions for backward movement

    public bool IsFinished { get; set; }

    private void Awake()
    {
        isFinished = false;
        previousPositions = new Stack<Vector2>();
    }

    private void Update()
    {

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

    public void ClearPreviousPositions()
    {
        previousPositions.Clear();
    }

    /// <summary>
    /// Pushes Token's current position into previousPositions
    /// </summary>
    public void RecordPosition()
    {
        previousPositions.Push(transform.position);
    }

    public int CountPreviousPositions()
    {
        return previousPositions.Count;
    }

    /// <summary>
    /// Pops and returns top of previousPositions
    /// </summary>
    /// <returns></returns>
    public Vector2 PopPreviousPositions()
    {
        return previousPositions.Pop();
    }

    /// <summary>
    /// Returns top of previousPositions
    /// </summary>
    /// <returns></returns>
    public Vector2 PeekPreviousPositions()
    {
        return previousPositions.Peek();
    }
}
