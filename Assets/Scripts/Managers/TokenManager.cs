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
    [SerializeField] private PrepareButtonManager prepareButtonManager;
    public List<GameObject> boardPoints;

    [SerializeField] private GameObject tokenPrefab1, tokenPrefab2;
    public List<Vector2> initialPositions1, initialPositions2;

    public List<Token> tokens1, tokens2;

    private void Start()
    {
        tokens1 = new List<Token>();
        tokens2 = new List<Token>();

        for (int i = 0; i < initialPositions1.Count; i++)
        {
            var newToken = Instantiate<GameObject>(tokenPrefab1, initialPositions1[i], Quaternion.identity);
            tokens1.Add(newToken.GetComponent<Token>());
        }
        for (int i = 0; i < initialPositions1.Count; i++)
        {
            var newToken = Instantiate<GameObject>(tokenPrefab2, initialPositions2[i], Quaternion.identity);
            tokens2.Add(newToken.GetComponent<Token>());
        }

        prepareButtonManager.GetInfo();
        prepareButtonManager.ActivePrepareButtons();
        //ResetToken(tokens1[0]); PRE
    }

    private void Update()
    {
        //if (GetBoardPointIndex(tokens1[0]) == -1) return; PRE
        //DebugHandleInput(); PRE
    }

    /// <summary>
    /// Clears previousPositions of token and moves it to lowerRightPoint
    /// </summary>
    /// <param name="token"></param>
    private void ResetToken(Token token)
    {
        token.ClearPreviousPositions();
        StartCoroutine(token.MoveTo(boardPoints[(int)boardPointIndex.LowerRight]));
    }

    /// <summary>
    /// Gets index of the boardPoint that token is on; -1 if none
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public int GetBoardPointIndex(Token token)
    {
        for (int index = 0; index < boardPoints.Count; index++)
        {
            if (token.IsTokenAt(boardPoints[index])) return index;
        }
        return -1;
    }

    /// <summary>
    /// Gets the next position that token has to go to from current position
    /// </summary>
    /// <param name="token"></param>
    /// <param name="isFirstMove"></param>
    /// <returns>nextPosition; if finished, finishedPosition</returns>
    private Vector2 GetNextPosition(Token token, bool isFirstMove)
    {
        int index = GetBoardPointIndex(token);
        if (index == -1) return Vector2.zero;
        if (index == (int)boardPointIndex.LowerRight && 
            token.CountPreviousPositions() != 0 && (
            token.PeekPreviousPositions() == (Vector2)boardPoints[(int)boardPointIndex.Lower4].transform.position ||
            token.PeekPreviousPositions() == (Vector2)boardPoints[(int)boardPointIndex.LeftDiag4].transform.position))
        {
            token.IsFinished = true;
            Vector2 finishedPosition = boardPoints[(int)boardPointIndex.LowerRight].transform.position;
            finishedPosition.y -= 15f;
            return finishedPosition;
        }

        if (isFirstMove)
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
            token.PeekPreviousPositions() == (Vector2)boardPoints[(int)boardPointIndex.LeftDiag2].transform.position)
        {
            return boardPoints[(int)boardPointIndex.LeftDiag3].transform.position;
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
    /// Actually moves token according to GetNextPosition(); Use with StartCoroutine()
    /// </summary>
    /// <param name="token"></param>
    /// <param name="distance"></param>
    /// <returns></returns>
    private IEnumerator MoveToken(Token token, int distance)
    {
        if (distance < 0)
        {
            if (token.CountPreviousPositions() == 0) yield return null;
            yield return token.MoveTo(token.PopPreviousPositions());
        }

        for (int i = 0; i < distance; i++)
        {
            Vector2 nextPosition = GetNextPosition(token, (i == 0) ? true : false);
            token.RecordPosition();
            yield return token.MoveTo(nextPosition);
        }
    }

    /// <summary>
    /// Starts a coroutine that moves token by distance
    /// </summary>
    /// <param name="token"></param>
    /// <param name="distance"></param>
    private void StartMove(Token token, int distance)
    {
        if (token.IsFinished) return;
        if (distance < 0 && token.CountPreviousPositions() == 0) return;
        StartCoroutine(MoveToken(token, distance));
    }

    /// <summary>
    /// A debug function that handles keyboard input
    /// </summary>
    public void GetTokenSteps(Token curToken, int steps) // PRE : DebugHandleInput()
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
        StartMove(curToken, steps);
    }
}
