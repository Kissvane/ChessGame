using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class ChessPiece
{
    public Dictionary<Vector2,bool> moveDirectionsAndBlockedState = new Dictionary<Vector2, bool>();
    //protected List<Vector2> allowedMoveDirections;
    public int maxRange = 0;
    public List<Vector2> availableDestinations = new List<Vector2>();
    public TeamManager team;
    public bool hasMoved = false;
    //public Renderer pawnRenderer;
    //public SpriteRenderer iconRenderer;
    public bool canCastling = false;
    public bool isKing = false;
    public bool isPawn = false;
    public bool enPassantAllowed = false;
    public string name = null;
    //when i use vector2 to represent positions on the board x is the column and y the line
    public Vector2 currentPosition;

    public virtual void CalculateAvailableDestinations(bool isSimulation, bool verbose = false)
    {
        ResetBlockedDirections();
        availableDestinations.Clear();
        int originLine = (int)currentPosition.y;
        int originColumn = (int)currentPosition.x;
        List<Vector2> temp = new List<Vector2>();
        temp.AddRange(moveDirectionsAndBlockedState.Keys);

        for (int i = 1; i <= maxRange; i++)
        {
            foreach (Vector2 direction in temp)
            {
                Vector2 destinationToTest = new Vector2(originColumn, originLine) + direction * i;
                //if this direction is not blocked
                if (!moveDirectionsAndBlockedState[direction])
                {
                    bool pieceTaken = false;
                    if (isValidMovement(ChessEngine.instance.board, (int)destinationToTest.x, (int)destinationToTest.y, out pieceTaken, isSimulation))
                    {
                        if (verbose) Debug.Log(currentPosition+" "+ name + " VALID DESTINATION " + destinationToTest);
                        availableDestinations.Add(new Vector2(originColumn, originLine)+direction*i);
                    }
                    else
                    {
                        if (verbose) Debug.Log(currentPosition + " " + name +" INVALID MOVE "+destinationToTest);
                    }
                    if (pieceTaken)
                    {
                        if (verbose) Debug.Log(currentPosition + " " + name + " PIECE TAKEN " + destinationToTest + ChessEngine.instance.board.getBox(destinationToTest).piece.name);
                        //for this calculation turn this direction is blocked
                        moveDirectionsAndBlockedState[direction] = true;
                    }
                }
                else
                {
                    if (verbose) Debug.Log(currentPosition + " " + name +" BLOCKED DIRECTION "+destinationToTest);
                }
            }
        }
    }

    //test if a movement is valid
    public virtual bool isValidMovement(Board board, int testedColumn, int testedLine, out bool pieceTaken, bool isSimulation)
    {
        pieceTaken = false;
        // the destination is out of the board
        if (testedLine < 0 || testedLine > 7 || testedColumn < 0 || testedColumn > 7) return false;

        ChessboardBoxData testedBox = board.boxesDatas[testedColumn][testedLine];
        ChessPiece takenPiece = testedBox.piece;

        //the box contains a friend piece movement is not valid
        if (takenPiece != null)
        {
            pieceTaken = true;
            if (takenPiece.team == team) return false;
        }

        return FinalValidation(board, testedColumn, testedLine, isSimulation);
    }

    //Validate the move by testing king status if it's not a simulation
    public bool FinalValidation(Board board, int testedColumn, int testedLine, bool isSimulation)
    {
        if (!isSimulation)
        {
            return CheckKingStatus(board, testedLine, testedColumn);
        }
        else
        {
            return true;
        }
    }

    public bool CheckKingStatus(Board board, int testedLine, int testedColumn)
    {
        Move(board, testedLine, testedColumn);
        bool isMyKingSafe = board.IsMyKingSafe(team);
        ChessEngine.instance.CancelLastMove(false);
        return isMyKingSafe;
    }

    public virtual void ForcedReverseMove(Board board, Move move)
    {
        ChessboardBoxData originBox = board.getBox(move.destination);
        ChessboardBoxData destinationBox = board.getBox(move.origin);
        if (move.capturedPiece != null)
        {
            originBox = board.getBox(move.capturedPiece.currentPosition);
            originBox.piece = move.capturedPiece;
            move.capturedPiece.RevertValuesIfNecessary(originBox,true);
        }
        else
        {
            originBox.piece = null;
        }

        destinationBox.piece = this;
        RevertValuesIfNecessary(destinationBox, false);
    }

    public virtual void ForcedMove(Board board, Move move)
    {
        ChessboardBoxData originBox = board.getBox(move.origin);
        ChessboardBoxData destinationBox = board.getBox(move.destination);
        if (move.capturedPiece != null)
        {
            move.capturedPiece.Captured();
        }
        originBox.piece = null;
        destinationBox.piece = this;
        //update values again after forced move
        RedoValuesIfNecessary(destinationBox);
    }

    public virtual void Move(Board board, int testedLine, int testedColumn)
    {
        ChessboardBoxData originBox = board.getBox(currentPosition);
        ChessboardBoxData destinationBox = board.getBox(testedColumn, testedLine);
        ChessPiece otherPiece = destinationBox.piece;

        if (otherPiece != null)
        {
            otherPiece.Captured();
        }

        originBox.piece = null;
        destinationBox.piece = this;

        Move moveToSave = new Move(currentPosition, new Vector2(testedColumn, testedLine), this, otherPiece, false);
        ChessEngine.instance.game.Add(moveToSave);
        Moved(new Vector2(testedColumn, testedLine));
    }

    //reset the data structure that allow the piece to calculate availables moves
    public virtual void ResetBlockedDirections()
    {
        List<Vector2> temp = new List<Vector2>();
        temp.AddRange(moveDirectionsAndBlockedState.Keys);

        foreach (Vector2 direction in temp)
        {
            moveDirectionsAndBlockedState[direction] = false;
        }
    }

    //operations to do when a piece is moved
    public virtual void Moved(Vector2 destination)
    {
        hasMoved = true;
        currentPosition = destination;
        ChessEngine.instance.movedDuringThisTurn.Add(this);
    }

    //operations to do when a piece is captured
    public virtual void Captured()
    {
        team.other.Capture(this);
        ChessEngine.instance.capturedDuringThisTurn = this;
        team.pieces.Remove(this);
    }

    //operations to do when a piece is liberated
    public virtual void Liberated()
    {
        ChessEngine.instance.liberated = this;
        team.other.Liberate(this);
        team.pieces.Add(this);
    }

    //revert some piece values after special move
    public virtual void RevertValuesIfNecessary(ChessboardBoxData box, bool liberated)
    {
        currentPosition = new Vector2(box.column, box.line);
        hasMoved = ChessEngine.instance.hasMovedInPreviousMove(this);
        if (liberated)
        {
            Liberated();
        }
        else
        {
            ChessEngine.instance.reverted.Add(this);
        }
    }

    //update values again after forced move
    public virtual void RedoValuesIfNecessary(ChessboardBoxData box)
    {
        currentPosition = new Vector2(box.column, box.line);
        hasMoved = ChessEngine.instance.hasMovedInPreviousMove(this);
        ChessEngine.instance.movedDuringThisTurn.Add(this);
    }

    public abstract void SetMovementLimit();
}
