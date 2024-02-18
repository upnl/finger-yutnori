using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
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

    [SerializeField] private GameObject tokenPrefab1, tokenPrefab2, emptyTokenPrefab;
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

        StartCoroutine(ResetToken(tokens1[0])); // TODO: When player selects token, reset that token and then move it
    }

    private void Update()
    {
        if (tokens1[0].boardPointIndex == BoardPointIndex.Finished) return;
        DebugHandleInput();
    }

    /// <summary>
    /// Moves token to boardPointIndex and sets token.boardPointIndex to boardPointIndex
    /// </summary>
    /// <param name="token"></param>
    /// <param name="boardPointIndex"></param>
    /// <returns></returns>
    public IEnumerator MoveTokenTo(Token token, BoardPointIndex boardPointIndex)
    {
        token.boardPointIndex = boardPointIndex;
        if (boardPointIndex == BoardPointIndex.Initial)
        {
            yield return token.MoveTo(token.initialPosition);
        }
        else if (boardPointIndex == BoardPointIndex.Finished)
        {
            yield return token.MoveTo(finishedPosition);
        }
        else yield return token.MoveTo(boardPoints[(int)token.boardPointIndex]);
    }

    public void InstantMoveTokenTo(Token token, BoardPointIndex boardPointIndex)
    {
        token.boardPointIndex = boardPointIndex;
        if (boardPointIndex == BoardPointIndex.Initial)
            token.InstantMoveTo(token.initialPosition);
        else if (boardPointIndex == BoardPointIndex.Finished)
            token.InstantMoveTo(finishedPosition);
        else token.InstantMoveTo(boardPoints[(int)token.boardPointIndex]);
    }

    /// <summary>
    /// Moves token to initialPosition and resets canFinish, hasLooped, routeType;
    /// Use with StartCoroutine()
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public IEnumerator ResetToken(Token token)
    {
        token.canFinish = false;
        token.hasLooped = false;
        token.routeType = 0;
        yield return MoveTokenTo(token, BoardPointIndex.Initial);
    }

    /// <summary>
    /// Gets the next BoardPointIndex that token has to go to from current position;
    /// Set isFirstMove to true for the first move of the turn
    /// </summary>
    /// <param name="token"></param>
    /// <param name="isFirstMove"></param>
    /// <returns></returns>
    public BoardPointIndex GetNextIndex(Token token, bool isFirstMove)
    {
        if (token.boardPointIndex == BoardPointIndex.Initial) return BoardPointIndex.LowerRight;
        if (token.boardPointIndex == BoardPointIndex.Finished) return BoardPointIndex.Finished;
        if (token.boardPointIndex == BoardPointIndex.LowerRight)
        {
            if (token.canFinish) return BoardPointIndex.Finished;
            return BoardPointIndex.Right1;
        }
        if (isFirstMove)
        {
            if (token.boardPointIndex == BoardPointIndex.UpperRight) return BoardPointIndex.RightDiag1;
            if (token.boardPointIndex == BoardPointIndex.UpperLeft) return BoardPointIndex.LeftDiag1;
            if (token.boardPointIndex == BoardPointIndex.Center) return BoardPointIndex.LeftDiag3;
        }
        if (token.boardPointIndex == BoardPointIndex.Center && token.routeType == 1) return BoardPointIndex.LeftDiag3;
        if (token.boardPointIndex == BoardPointIndex.LeftDiag2) return BoardPointIndex.Center;
        if (token.boardPointIndex == BoardPointIndex.LeftDiag4) return BoardPointIndex.LowerRight;
        if (token.boardPointIndex == BoardPointIndex.RightDiag4) return BoardPointIndex.LowerLeft;
        if (token.boardPointIndex == BoardPointIndex.Lower4) return BoardPointIndex.LowerRight;
        return token.boardPointIndex + 1;
    }

    /// <summary>
    /// Gets the next BoardPointIndex to go to from the current position, but backwards
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public BoardPointIndex GetPreviousIndex(Token token)
    {
        if (token.boardPointIndex == BoardPointIndex.Initial) return BoardPointIndex.Initial;
        if (token.boardPointIndex == BoardPointIndex.Finished) return BoardPointIndex.Finished;
        if (token.boardPointIndex == BoardPointIndex.LowerRight)
        {
            if (token.hasLooped)
            {
                if (token.routeType == 1 || token.routeType == 3) return BoardPointIndex.LeftDiag4;
                return BoardPointIndex.Lower4;
            }
            return BoardPointIndex.Initial;
        }
        if (token.boardPointIndex == BoardPointIndex.LowerLeft)
        {
            if (token.routeType == 0) return BoardPointIndex.Left4;
            return BoardPointIndex.RightDiag4;
        }
        if (token.boardPointIndex == BoardPointIndex.RightDiag1) return BoardPointIndex.UpperRight;
        if (token.boardPointIndex == BoardPointIndex.LeftDiag1) return BoardPointIndex.UpperLeft;
        if (token.boardPointIndex == BoardPointIndex.LeftDiag3) return BoardPointIndex.Center;
        if (token.boardPointIndex == BoardPointIndex.Center)
        {
            if (token.routeType == 1) return BoardPointIndex.LeftDiag2;
            return BoardPointIndex.RightDiag2;
        }
        return token.boardPointIndex - 1;
    }

    public IEnumerator MoveTokenByOne(Token token, bool isFirstMove)
    {
        int previousRouteType = token.routeType;
        BoardPointIndex nextBoardPointIndex = GetNextIndex(token, isFirstMove);
        token.hasLooped = (nextBoardPointIndex == BoardPointIndex.LowerRight);
        switch (nextBoardPointIndex)
        {
            case BoardPointIndex.Right1:
                token.canFinish = true;
                break;
            case BoardPointIndex.RightDiag1:
            case BoardPointIndex.RightDiag3:
                token.routeType = 2;
                break;
            case BoardPointIndex.LeftDiag1:
                token.routeType = 1;
                break;
            case BoardPointIndex.LeftDiag3:
                if (previousRouteType == 2) token.routeType = 3;
                break;
        }
        yield return MoveTokenTo(token, nextBoardPointIndex);
    }
    public void InstantMoveTokenByOne(Token token, bool isFirstMove)
    {
        int previousRouteType = token.routeType;
        BoardPointIndex nextBoardPointIndex = GetNextIndex(token, isFirstMove);
        token.hasLooped = (nextBoardPointIndex == BoardPointIndex.LowerRight);
        switch (nextBoardPointIndex)
        {
            case BoardPointIndex.Right1:
                token.canFinish = true;
                break;
            case BoardPointIndex.RightDiag1:
            case BoardPointIndex.RightDiag3:
                token.routeType = 2;
                break;
            case BoardPointIndex.LeftDiag1:
                token.routeType = 1;
                break;
            case BoardPointIndex.LeftDiag3:
                if (previousRouteType == 2) token.routeType = 3;
                break;
        }
        InstantMoveTokenTo(token, nextBoardPointIndex);
    }

    public IEnumerator MoveTokenByOneBackwards(Token token)
    {
        BoardPointIndex previousBoardPointIndex = GetPreviousIndex(token);
        if (previousBoardPointIndex == BoardPointIndex.Initial)
        {
            yield return ResetToken(token);
            yield break;
        }
        if (previousBoardPointIndex == BoardPointIndex.UpperRight || previousBoardPointIndex == BoardPointIndex.UpperLeft)
            token.routeType = 0;
        if (previousBoardPointIndex == BoardPointIndex.LowerRight)
            token.canFinish = true;
        token.hasLooped = false;
        yield return MoveTokenTo(token, previousBoardPointIndex);
    }

    /// <summary>
    /// Gets the BoardPointIndex that token would move to after moving token by distance
    /// </summary>
    /// <param name="token"></param>
    /// <param name="distance"></param>
    /// <returns></returns>
    public BoardPointIndex GetIndexAfterMove(Token token, int distance)
    {
        if (distance < 0)
        {
            if (token.boardPointIndex == BoardPointIndex.Initial) return BoardPointIndex.Initial;
            return GetPreviousIndex(token);
        }
        var tempTokenObject = Instantiate<GameObject>(emptyTokenPrefab, token.transform.position, Quaternion.identity);
        Token tempToken = tempTokenObject.GetComponent<Token>();
        tempToken.canFinish = token.canFinish;
        tempToken.hasLooped = token.hasLooped;
        tempToken.routeType = token.routeType;
        tempToken.boardPointIndex = token.boardPointIndex;

        if (tempToken.boardPointIndex == BoardPointIndex.Initial)
            InstantMoveTokenTo(tempToken, BoardPointIndex.LowerRight);
        for (int i = 0; i < distance; i++)
        {
            InstantMoveTokenByOne(tempToken, (i == 0));
            if (tempToken.boardPointIndex == BoardPointIndex.Finished) break;
        }
        BoardPointIndex boardPointIndex = tempToken.boardPointIndex;
        Destroy(tempTokenObject);
        return boardPointIndex;
    }

    /// <summary>
    /// Actually moves token by distance using MoveTokenByOne();
    /// Use negative distance for backward movement
    /// </summary>
    /// <param name="token"></param>
    /// <param name="distance"></param>
    public IEnumerator MoveToken(Token token, int distance)
    {
        if (distance < 0)
        {
            if (token.boardPointIndex != BoardPointIndex.Initial)
                yield return MoveTokenByOneBackwards(token);
            yield break;
        }
        if (token.boardPointIndex == BoardPointIndex.Initial)
            yield return MoveTokenTo(token, BoardPointIndex.LowerRight);

        for (int i = 0; i < distance; i++)
        {
            yield return MoveTokenByOne(token, (i == 0));
            if (token.boardPointIndex == BoardPointIndex.Finished) break;
        }
    }

    /// <summary>
    /// Starts a coroutine that moves token by distance
    /// </summary>
    /// <param name="token"></param>
    /// <param name="distance"></param>
    public void StartMove(Token token, int distance)
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
