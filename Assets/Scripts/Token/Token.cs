using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using JetBrains.Annotations;
using System.Collections;

public class Token : MonoBehaviour
{
    private bool _canFinish;  // false before moving out of lowerRightPoint
    private bool _hasLooped;  // true if token has reached lowerRightPoint with forward movement
    private int _routeType;   // 0: right->upper->left->lower / 1: right->upper->leftDiag
                              // 2: right->rightDiag->lower   / 3: right->rightDiag->leftDiag
    private BoardPointIndex _boardPointIndex;
    private Vector2 _initialPosition;

    public bool canFinish { get => _canFinish; set => _canFinish = value; }
    public bool hasLooped { get => _hasLooped; set => _hasLooped = value; }
    public int routeType { get => _routeType; set => _routeType = value; }
    public BoardPointIndex boardPointIndex { get => _boardPointIndex; set => _boardPointIndex = value; }
    public Vector2 initialPosition { get => _initialPosition; set => _initialPosition = value; }

    private void Awake()
    {
        _canFinish = false;
        _hasLooped = false;
        _routeType = 0;
        _boardPointIndex = BoardPointIndex.Initial;
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
