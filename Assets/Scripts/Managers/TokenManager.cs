using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    [SerializeField] private PrepareManager prepareManager;

    public List<GameObject> boardPoints;

    [SerializeField] private GameObject tokenPrefab1, tokenPrefab2, emptyTokenPrefab;
    public List<Vector2> initialPositions1, initialPositions2;
    public Vector2 finishedPosition; // TODO: Make an actual finishing position or animation?

    public List<Token> tokens1, tokens2;

    private int winPlayer;

    private bool readyToKeyPlayer = true;
    private bool readyToKeyNumber = false;
    private bool readyToButton = false;

    private PrepareButton selectedPrepareButton = null;

    private void Start()
    {
        tokens1 = new List<Token>();
        tokens2 = new List<Token>();

        for (int i = 0; i < initialPositions1.Count; i++)
        {
            var newTokenObject = Instantiate<GameObject>(tokenPrefab1, initialPositions1[i], Quaternion.identity);
            Token newToken = newTokenObject.GetComponent<Token>();
            newToken.boardPointIndex = BoardPointIndex.Initial;
            newToken.initialPosition = initialPositions1[i];
            tokens1.Add(newToken);
        }
        for (int i = 0; i < initialPositions2.Count; i++)
        {
            var newTokenObject = Instantiate<GameObject>(tokenPrefab2, initialPositions2[i], Quaternion.identity);
            Token newToken = newTokenObject.GetComponent<Token>();
            newToken.boardPointIndex = BoardPointIndex.Initial;
            newToken.initialPosition = initialPositions2[i];
            tokens2.Add(newToken);
        }

        foreach (Token token1 in tokens1) StartCoroutine(ResetToken(token1)); // reset tokens1
        foreach (Token token2 in tokens2) StartCoroutine(ResetToken(token2)); // reset tokens2

        prepareManager.ResetSettings();
    }

    private void Update()
    {
        DebugHandleInput();
    }

    public int GetPlayer(Token token)
    {
        if (tokens1.Contains(token)) return 1;
        return 2;
    }

    public List<Token> GetTokens(int player)
    {
        if (player == 1) return tokens1;
        return tokens2;
    }

    /// <summary>
    /// Finds all tokens that are stacked with thisToken and stacks them onto thisToken
    /// </summary>
    /// <param name="thisToken"></param>
    public void FindAllStacked(Token thisToken)
    {
        int player = GetPlayer(thisToken);
        List<Token> tokens = GetTokens(player);
        int i = 0;
        while (i < tokens.Count)
        {
            if (thisToken != tokens[i] && thisToken.IsStacked(tokens[i]))
            {
                thisToken.Stack(tokens[i]);
                tokens.RemoveAt(i);
            }
            else i++;
        }
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
        token.visitedCorners.Clear();
        token.Unstack();
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
        if (token.boardPointIndex == BoardPointIndex.LowerRight) return BoardPointIndex.Finished;
        if (isFirstMove)
        {
            if (token.boardPointIndex == BoardPointIndex.UpperRight) return BoardPointIndex.RightDiag1;
            if (token.boardPointIndex == BoardPointIndex.UpperLeft) return BoardPointIndex.LeftDiag1;
            if (token.boardPointIndex == BoardPointIndex.Center) return BoardPointIndex.LeftDiag3;
        }
        if (token.boardPointIndex == BoardPointIndex.Center &&
            token.visitedCorners.Contains(BoardPointIndex.UpperLeft)) return BoardPointIndex.LeftDiag3;
        if (token.boardPointIndex == BoardPointIndex.LeftDiag2) return BoardPointIndex.Center;
        if (token.boardPointIndex == BoardPointIndex.LeftDiag4) return BoardPointIndex.LowerRight;
        if (token.boardPointIndex == BoardPointIndex.RightDiag4) return BoardPointIndex.LowerLeft;
        if (token.boardPointIndex == BoardPointIndex.Lower4) return BoardPointIndex.LowerRight;
        return token.boardPointIndex + 1;
    }

    /// <summary>
    /// Gets the next BoardPointIndex to go to from the current position, but backwards;
    /// Only for singular token
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public BoardPointIndex GetPreviousIndex(Token token)
    {
        if (token.boardPointIndex == BoardPointIndex.Initial) return BoardPointIndex.Initial;
        if (token.boardPointIndex == BoardPointIndex.Finished) return BoardPointIndex.Finished;
        if (token.boardPointIndex == BoardPointIndex.LowerRight)
        {
            if (token.visitedCorners.Contains(BoardPointIndex.LowerLeft))
                return BoardPointIndex.Lower4;
            if (token.visitedCorners.Contains(BoardPointIndex.Center))
                return BoardPointIndex.LeftDiag4;
            return BoardPointIndex.Initial;            
        }
        if (token.boardPointIndex == BoardPointIndex.LowerLeft)
        {
            if (token.visitedCorners.Contains(BoardPointIndex.Center)) return BoardPointIndex.RightDiag4;
            return BoardPointIndex.Left4;
        }
        if (token.boardPointIndex == BoardPointIndex.RightDiag1) return BoardPointIndex.UpperRight;
        if (token.boardPointIndex == BoardPointIndex.LeftDiag1) return BoardPointIndex.UpperLeft;
        if (token.boardPointIndex == BoardPointIndex.LeftDiag3) return BoardPointIndex.Center;
        if (token.boardPointIndex == BoardPointIndex.Center &&
            token.visitedCorners.Contains(BoardPointIndex.UpperLeft))
            return BoardPointIndex.LeftDiag2;
        return token.boardPointIndex - 1;
    }

    /// <summary>
    /// Gets every possible BoardPointIndex to go to from the current position backwards
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public List<BoardPointIndex> GetPreviousIndices(Token token)
    {
        List<BoardPointIndex> indices = new List<BoardPointIndex> { GetPreviousIndex(token) };
        foreach (Token stackedToken in token.stackedTokens)
        {
            BoardPointIndex tempIndex = GetPreviousIndex(stackedToken);
            if (!indices.Contains(tempIndex)) indices.Add(tempIndex);
        }

        foreach (BoardPointIndex index in indices) Debug.Log(index);
        return indices;
    }


    public IEnumerator MoveTokenByOne(Token token, bool isFirstMove)
    {
        BoardPointIndex nextBoardPointIndex = GetNextIndex(token, isFirstMove);
        token.PushVisitedCorners(nextBoardPointIndex);
        foreach (Token stackedToken in token.stackedTokens) {
            stackedToken.PushVisitedCorners(nextBoardPointIndex);
            InstantMoveTokenTo(stackedToken, nextBoardPointIndex);
        }
        yield return MoveTokenTo(token, nextBoardPointIndex);
    }

    public void InstantMoveTokenByOne(Token token, bool isFirstMove)
    {
        BoardPointIndex nextBoardPointIndex = GetNextIndex(token, isFirstMove);
        token.PushVisitedCorners(nextBoardPointIndex);
        foreach (Token stackedToken in token.stackedTokens)
        {
            stackedToken.PushVisitedCorners(nextBoardPointIndex);
            InstantMoveTokenTo(stackedToken, nextBoardPointIndex);
        }
        InstantMoveTokenTo(token, nextBoardPointIndex);
    }

    /// <summary>
    /// Moves token backwards to boardPointIndex
    /// </summary>
    /// <param name="token"></param>
    /// <param name="boardPointIndex"></param>
    /// <returns></returns>
    public IEnumerator MoveTokenBackwardsTo(Token token, BoardPointIndex boardPointIndex)
    {
        if (boardPointIndex == BoardPointIndex.Initial)
        {
            yield return ResetToken(token);
            yield break;
        }
        switch (boardPointIndex)
        {
            case BoardPointIndex.Lower4:
                token.PopVisitedCornersUntil(BoardPointIndex.LowerLeft);
                break;
            case BoardPointIndex.LeftDiag4:
            case BoardPointIndex.RightDiag4:
                token.PopVisitedCornersUntil(BoardPointIndex.Center);
                break;
            case BoardPointIndex.Right4:
                token.PopVisitedCornersUntil(BoardPointIndex.LowerRight);
                break;
            case BoardPointIndex.Upper4:
            case BoardPointIndex.RightDiag2:
                token.PopVisitedCornersUntil(BoardPointIndex.UpperRight);
                break;
            case BoardPointIndex.Left4:
            case BoardPointIndex.LeftDiag2:
                token.PopVisitedCornersUntil(BoardPointIndex.UpperLeft);
                break;
        }
        yield return MoveTokenTo(token, boardPointIndex);
        FindAllStacked(token);
    }

    /// <summary>
    /// Moves token backwards, with a route specified by index
    /// </summary>
    /// <param name="token"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    public IEnumerator MoveTokenByOneBackwards(Token token, int index)
    {
        BoardPointIndex previousBoardPointIndex = GetPreviousIndices(token)[index];
        yield return MoveTokenBackwardsTo(token, previousBoardPointIndex);
    }

    /// <summary>
    /// Gets the BoardPointIndex that token would move to after moving token by distance
    /// </summary>
    /// <param name="token"></param>
    /// <param name="distance"></param>
    /// <returns></returns>
    public BoardPointIndex GetIndexAfterMove(Token token, int distance)
    {
        var tempTokenObject = Instantiate<GameObject>(emptyTokenPrefab, token.transform.position, Quaternion.identity);
        Token tempToken = tempTokenObject.GetComponent<Token>();
        tempToken.visitedCorners = new Stack<BoardPointIndex>(token.visitedCorners);
        tempToken.boardPointIndex = token.boardPointIndex;

        if (tempToken.boardPointIndex == BoardPointIndex.Initial)
            InstantMoveTokenTo(tempToken, BoardPointIndex.Right1);
        else InstantMoveTokenByOne(tempToken, true);

        for (int i = 0; i < distance-1; i++)
        {
            if (tempToken.boardPointIndex == BoardPointIndex.Finished) break;
            InstantMoveTokenByOne(tempToken, false);
        }
        BoardPointIndex boardPointIndex = tempToken.boardPointIndex;
        Destroy(tempTokenObject);
        return boardPointIndex;
    }

    /// <summary>
    /// Actually moves token by distance using MoveTokenByOne();
    /// </summary>
    /// <param name="token"></param>
    /// <param name="distance"></param>
    public IEnumerator MoveToken(Token token, int distance)
    {
        if (token.boardPointIndex == BoardPointIndex.Initial)
        {
            yield return MoveTokenTo(token, BoardPointIndex.LowerRight);
            yield return MoveTokenTo(token, BoardPointIndex.Right1);
        }
        else yield return MoveTokenByOne(token, true);

        for (int i = 0; i < distance-1; i++)
        {
            if (token.boardPointIndex == BoardPointIndex.Finished) break;
            yield return MoveTokenByOne(token, false);
        }
        FindAllStacked(token);
    }

    /// <summary>
    /// Starts a coroutine that moves token by distance
    /// </summary>
    /// <param name="token"></param>
    /// <param name="distance"></param>
    public void StartMove(Token token, int distance)
    {
        StartCoroutine(MoveToken(token, distance));
    }

    /// <summary>
    /// Starts a coroutine that moves token backwards to boardPointIndex
    /// </summary>
    /// <param name="token"></param>
    /// <param name="boardPointIndex"></param>
    public void StartMoveBackwards(Token token, BoardPointIndex boardPointIndex)
    {
        StartCoroutine(MoveTokenBackwardsTo(token, boardPointIndex));
    }

    /// <summary>
    /// Starts a coroutine that moves token backwards;
    /// Select a route with index
    /// </summary>
    /// <param name="token"></param>
    /// <param name="index"></param>
    public void StartMoveBackwards(Token token, int index)
    {
        StartCoroutine(MoveTokenByOneBackwards(token, index));
    }

    /// <summary>
    /// A debug function that handles keyboard input
    /// </summary>
    private void DebugHandleInput()
    {
        int steps = 0;

        if (readyToKeyPlayer)
        {
            readyToKeyPlayer = false;
            readyToKeyNumber = true;

            if (Input.GetKeyDown(KeyCode.A)) winPlayer = 1;
            else if (Input.GetKeyDown(KeyCode.B)) winPlayer = 2;
            else // if A or B is not pressed, restore settings
            {
                readyToKeyPlayer = true;
                readyToKeyNumber = false;
            }
        }

        if (readyToKeyNumber)
        {
            readyToKeyNumber = false;
            readyToButton = true;

            if (Input.GetKeyDown(KeyCode.Alpha1)) steps = 1;
            else if (Input.GetKeyDown(KeyCode.Alpha2)) steps = 2;
            else if (Input.GetKeyDown(KeyCode.Alpha3)) steps = 3;
            else if (Input.GetKeyDown(KeyCode.Alpha4)) steps = 4;
            else if (Input.GetKeyDown(KeyCode.Alpha5)) steps = 5;
            else if (Input.GetKeyDown(KeyCode.Alpha0)) steps = -1;
            else // if 0 ~ 5 is not pressed, restore settings
            {
                readyToKeyNumber = true;
                readyToButton = false;
            }
        }
        

        if (readyToButton == true)
        {
            readyToButton = false;

            prepareManager.ActivatePrepareButtons(winPlayer, steps);
        }
    }

    public void FailActivePrepareButton() // when steps is -1 and there are no tokens able to move
    {
        readyToKeyPlayer = true; // go to DebugHandleInput step to choose player
    }

    public void OnClickPrepareButton(PrepareButton prepareButton, Token token, int steps)
    {
        prepareManager.DeactivateMoveButtons();

        if (selectedPrepareButton == prepareButton) selectedPrepareButton = null; // select selected prepareButton
        else
        {
            selectedPrepareButton = prepareButton;

            prepareManager.ActivateMoveButton(token, steps);
        }
    }

    public void OnClickMoveButton(Token token, int steps)
    {
        readyToKeyPlayer = true; // go to DebugHandleInput step to choose player

        prepareManager.DeactivatePrepareButtons();
        prepareManager.DeactivateMoveButtons();

        selectedPrepareButton = null;
        if (steps < 0) StartMoveBackwards(token, 0);
        else StartMove(token, steps);
    }
}