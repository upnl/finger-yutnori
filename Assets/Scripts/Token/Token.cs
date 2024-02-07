using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using JetBrains.Annotations;

public class Token : MonoBehaviour
{
    [SerializeField] private List<GameObject> rightPoints;
    [SerializeField] private List<GameObject> upperPoints;
    [SerializeField] private List<GameObject> leftPoints;
    [SerializeField] private List<GameObject> lowerPoints;
    [SerializeField] private List<GameObject> rightDiagPoints;
    [SerializeField] private List<GameObject> leftDiagPoints;
    [SerializeField] private GameObject centralPoint;
    [SerializeField] private GameObject lowerRightPoint; // The starting point
    [SerializeField] private GameObject upperRightPoint;
    [SerializeField] private GameObject upperLeftPoint;
    [SerializeField] private GameObject lowerLeftPoint;

    /// <summary>
    /// Distance that token has to move on this turn; Negative values represent backward movement
    /// </summary>
    private int totalDist;

    /// <summary>
    /// Distance that token has moved on this turn
    /// </summary>
    private int movedDist;

    /// <summary>
    /// Stores the token's previous positions for backward movement
    /// </summary>
    private Stack<Vector2> previousPositions;

    /// <summary>
    /// Whether the token has completed a lap
    /// </summary>
    private bool finished;

    private void Start()
    {
        totalDist = movedDist = 0;
        finished = false;
        previousPositions = new Stack<Vector2>();
        MoveTo(lowerRightPoint);
    }

    /// <summary>
    /// If the token is finished or it is moving, return. If the token is done moving, accept inputs and return.
    /// Else, find where the token is and move it accordingly.
    /// </summary>
    private void Update()
    {
        if (finished || !IsOnBoardPoint()) return;
        if (IsDoneMoving())
        {
            DebugHandleKeyPress();
            return;
        }
        if (totalDist < 0)
        {
            MoveBackwards();
            return;
        }
        int index = CheckRightPoints();
        if (index != -1)
        {
            MoveFromRightPoints(index);
            return;
        }
        index = CheckUpperPoints();
        if (index != -1)
        {
            MoveFromUpperPoints(index);
            return;
        }
        index = CheckLeftPoints();
        if (index != -1)
        {
            MoveFromLeftPoints(index);
            return;
        }
        index = CheckLowerPoints();
        if (index != -1)
        {
            MoveFromLowerPoints(index);
            return;
        }
        index = CheckRightDiagPoints();
        if (index != -1)
        {
            MoveFromRightDiagPoints(index);
            return;
        }
        index = CheckLeftDiagPoints();
        if (index != -1)
        {
            MoveFromLeftDiagPoints(index);
            return;
        }
        index = CheckCentralPoint();
        if (index != -1)
        {
            MoveFromCentralPoint();
            return;
        }
        index = CheckLowerRightPoint();
        if (index != -1)
        {
            MoveFromLowerRightPoint();
            return;
        }
        index = CheckUpperRightPoint();
        if (index != -1)
        {
            MoveFromUpperRightPoint();
            return;
        }
        index = CheckUpperLeftPoint();
        if (index != -1)
        {
            MoveFromUpperLeftPoint();
            return;
        }
        index = CheckLowerLeftPoint();
        if (index != -1)
        {
            MoveFromLowerLeftPoint();
            return;
        }
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
    /// Moves token to newPosition smoothly
    /// </summary>
    /// <param name="newPosition"></param>
    public void MoveTo(Vector2 newPosition)
    {
        transform.DOMove(newPosition, 0.5f);
    }
    /// <summary>
    /// Moves token to boardPoint smoothly
    /// </summary>
    /// <param name="boardPoint"></param>
    public void MoveTo(GameObject boardPoint)
    {
        MoveTo(boardPoint.transform.position);
    }
    /// <summary>
    /// Returns as bool whether the Token is at checkPosition
    /// </summary>
    /// <param name="checkPosition"></param>
    /// <returns></returns>
    public bool IsTokenAt(Vector2 checkPosition)
    {
        return transform.position.x == checkPosition.x && transform.position.y == checkPosition.y;
    }
    /// <summary>
    /// Returns as bool whether the Token is at the same position as boardPoint
    /// </summary>
    /// <param name="boardPoint"></param>
    /// <returns></returns>
    public bool IsTokenAt(GameObject boardPoint)
    {
        return IsTokenAt(boardPoint.transform.position);
    }
    /// <summary>
    /// Sets token's totalDist to given value and resets movedDist to 0
    /// </summary>
    /// <param name="totalDist"></param>
    public void StartMove(int totalDist)
    {
        this.totalDist = totalDist;
        movedDist = 0;
    }
    /// <summary>
    /// Returns as bool whether the token's movedDist is equal to totalDist
    /// </summary>
    /// <returns></returns>
    public bool IsDoneMoving() { return movedDist == totalDist; }
    /// <summary>
    /// Increases movedDist by 1 and pushes current position into previousPositions
    /// </summary>
    private void HandleEndOfMove()
    {
        movedDist++;
        previousPositions.Push(transform.position);
    }
    /// <summary>
    /// Checks if the token is on rightPoints
    /// </summary>
    /// <returns>If on rightPoints, the index. If not, -1.</returns>
    private int CheckRightPoints()
    {
        for (int i = 0; i < 4; i++)
        {
            if (IsTokenAt(rightPoints[i])) return i;
        }
        return -1;
    }
    /// <summary>
    /// Moves the token from rightPoints[i]
    /// </summary>
    /// <param name="i"></param>
    private void MoveFromRightPoints(int i)
    {
        if (i == 3) MoveTo(upperRightPoint);
        else MoveTo(rightPoints[i + 1]);
        HandleEndOfMove();
    }
    /// <summary>
    /// Checks if the token is on upperPoints
    /// </summary>
    /// <returns>If on upperPoints, the index. If not, -1.</returns>
    private int CheckUpperPoints()
    {
        for (int i = 0; i < 4; i++)
        {
            if (IsTokenAt(upperPoints[i])) return i;
        }
        return -1;
    }
    /// <summary>
    /// Moves the token from upperPoints[i]
    /// </summary>
    /// <param name="i"></param>
    private void MoveFromUpperPoints(int i)
    {
        if (i == 3) MoveTo(upperLeftPoint);
        else MoveTo(upperPoints[i + 1]);
        HandleEndOfMove();
    }
    /// <summary>
    /// Checks if the token is on leftPoints
    /// </summary>
    /// <returns>If on leftPoints, the index. If not, -1.</returns>
    private int CheckLeftPoints()
    {
        for (int i = 0; i < 4; i++)
        {
            if (IsTokenAt(leftPoints[i])) return i;
        }
        return -1;
    }
    /// <summary>
    /// Moves the token from leftPoints[i]
    /// </summary>
    /// <param name="i"></param>
    private void MoveFromLeftPoints(int i)
    {
        if (i == 3) MoveTo(lowerLeftPoint);
        else MoveTo(leftPoints[i + 1]);
        HandleEndOfMove();
    }
    /// <summary>
    /// Checks if the token is on lowerPoints
    /// </summary>
    /// <returns>If on lowerPoints, the index. If not, -1.</returns>
    private int CheckLowerPoints()
    {
        for (int i = 0; i < 4; i++)
        {
            if (IsTokenAt(lowerPoints[i])) return i;
        }
        return -1;
    }
    /// <summary>
    /// Moves the token from upperPoints[i]
    /// </summary>
    /// <param name="i"></param>
    private void MoveFromLowerPoints(int i)
    {
        if (i == 3) MoveTo(lowerRightPoint);
        else MoveTo(lowerPoints[i + 1]);
        HandleEndOfMove();
    }
    /// <summary>
    /// Checks if the token is on rightDiagPoints
    /// </summary>
    /// <returns>If on rightDiagPoints, the index. If not, -1.</returns>
    private int CheckRightDiagPoints()
    {
        for (int i = 0; i < 4; i++)
        {
            if (IsTokenAt(rightDiagPoints[i])) return i;
        }
        return -1;
    }
    /// <summary>
    /// Moves the token from rightDiagPoints[i]
    /// </summary>
    /// <param name="i"></param>
    private void MoveFromRightDiagPoints(int i)
    {
        if (i == 1) MoveTo(centralPoint);
        else if (i == 3) MoveTo(lowerLeftPoint);
        else MoveTo(rightDiagPoints[i + 1]);
        HandleEndOfMove();
    }
    /// <summary>
    /// Checks if the token is on leftDiagPoints
    /// </summary>
    /// <returns>If on leftDiagPoints, the index. If not, -1.</returns>
    private int CheckLeftDiagPoints()
    {
        for (int i = 0; i < 4; i++)
        {
            if (IsTokenAt(leftDiagPoints[i])) return i;
        }
        return -1;
    }

    /// <summary>
    /// Moves the token from leftDiagPoints[i]
    /// </summary>
    /// <param name="i"></param>
    private void MoveFromLeftDiagPoints(int i)
    {
        if (i == 1) MoveTo(centralPoint);
        else if (i == 3) MoveTo(lowerRightPoint);
        else MoveTo(leftDiagPoints[i + 1]);
        HandleEndOfMove();
    }
    /// <summary>
    /// Checks if the token is on centralPoint
    /// </summary>
    /// <returns>If on centralPoint, 0. If not, -1.</returns>
    private int CheckCentralPoint()
    {
        if (IsTokenAt(centralPoint)) return 0;
        return -1;
    }

    /// <summary>
    /// Moves the token from centralPoint
    /// </summary>
    private void MoveFromCentralPoint()
    {
        if (movedDist == 0 || previousPositions.Peek() == (Vector2)leftDiagPoints[1].transform.position)
            MoveTo(leftDiagPoints[2]);
        else MoveTo(rightDiagPoints[2]);
        HandleEndOfMove();
    }
    /// <summary>
    /// Checks if the token is on lowerRightPoint
    /// </summary>
    /// <returns>If on lowerRightPoint, 0. If not, -1.</returns>
    private int CheckLowerRightPoint()
    {
        if (IsTokenAt(lowerRightPoint)) return 0;
        return -1;
    }
    /// <summary>
    /// Moves the token from lowerRightPoint and checks if the token has completed a lap
    /// </summary>
    private void MoveFromLowerRightPoint()
    {
        if (previousPositions.Count > 0 && (
            previousPositions.Peek() == (Vector2)lowerPoints[3].transform.position ||
            previousPositions.Peek() == (Vector2)leftDiagPoints[3].transform.position)) finished = true;
        else MoveTo(rightPoints[0]);
        HandleEndOfMove();
    }
    /// <summary>
    /// Checks if the token is on upperRightPoint
    /// </summary>
    /// <returns>If on upperRightPoint, 0. If not, -1.</returns>
    private int CheckUpperRightPoint()
    {
        if (IsTokenAt(upperRightPoint)) return 0;
        return -1;
    }
    /// <summary>
    /// Moves the token from upperRightPoint
    /// </summary>
    private void MoveFromUpperRightPoint()
    {
        if (movedDist == 0) MoveTo(rightDiagPoints[0]);
        else MoveTo(upperPoints[0]);
        HandleEndOfMove();
    }
    /// <summary>
    /// Checks if the token is on upperLeftPoint
    /// </summary>
    /// <returns>If on upperLeftPoint, 0. If not, -1.</returns>
    private int CheckUpperLeftPoint()
    {
        if (IsTokenAt(upperLeftPoint)) return 0;
        return -1;
    }
    /// <summary>
    /// Moves the token from upperLeftPoint
    /// </summary>
    private void MoveFromUpperLeftPoint()
    {
        if (movedDist == 0) MoveTo(leftDiagPoints[0]);
        else MoveTo(leftPoints[0]);
        HandleEndOfMove();
    }
    /// <summary>
    /// Checks if the token is on lowerLeftPoint
    /// </summary>
    /// <returns>If on lowerLeftPoint, 0. If not, -1.</returns>
    private int CheckLowerLeftPoint()
    {
        if (IsTokenAt(lowerLeftPoint)) return 0;
        return -1;
    }
    /// <summary>
    /// Moves the token from lowerLeftPoint
    /// </summary>
    private void MoveFromLowerLeftPoint()
    {
        MoveTo(lowerPoints[0]);
        HandleEndOfMove();
    }
    /// <summary>
    /// Returns whether the token is on any boardPoint
    /// </summary>
    /// <returns></returns>
    private bool IsOnBoardPoint()
    {
        return
            CheckRightPoints() != -1 ||
            CheckUpperPoints() != -1 ||
            CheckLeftPoints() != -1 ||
            CheckLowerPoints() != -1 ||
            CheckRightDiagPoints() != -1 ||
            CheckLeftDiagPoints() != -1 ||
            CheckCentralPoint() != -1 ||
            CheckLowerRightPoint() != -1 ||
            CheckUpperRightPoint() != -1 ||
            CheckUpperLeftPoint() != -1 ||
            CheckLowerLeftPoint() != -1;
    }
    /// <summary>
    /// Moves the token backwards
    /// </summary>
    private void MoveBackwards()
    {
        if (previousPositions.Count > 0)
            MoveTo(previousPositions.Pop());
        movedDist--;
    }
    /// <summary>
    /// Detects number key presses and starts moves accordingly
    /// </summary>
    private void DebugHandleKeyPress()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            StartMove(1);
            return;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            StartMove(2);
            return;
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            StartMove(3);
            return;
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            StartMove(4);
            return;
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            StartMove(5);
            return;
        }
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            StartMove(-1);
            return;
        }
    }
}
