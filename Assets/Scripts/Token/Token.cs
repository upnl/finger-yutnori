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

    private int totalDist, movedDist;
    private Stack<Vector2> previousPositions;
    private bool finished;

    private int curState; // For debugging
    public void InstantMoveTo(Vector2 position)
    {
        transform.position = position;
    }
    public void InstantMoveTo(GameObject boardPoint)
    {
        InstantMoveTo(boardPoint.transform.position);
    }
    public void MoveTo(Vector2 newPosition)
    {
        transform.DOMove(newPosition, 0.5f);
    }
    public void MoveTo(GameObject boardPoint)
    {
        MoveTo(boardPoint.transform.position);
    }

    public bool IsTokenAt(Vector2 position)
    {
        return transform.position.x == position.x && transform.position.y == position.y;
    }

    public bool IsTokenAt(GameObject boardPoint)
    {
        return IsTokenAt(boardPoint.transform.position);
    }

    public void StartMove(int totalDist)
    {
        this.totalDist = totalDist;
        movedDist = 0;
    }

    public bool IsDoneMoving() { return movedDist == totalDist; }

    private void HandleEndOfMove()
    {
        movedDist++;
        previousPositions.Push(transform.position);
    }

    private bool HandleRightPoints()
    {
        for (int i = 0; i < rightPoints.Count; i++)
        {
            if (IsTokenAt(rightPoints[i]))
            {
                if (i == rightPoints.Count - 1) MoveTo(upperRightPoint);
                else MoveTo(rightPoints[i+1]);

                HandleEndOfMove();
                return true;
            }
        }
        return false;
    }
    private bool HandleUpperPoints()
    {
        for (int i = 0; i < upperPoints.Count; i++)
        {
            if (IsTokenAt(upperPoints[i]))
            {
                if (i == upperPoints.Count - 1) MoveTo(upperLeftPoint);
                else MoveTo(upperPoints[i+1]);

                HandleEndOfMove();
                return true;
            }
        }
        return false;
    }
    private bool HandleLeftPoints()
    {
        for (int i = 0; i < leftPoints.Count; i++)
        {
            if (IsTokenAt(leftPoints[i]))
            {
                if (i == leftPoints.Count - 1) MoveTo(lowerLeftPoint);
                else MoveTo(leftPoints[i+1]);

                HandleEndOfMove();
                return true;
            }
        }
        return false;
    }
    private bool HandleLowerPoints()
    {
        for (int i = 0; i < lowerPoints.Count; i++)
        {
            if (IsTokenAt(lowerPoints[i]))
            {
                if (i == lowerPoints.Count - 1) MoveTo(lowerRightPoint);
                else MoveTo(lowerPoints[i+1]);

                HandleEndOfMove();
                return true;
            }
        }
        return false;
    }

    private bool HandleRightDiagPoints()
    {
        for (int i = 0; i < rightDiagPoints.Count; i++)
        {
            if (IsTokenAt(rightDiagPoints[i]))
            {
                if (i == 1) MoveTo(centralPoint);
                else if (i == 3) MoveTo(lowerLeftPoint);
                else MoveTo(rightDiagPoints[i + 1]);

                HandleEndOfMove();
                return true;
            }
        }
        return false;
    }
    private bool HandleLeftDiagPoints()
    {
        for (int i = 0; i < leftDiagPoints.Count; i++)
        {
            if (IsTokenAt(leftDiagPoints[i]))
            {
                if (i == 1) MoveTo(centralPoint);
                else if (i == 3) MoveTo(lowerRightPoint);
                else MoveTo(leftDiagPoints[i + 1]);

                HandleEndOfMove();
                return true;
            }
        }
        return false;
    }

    private bool HandleCentralPoint()
    {
        if (IsTokenAt(centralPoint))
        {
            if (movedDist == 0 || previousPositions.Peek() == (Vector2)leftDiagPoints[1].transform.position)
                MoveTo(leftDiagPoints[2]);
            else MoveTo(rightDiagPoints[2]);
            HandleEndOfMove();
            return true;
        }
        return false;
    }
    private bool HandleLowerRightPoint()
    {
        if (IsTokenAt(lowerRightPoint))
        {
            if (previousPositions.Count > 0 &&
               (previousPositions.Peek() == (Vector2)lowerPoints[3].transform.position ||
                previousPositions.Peek() == (Vector2)leftDiagPoints[3].transform.position)) finished = true;
            else MoveTo(rightPoints[0]);
            HandleEndOfMove();
            return true;
        }
        return false;
    }
    private bool HandleUpperRightPoint()
    {
        if (IsTokenAt(upperRightPoint))
        {
            if (movedDist == 0) MoveTo(rightDiagPoints[0]);
            else MoveTo(upperPoints[0]);
            HandleEndOfMove();
            return true;
        }
        return false;
    }
    private bool HandleUpperLeftPoint()
    {
        if (IsTokenAt(upperLeftPoint))
        {
            if (movedDist == 0) MoveTo(leftDiagPoints[0]);
            else MoveTo(leftPoints[0]);
            HandleEndOfMove();
            return true;
        }
        return false;
    }
    private bool HandleLowerLeftPoint()
    {
        if (IsTokenAt(lowerLeftPoint))
        {
            MoveTo(lowerPoints[0]);
            HandleEndOfMove();
            return true;
        }
        return false;
    }

    private bool HandleBackwardsMove()
    {
        if (totalDist < 0)
        {
            if (previousPositions.Count > 0)
                MoveTo(previousPositions.Pop());
            movedDist--;
            return true;
        }
        return false;
    }

    private void DebugHandleKeyPress()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            curState = 1;
            StartMove(1);
            return;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            curState = 1;
            StartMove(2);
            return;
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            curState = 1;
            StartMove(3);
            return;
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            curState = 1;
            StartMove(4);
            return;
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            curState = 1;
            StartMove(5);
            return;
        }
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            curState = 1;
            StartMove(-1);
            return;
        }
    }

    private void Start()
    {
        totalDist = movedDist = curState = 0;
        finished = false;
        previousPositions = new Stack<Vector2>();
        MoveTo(lowerRightPoint);
    }

    private void Update()
    {
        if (curState == 0)
        {
            DebugHandleKeyPress();
            return;
        }
        if (finished || IsDoneMoving())
        {
            curState = 0;
            return;
        }
        if (HandleBackwardsMove()) return;
        
        if (HandleRightPoints()) return;
        if (HandleUpperPoints()) return;
        if (HandleLeftPoints()) return;
        if (HandleLowerPoints()) return;
        if (HandleRightDiagPoints()) return;
        if (HandleLeftDiagPoints()) return;
        if (HandleCentralPoint()) return;
        if (HandleLowerRightPoint()) return;
        if (HandleUpperRightPoint()) return;
        if (HandleUpperLeftPoint()) return;
        if (HandleLowerLeftPoint()) return;
    }
}
