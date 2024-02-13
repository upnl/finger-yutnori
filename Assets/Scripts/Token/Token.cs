using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using JetBrains.Annotations;
using System.Collections;

public class Token : MonoBehaviour
{
    private bool _canFinish;        // Whether the token can finish on the next turn
    private bool _isFinished;       // Whether the token has completed a lap
    private bool _isFromLeftDiag;   // Whether the token came to center from leftDiag
    private Vector2 _initialPosition;
    private Vector2 _previousPosition;

    public bool canFinish { get => _canFinish; set => _canFinish = value; }
    public bool isFinished { get => _isFinished; set => _isFinished = value; }
    public bool isFromLeftDiag { get => _isFromLeftDiag; set => _isFromLeftDiag = value; }
    public Vector2 initialPosition { get => _initialPosition; set => _initialPosition = value; }
    public Vector2 previousPosition { get => _previousPosition; set => _previousPosition = value; }

    private void Awake()
    {
        _canFinish = false;
        _isFinished = false;
        _isFromLeftDiag = false;
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
    /// Moves token to newPosition smoothly and records previous position; Use with StartCoroutine()
    /// </summary>
    /// <param name="newPosition"></param>
    public IEnumerator MoveTo(Vector2 newPosition)
    {
        yield return transform.DOMove(newPosition, 0.5f).WaitForCompletion();
    }

    /// <summary>
    /// Moves token to boardPoint smoothly and records previous position; Use with StartCoroutine()
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
}
