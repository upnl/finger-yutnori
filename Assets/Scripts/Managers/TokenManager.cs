using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public enum BoardPointIndex
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
    LeftDiag4,
    Initial,
    Finished
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
            tokens1.Add(newToken);
        }
        for (int i = 0; i < initialPositions2.Count; i++)
        {
            var newTokenObject = Instantiate<GameObject>(tokenPrefab2, initialPositions2[i], Quaternion.identity);
            Token newToken = newTokenObject.GetComponent<Token>();
            tokens2.Add(newToken);
        }

        ResetToken(tokens1[0]); // TODO: When player selects token, reset that token and then move it
    }

    private void Update()
    {
        if (tokens1[0].boardPointIndex == BoardPointIndex.Finished) return;
        DebugHandleInput();
    }

    private IEnumerator MoveTokenTo(Token token, BoardPointIndex boardPointIndex)
    {
        token.boardPointIndex = boardPointIndex;
        if (boardPointIndex == BoardPointIndex.Finished)
        {
            yield return token.MoveTo(finishedPosition);
        }
        else yield return token.MoveTo(boardPoints[(int)token.boardPointIndex]);
    }

    /// <summary>
    /// Moves token to lowerRightPoint and resets previousPosition
    /// </summary>
    /// <param name="token"></param>
    private void ResetToken(Token token)
    {
        token.canFinish = false;
        token.routeType = 0;
        StartCoroutine(MoveTokenTo(token, BoardPointIndex.LowerRight));
    }

    /// <summary>
    /// Gets the next position that token has to go to from current position;
    /// Set isFirstMove to true for the first move of the turn
    /// </summary>
    /// <param name="token"></param>
    /// <param name="isFirstMove"></param>
    /// <returns>nextPosition; if finished, finishedPosition</returns>
    private IEnumerator MoveTokenByOne(Token token, bool isFirstMove)
    {
        if (token.boardPointIndex == BoardPointIndex.LowerRight)
        {
            if (token.canFinish)
                yield return MoveTokenTo(token, BoardPointIndex.Finished);
            else
            {
                token.canFinish = true;
                yield return MoveTokenTo(token, BoardPointIndex.Right1);
            }
        }
        else
        {
            if (isFirstMove) // Only on the first move of the turn
            {
                if (token.boardPointIndex == BoardPointIndex.UpperRight)
                {
                    token.routeType = 2;
                    yield return MoveTokenTo(token, BoardPointIndex.RightDiag1);
                }
                else if (token.boardPointIndex == BoardPointIndex.UpperLeft)
                {
                    token.routeType = 1;
                    yield return MoveTokenTo(token, BoardPointIndex.LeftDiag1);
                }
                else if (token.boardPointIndex == BoardPointIndex.Center)
                {
                    if (token.routeType == 2) // From rightDiag
                    {
                        token.routeType = 3;
                    }
                    yield return MoveTokenTo(token, BoardPointIndex.LeftDiag3);
                }
                else yield return MoveTokenByOne(token, false);
            }
            else if (token.boardPointIndex == BoardPointIndex.Center && token.routeType == 1)
            {
                yield return MoveTokenTo(token, BoardPointIndex.LeftDiag3);
            }
            else if (token.boardPointIndex == BoardPointIndex.LeftDiag2)
            {
                yield return MoveTokenTo(token, BoardPointIndex.Center);
            }
            else if (token.boardPointIndex == BoardPointIndex.LeftDiag4)
            {
                yield return MoveTokenTo(token, BoardPointIndex.LowerRight);
            }
            else if (token.boardPointIndex == BoardPointIndex.RightDiag4)
            {
                yield return MoveTokenTo(token, BoardPointIndex.LowerLeft);
            }
            else if (token.boardPointIndex == BoardPointIndex.Lower4)
            {
                yield return MoveTokenTo(token, BoardPointIndex.LowerRight);
            }
            else yield return MoveTokenTo(token, token.boardPointIndex + 1);
        }
    }

    /// <summary>
    /// Gets the next position to go to from the current position, but backwards
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    private IEnumerator MoveTokenByOneBackwards(Token token)
    {
        if (token.boardPointIndex == BoardPointIndex.LowerRight)
        {
            if (token.routeType == 1 || token.routeType == 3)
            {
                yield return MoveTokenTo(token, BoardPointIndex.LeftDiag4);
            }
            else yield return MoveTokenTo(token, BoardPointIndex.Lower4);
        }
        else if (token.boardPointIndex == BoardPointIndex.LowerLeft)
        {
            if (token.routeType == 0)
            {
                yield return MoveTokenTo(token, BoardPointIndex.Left4);
            }
            else yield return MoveTokenTo(token, BoardPointIndex.RightDiag4);
        }
        else if (token.boardPointIndex == BoardPointIndex.RightDiag1)
        {
            yield return MoveTokenTo(token, BoardPointIndex.UpperRight);
        }
        else if (token.boardPointIndex == BoardPointIndex.LeftDiag1)
        {
            yield return MoveTokenTo(token, BoardPointIndex.UpperLeft);
        }
        else if (token.boardPointIndex == BoardPointIndex.LeftDiag3)
        {
            yield return MoveTokenTo(token, BoardPointIndex.Center);
        }
        else if (token.boardPointIndex == BoardPointIndex.Center)
        {
            if (token.routeType == 1)
            {
                yield return MoveTokenTo(token, BoardPointIndex.LeftDiag2);
            }
            else if (token.routeType == 2 || token.routeType == 3)
            {
                yield return MoveTokenTo(token, BoardPointIndex.RightDiag2);
            }
        }
        else yield return MoveTokenTo(token, token.boardPointIndex - 1);
    }

    /// <summary>
    /// Actually moves token by distance using MoveTokenByOne();
    /// Use negative distance for backward movement
    /// </summary>
    /// <param name="token"></param>
    /// <param name="distance"></param>
    private IEnumerator MoveToken(Token token, int distance)
    {
        if (distance < 0)
        {
            yield return MoveTokenByOneBackwards(token);
        }

        for (int i = 0; i < distance; i++)
        {
            yield return MoveTokenByOne(token, (i == 0) ? true : false);
            if (token.boardPointIndex == BoardPointIndex.Finished) break;
        }
    }

    /// <summary>
    /// Starts a coroutine that moves token by distance
    /// </summary>
    /// <param name="token"></param>
    /// <param name="distance"></param>
    private void StartMove(Token token, int distance)
    {
        if (token.boardPointIndex == BoardPointIndex.Finished) return;
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
