using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public enum boardPointIndex
{
    LowerRight,
    Right1,
    Right2,
    Right3,
    Right4,
    UpperRight,
    Upper1,
    Upper2,
    Upper3,
    Upper4,
    UpperLeft,
    Left1,
    Left2,
    Left3,
    Left4,
    LowerLeft,
    Lower1,
    Lower2,
    Lower3,
    Lower4,
    RightDiag1,
    RightDiag2,
    Center,
    RightDiag3,
    RightDiag4,
    LeftDiag1,
    LeftDiag2,
    LeftDiag3,
    LeftDiag4
}

public class TokenManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> boardPoints;

    [SerializeField] private GameObject tokenPrefab1, tokenPrefab2;
    [SerializeField] private List<Vector2> initialPositions1, initialPositions2;
    [SerializeField] private Vector2 finishedPosition; // TODO: Make an actual finishing position or animation?

    private List<Token> tokens1, tokens2;

    private void Start()
    {
        tokens1 = new List<Token>();
        tokens2 = new List<Token>();

        for (int i = 0; i < initialPositions1.Count; i++)
        {
            var newTokenObject = Instantiate<GameObject>(tokenPrefab1, initialPositions1[i], Quaternion.identity);
            Token newToken = newTokenObject.GetComponent<Token>();
            newToken.initialPosition = initialPositions1[i];
            tokens1.Add(newToken);
        }
        for (int i = 0; i < initialPositions2.Count; i++)
        {
            var newTokenObject = Instantiate<GameObject>(tokenPrefab2, initialPositions2[i], Quaternion.identity);
            Token newToken = newTokenObject.GetComponent<Token>();
            newToken.initialPosition = initialPositions2[i];
            tokens2.Add(newToken);
        }

        ResetToken(tokens1[0]); // TODO: When player selects token, reset that token and then move it
    }

    private void Update()
    {
        if (GetBoardPointIndex(tokens1[0]) == -1) return;
        DebugHandleInput();
    }

    /// <summary>
    /// Moves token to lowerRightPoint and resets previousPosition
    /// </summary>
    /// <param name="token"></param>
    private void ResetToken(Token token)
    {
        StartCoroutine(token.MoveTo(boardPoints[(int)boardPointIndex.LowerRight]));
        token.previousPosition = token.initialPosition;
    }

    /// <summary>
    /// Gets index of the boardPoint that token is on; -1 if none
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    private int GetBoardPointIndex(Token token)
    {
        for (int index = 0; index < boardPoints.Count; index++)
        {
            if (token.IsTokenAt(boardPoints[index])) return index;
        }
        return -1;
    }

    /// <summary>
    /// Gets the next position that token has to go to from current position;
    /// Set isFirstMove to true for the first move of the turn
    /// </summary>
    /// <param name="token"></param>
    /// <param name="isFirstMove"></param>
    /// <returns>nextPosition; if finished, finishedPosition</returns>
    private Vector2 GetNextPosition(Token token, bool isFirstMove)
    {
        int index = GetBoardPointIndex(token);
        if (index == -1) return token.initialPosition; // Exception: token not on any boardPoint
        if (token.canFinish) return finishedPosition;
        if (isFirstMove) // Only on the first move of the turn
        {
            if (index == (int)boardPointIndex.UpperRight)
            {
                return boardPoints[(int)boardPointIndex.RightDiag1].transform.position;
            }
            else if (index == (int)boardPointIndex.UpperLeft)
            {
                return boardPoints[(int)boardPointIndex.LeftDiag1].transform.position;
            }
            else if (index == (int)boardPointIndex.Center)
            {
                return boardPoints[(int)boardPointIndex.LeftDiag3].transform.position;
            }
        }
        if (index == (int)boardPointIndex.Center &&
            token.previousPosition == (Vector2)boardPoints[(int)boardPointIndex.LeftDiag2].transform.position)
            // When token is at centralPoint and the previous position was upper left point of centralPoint
        {
            return boardPoints[(int)boardPointIndex.LeftDiag3].transform.position;
            // Move to lower right point of centralPoint
        }
        if (index == (int)boardPointIndex.LeftDiag2)
        {
            return boardPoints[(int)boardPointIndex.Center].transform.position;
        }
        if (index == (int)boardPointIndex.LeftDiag4)
        {
            return boardPoints[(int)boardPointIndex.LowerRight].transform.position;
        }
        if (index == (int)boardPointIndex.RightDiag4)
        {
            return boardPoints[(int)boardPointIndex.LowerLeft].transform.position;
        }
        if (index == (int)boardPointIndex.Lower4)
        {
            return boardPoints[(int)boardPointIndex.LowerRight].transform.position;
        }
        return boardPoints[index + 1].transform.position;
    }

    /// <summary>
    /// Gets the next position to go to from the current position, but backwards
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    private Vector2 GetNextPositionBackward(Token token)
    {
        if (token.previousPosition != token.initialPosition) // When previousPosition is recorded
        {
            Vector2 nextPosition = token.previousPosition;
            token.previousPosition = token.initialPosition;
            return nextPosition;
        }
        // Else: when previousPosition is initialPosition
        int index = GetBoardPointIndex(token);
        if (index == -1) return token.initialPosition; // Exception: token not on any boardPoint
        if (index == (int)boardPointIndex.LowerRight) // when index is zero
            return boardPoints[(int)boardPointIndex.Lower4].transform.position;
        if (index == (int)boardPointIndex.RightDiag1)
            return boardPoints[(int)boardPointIndex.UpperRight].transform.position;
        if (index == (int)boardPointIndex.LeftDiag1)
            return boardPoints[(int)boardPointIndex.UpperLeft].transform.position;
        if (index == (int)boardPointIndex.LeftDiag3)
        {
            token.isFromLeftDiag = true;
            return boardPoints[(int)boardPointIndex.Center].transform.position;
        }
        if (index == (int)boardPointIndex.Center && token.isFromLeftDiag)
        {
            token.isFromLeftDiag = false;
            return boardPoints[(int)boardPointIndex.LeftDiag2].transform.position;
        }
        return boardPoints[index - 1].transform.position;
    }

    /// <summary>
    /// Checks if token can finish on the next turn after it moves to nextPosition
    /// </summary>
    /// <param name="token"></param>
    /// <param name="nextPosition"></param>
    private void CheckCanFinish(Token token, Vector2 nextPosition)
    {
        if (nextPosition == (Vector2)boardPoints[(int)boardPointIndex.LowerRight].transform.position)
            token.canFinish = true;
        else token.canFinish = false;
    }

    /// <summary>
    /// Actually moves token according to GetNextPosition(); Use with StartCoroutine()
    /// </summary>
    /// <param name="token"></param>
    /// <param name="distance"></param>
    /// <returns></returns>
    private IEnumerator MoveToken(Token token, int distance)
    {
        Vector2 nextPosition;
        if (distance < 0)
        {
            nextPosition = GetNextPositionBackward(token);
            CheckCanFinish(token, nextPosition);
            yield return token.MoveTo(nextPosition);
        }

        for (int i = 0; i < distance; i++)
        {
            nextPosition = GetNextPosition(token, (i == 0) ? true : false);
            token.previousPosition = token.transform.position;
            CheckCanFinish(token, nextPosition);
            if (nextPosition == finishedPosition) token.isFinished = true;
            yield return token.MoveTo(nextPosition);
            if (token.isFinished) break;
        }
    }

    /// <summary>
    /// Starts a coroutine that moves token by distance
    /// </summary>
    /// <param name="token"></param>
    /// <param name="distance"></param>
    private void StartMove(Token token, int distance)
    {
        if (token.isFinished) return;
        StartCoroutine(MoveToken(token, distance));
    }

    /// <summary>
    /// A debug function that handles keyboard input
    /// </summary>
    private void DebugHandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            StartMove(tokens1[0], 1);
            return;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            StartMove(tokens1[0], 2);
            return;
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            StartMove(tokens1[0], 3);
            return;
        }

        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            StartMove(tokens1[0], -1);
            return;
        }
    }
}
