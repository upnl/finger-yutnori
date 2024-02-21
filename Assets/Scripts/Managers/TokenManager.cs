using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
    [SerializeField] private Button RestartButton, NewGameButton;

    public List<GameObject> boardPoints;

    [SerializeField] private GameObject tokenPrefab1, tokenPrefab2, emptyTokenPrefab;
    public List<Vector2> initialPositions1, initialPositions2;
    public List<Vector2> finishedPositions1, finishedPositions2;

    public List<Token> tokens1, tokens2;

    private int score1, score2;

    public List<Token> winTokenList;
    /// <summary>
    /// 0 : wait to choose player
    /// 1 : wait to choose steps
    /// 2 : PrepareManager.ActivatePreviews()
    /// 3 : wait token click
    /// 4 : wait back button click
    /// </summary>
    private int keyStep = 0;
    private int steps;

    private void Start()
    {
        tokens1 = new List<Token>();
        tokens2 = new List<Token>();

        score1 = score2 = 0;

        for (int i = 0; i < initialPositions1.Count; i++)
        {
            var newTokenObject = Instantiate<GameObject>(tokenPrefab1, initialPositions1[i], Quaternion.identity);
            Token newToken = newTokenObject.GetComponent<Token>();
            newToken.boardPointIndex = BoardPointIndex.Initial;
            newToken.initialPosition = initialPositions1[i];
            newToken.finishedPosition = finishedPositions1[i];
            tokens1.Add(newToken);
        }
        for (int i = 0; i < initialPositions2.Count; i++)
        {
            var newTokenObject = Instantiate<GameObject>(tokenPrefab2, initialPositions2[i], Quaternion.identity);
            Token newToken = newTokenObject.GetComponent<Token>();
            newToken.boardPointIndex = BoardPointIndex.Initial;
            newToken.initialPosition = initialPositions2[i];
            newToken.finishedPosition = finishedPositions2[i];
            tokens2.Add(newToken);
        }

        foreach (Token token in tokens1) ResetToken(token); // reset tokens1
        foreach (Token token in tokens2) ResetToken(token); // reset tokens2

        prepareManager.ResetSettings();
    }

    private void Update()
    {
        if (DecideWinner() != 0)
        {
            Debug.Log(DecideWinner() + " won!");
            //UnityEditor.EditorApplication.isPlaying = false;

            RestartButton.gameObject.SetActive(true);
            NewGameButton.gameObject.SetActive(true);
        } 
        DebugHandleInput();
    }

    public int GetPlayer(Token token)
    {
        return tokens1.Contains(token) ? 1 : 2;
    }

    public int GetOpponent(Token token)
    {
        return tokens1.Contains(token) ? 2 : 1;
    }

    public List<Token> GetTokens(int player)
    {
        return (player == 1) ? tokens1 : tokens2;
    }

    /// <summary>
    /// Finds a token in player's team that is stackable with thisToken
    /// </summary>
    /// <param name="thisToken"></param>
    public Token FindStackable(Token thisToken, int player)
    {
        foreach (Token token in GetTokens(player))
        {
            if (token != thisToken && token.IsStackable(thisToken))
                return token;
        }
        return null;
    }

    public void StackOnto(Token thisToken, Token otherToken)
    {
        thisToken.Stack(otherToken);
        GetTokens(GetPlayer(thisToken)).Remove(otherToken);
    }

    public void HandleStackable(Token token)
    {
        Token stackableToken = FindStackable(token, GetPlayer(token));
        if (stackableToken != null) StackOnto(token, stackableToken);

        stackableToken = FindStackable(token, GetOpponent(token));
        if (stackableToken != null)
        {
            token.AttackTrigger = true;
            ResetToken(stackableToken);
            StartMove(token, 1);
        }
    }

    /// <summary>
    /// Moves token to initialPosition and resets visitedCorners and stackedTokens;
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public void ResetToken(Token token)
    {
        foreach (Token stackedToken in token.Unstack())
        {
            stackedToken.visitedCorners.Clear();
            StartCoroutine(MoveTokenTo(stackedToken, BoardPointIndex.Initial));
            GetTokens(GetPlayer(token)).Add(stackedToken);
        }
        token.visitedCorners.Clear();
        StartCoroutine(MoveTokenTo(token, BoardPointIndex.Initial));
    }

    public IEnumerator FinishToken(Token token)
    {
        foreach (Token stackedToken in token.Unstack())
        {
            stackedToken.visitedCorners.Clear();
            yield return MoveTokenTo(stackedToken, BoardPointIndex.Finished);
            if (GetPlayer(token) == 1) score1++;
            else score2++;
        }
        token.visitedCorners.Clear();
        yield return MoveTokenTo(token, BoardPointIndex.Finished);
        if (GetPlayer(token) == 1) score1++;
        else score2++;
    }

    /// <summary>
    /// Decides the winner of the game(1 or 2); 0 if not yet ended
    /// </summary>
    /// <returns></returns>
    public int DecideWinner()
    {
        if (score1 == 4) return 1;
        if (score2 == 4) return 2;
        return 0;
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
        foreach (Token stackedToken in token.stackedTokens)
            stackedToken.boardPointIndex = boardPointIndex;
        if (boardPointIndex == BoardPointIndex.Initial)
        {
            yield return token.MoveTo(token.initialPosition);
        }
        else if (boardPointIndex == BoardPointIndex.Finished)
        {
            yield return token.MoveTo(token.finishedPosition);
        }
        else yield return token.MoveTo(boardPoints[(int)token.boardPointIndex]);
    }

    public void InstantMoveTokenTo(Token token, BoardPointIndex boardPointIndex)
    {
        token.boardPointIndex = boardPointIndex;
        foreach (Token stackedToken in token.stackedTokens)
            stackedToken.boardPointIndex = boardPointIndex;
        if (boardPointIndex == BoardPointIndex.Initial)
            token.InstantMoveTo(token.initialPosition);
        else if (boardPointIndex == BoardPointIndex.Finished)
            token.InstantMoveTo(token.finishedPosition);
        else token.InstantMoveTo(boardPoints[(int)token.boardPointIndex]);
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
            token.GetPreviousCornerAtCorner() == BoardPointIndex.UpperLeft) return BoardPointIndex.LeftDiag3;
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
            if (token.GetPreviousCornerAtCorner() == BoardPointIndex.LowerLeft)
                return BoardPointIndex.Lower4;
            if (token.GetPreviousCornerAtCorner() == BoardPointIndex.Center)
                return BoardPointIndex.LeftDiag4;
            return BoardPointIndex.Initial;            
        }
        if (token.boardPointIndex == BoardPointIndex.LowerLeft)
        {
            if (token.GetPreviousCornerAtCorner() == BoardPointIndex.Center) return BoardPointIndex.RightDiag4;
            return BoardPointIndex.Left4;
        }
        if (token.boardPointIndex == BoardPointIndex.RightDiag1) return BoardPointIndex.UpperRight;
        if (token.boardPointIndex == BoardPointIndex.LeftDiag1) return BoardPointIndex.UpperLeft;
        if (token.boardPointIndex == BoardPointIndex.LeftDiag3) return BoardPointIndex.Center;
        if (token.boardPointIndex == BoardPointIndex.Center &&
            token.GetPreviousCornerAtCorner() == BoardPointIndex.UpperLeft)
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
        List<BoardPointIndex> indices = new List<BoardPointIndex>();
        if (token.isValid()) indices.Add(GetPreviousIndex(token));
        foreach (Token stackedToken in token.stackedTokens)
        {
            BoardPointIndex tempIndex = GetPreviousIndex(stackedToken);
            if (stackedToken.isValid() && !indices.Contains(tempIndex)) indices.Add(tempIndex);
        }

        foreach (BoardPointIndex index in indices) Debug.Log(index);
        return indices;
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
        tempToken.visitedCorners = new Stack<BoardPointIndex>(tempToken.visitedCorners);
        tempToken.boardPointIndex = token.boardPointIndex;

        if (tempToken.boardPointIndex == BoardPointIndex.Initial)
        {
            token.PushVisitedCorners(BoardPointIndex.LowerRight);
            InstantMoveTokenTo(tempToken, BoardPointIndex.Right1);
        }
        else InstantMoveTokenByOne(tempToken, true);

        for (int i = 0; i < distance - 1; i++)
        {
            if (tempToken.boardPointIndex == BoardPointIndex.Finished) break;
            InstantMoveTokenByOne(tempToken, false);
        }
        BoardPointIndex boardPointIndex = tempToken.boardPointIndex;
        Destroy(tempTokenObject);
        return boardPointIndex;
    }

    /// <summary>
    /// Moves token by one forwards; Set isFirstMove to true for the first move in the turn
    /// </summary>
    /// <param name="token"></param>
    /// <param name="isFirstMove"></param>
    /// <returns></returns>
    public IEnumerator MoveTokenByOne(Token token, bool isFirstMove)
    {
        BoardPointIndex nextBoardPointIndex = GetNextIndex(token, isFirstMove);
        if (nextBoardPointIndex == BoardPointIndex.Finished)
        {
            yield return FinishToken(token);
            yield break;
        }
        token.PushVisitedCorners(nextBoardPointIndex);
        foreach (Token stackedToken in token.stackedTokens) {
            stackedToken.PushVisitedCorners(nextBoardPointIndex);
            InstantMoveTokenTo(stackedToken, nextBoardPointIndex);
        }
        yield return MoveTokenTo(token, nextBoardPointIndex);
    }

    /// <summary>
    /// Instantly moves token by one forwards; Set isFirstMove to true for the first move in the turn
    /// </summary>
    /// <param name="token"></param>
    /// <param name="isFirstMove"></param>
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
    /// Actually moves token by distance using MoveTokenByOne();
    /// </summary>
    /// <param name="token"></param>
    /// <param name="distance"></param>
    public IEnumerator MoveToken(Token token, int distance)
    {
        if (token.boardPointIndex == BoardPointIndex.Initial)
        {
            yield return MoveTokenTo(token, BoardPointIndex.LowerRight);
            token.PushVisitedCorners(BoardPointIndex.LowerRight);
            yield return MoveTokenTo(token, BoardPointIndex.Right1);
        }
        else yield return MoveTokenByOne(token, true);

        for (int i = 0; i < distance-1; i++)
        {
            if (token.boardPointIndex == BoardPointIndex.Finished) break;
            yield return MoveTokenByOne(token, false);
        }
        HandleStackable(token);
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
            ResetToken(token);
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
        HandleStackable(token);
    }

    /// <summary>
    /// Moves token backwards, with a route specified by index
    /// </summary>
    /// <param name="token"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    public IEnumerator MoveTokenBackwards(Token token, int index)
    {
        BoardPointIndex previousBoardPointIndex = GetPreviousIndices(token)[index];
        yield return MoveTokenBackwardsTo(token, previousBoardPointIndex);
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
        StartCoroutine(MoveTokenBackwards(token, index));
    }

    /// <summary>
    /// A debug function that handles keyboard input
    /// </summary>
    private void DebugHandleInput()
    {
        if (keyStep == 0)
        {
            keyStep = 1;

            if (Input.GetKeyDown(KeyCode.A)) winTokenList = tokens1;
            else if (Input.GetKeyDown(KeyCode.B)) winTokenList = tokens2;
            else keyStep = 0;
        }
        else if (keyStep == 1)
        {
            keyStep = 2;

            if (Input.GetKeyDown(KeyCode.Alpha1)) steps = 1;
            else if (Input.GetKeyDown(KeyCode.Alpha2)) steps = 2;
            else if (Input.GetKeyDown(KeyCode.Alpha3)) steps = 3;
            else if (Input.GetKeyDown(KeyCode.Alpha4)) steps = 4;
            else if (Input.GetKeyDown(KeyCode.Alpha5)) steps = 5;
            else if (Input.GetKeyDown(KeyCode.Alpha0)) steps = -1;
            else keyStep = 1;
        }
        else if (keyStep == 2)
        {
            keyStep = 3;

            Token onMouseOverToken = null;
            foreach (Token winToken in winTokenList)
            {
                if (AbleToClickToken(winToken) && winToken.IsOnMouseOver() == true)
                {
                    onMouseOverToken = winToken;
                    break;
                }
            }

            if (onMouseOverToken == null) prepareManager.PreparePreviews(steps);
            else OnMouseEnterTokenGroup(onMouseOverToken);
        }
    }

    public void OnMouseEnterTokenGroup(Token token)
    {
        if (keyStep == 3 && AbleToClickToken(token))
        {
            prepareManager.OnMouseEnterTokenGroup(token, steps);
        }
    }

    public void OnMouseExitTokenGroup(Token token)
    {
        if (keyStep == 3 && AbleToClickToken(token))
        {
            prepareManager.OnMouseExitTokenGroup(token, steps);
        }
    }

    public void OnMouseDownTokenGroup(Token token)
    {
        if (keyStep == 3 && AbleToClickToken(token))
        {
            prepareManager.OnMouseDownTokenGroup(token, steps);

            if (steps == -1)
            {
                List<BoardPointIndex> previousIndices = GetPreviousIndices(token);
                if (previousIndices.Count == 1)
                {
                    keyStep = 0;

                    StartMoveBackwards(token, previousIndices[0]);
                }
                else keyStep = 4;
            }
            else
            {
                keyStep = 0;
                StartMove(token, steps);
            }
        }
    }

    public void OnClickBackButton(Token token, BoardPointIndex boardPointIndex)
    {
        if (keyStep == 4)
        {
            keyStep = 0;

            prepareManager.OnClickBackButton();

            StartMoveBackwards(token, boardPointIndex);
        }
    }

    public bool AbleToClickToken(Token token)
    {
        if (winTokenList.Contains(token))
        {
            if (token.boardPointIndex != BoardPointIndex.Finished)
            {
                if (steps != -1 || token.boardPointIndex != BoardPointIndex.Initial)
                {
                    return true;
                }
            }
        }

        return false;
    }

    public void OnClickRestartButton()
    {
        SceneManager.LoadScene("Yutnori");
    }

    public void OnClickNewGameButton()
    {
        SceneManager.LoadScene("Login");
    }
}