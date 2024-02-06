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
    private bool canFinish, finished;

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
        transform.DOMove(newPosition, 1f);
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

    private bool HandleRightPoints()
    {
        for (int i = 0; i < rightPoints.Count; i++)
        {
            if (IsTokenAt(rightPoints[i]))
            {
                if (i == rightPoints.Count - 1) MoveTo(upperRightPoint);
                else MoveTo(rightPoints[i+1]);

                movedDist++;
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

                movedDist++;
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

                movedDist++;
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

                movedDist++;
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

                movedDist++;
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

                movedDist++;
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
            movedDist++;
            return true;
        }
        return false;
    }
    private bool HandleLowerRightPoint()
    {
        if (IsTokenAt(lowerRightPoint))
        {
            if (canFinish) finished = true;
            else MoveTo(rightPoints[0]);
            return true;
        }
        return false;
    }
    private bool HandleUpperRightPoint()
    {
        if (IsTokenAt(upperRightPoint))
        {
            if (movedDist == 0) MoveTo(rightDiagPoints[0]);
            else MoveTo(rightPoints[0]);
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
            return true;
        }
        return false;
    }
    private bool HandleLowerLeftPoint()
    {
        if (IsTokenAt(lowerLeftPoint))
        {
            MoveTo(lowerPoints[0]);
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

    private void Start()
    {
        totalDist = movedDist = 0;
        canFinish = finished = false;
        previousPositions = new Stack<Vector2>();
        MoveTo(lowerRightPoint);
    }

    private void Update()
    {
        if (finished || IsDoneMoving()) return;
        if (HandleBackwardsMove()) return;

        previousPositions.Push(transform.position);
        
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
