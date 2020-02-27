using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ChessPiece
{
    public Dictionary<Vector2,bool> moveDirectionsAndBlockedState = new Dictionary<Vector2, bool>();
    //protected List<Vector2> allowedMoveDirections;
    public int maxRange = 0;
    public List<Vector2> availableDestinations;
    public TeamManager team;
    public bool hasMoved = false;
    public Renderer pawnRenderer;
    public SpriteRenderer iconRenderer;
    public bool canCastling = false;
    public bool isKing = false;
    public bool isPawn = false;
    //when i use vector2 to represent positions on the board x is the column and y the line
    public Vector2 currentPosition;

    public virtual void CalculateAvailableDestinations(bool isSimulation = false)
    {
        ResetBlockedDirections();
        availableDestinations.Clear();
        Board board = MyEventSystem.instance.Get("Board");
        int originLine = (int)currentPosition.y;
        int originColumn = (int)currentPosition.x;
        for (int i = 1; i <= maxRange; i++)
        {
            foreach (Vector2 direction in moveDirectionsAndBlockedState.Keys)
            {
                //if this direction is not blocked
                if (!moveDirectionsAndBlockedState[direction])
                {
                    if (isValidMovement(board, (int)direction.y*i, (int)direction.x*i, isSimulation))
                    {
                        availableDestinations.Add(new Vector2(originColumn, originLine)+direction*i);
                    }
                    else
                    {
                        //for this calculation turn this direction is blocked
                        moveDirectionsAndBlockedState[direction] = true;
                    }
                }
            }
        }
    }

    //test if a movement is valid
    public virtual bool isValidMovement(Board board, int testedLine, int testedColumn, bool checkKingSafety = true)
    {
        int originColumn = (int)currentPosition.x;
        int originLine = (int)currentPosition.y;
        ChessboardBoxData testedBox = board.boxesDatas[testedColumn][testedLine];
        ChessPiece movedPiece = board.boxesDatas[originColumn][originLine].piece;
        ChessPiece takenPiece = testedBox.piece;
        bool tryCastling = false;
        // the destination is out of the board
        if (testedLine < 0 || testedLine > 7 || testedColumn < 0 || testedColumn > 7) return false;

        //KING ONLY
        if (takenPiece != null && takenPiece.team == movedPiece.team)
        {
            if (!takenPiece.canCastling || !movedPiece.canCastling) return false;
            tryCastling = true;
        }

        if (checkKingSafety)
        {
            //copy the current state of board
            Board simulation = new Board(board);
            //simulate the move
            Move(simulation,originLine, originColumn, testedLine, testedColumn, tryCastling);
            //simulate the move and check if the king is safe
            return simulation.IsMyKingSafe(movedPiece.team);
        }
        else
        {
            return true;
        }
    }

    public virtual void Move(Board board, int originLine, int originColumn, int testedLine, int testedColumn, bool tryCastling = false)
    {
        ChessboardBoxData origin = board.getBox(originColumn, originLine);
        ChessboardBoxData destination = board.getBox(testedColumn, testedLine);
        ChessPiece movedPiece = origin.piece;
        ChessPiece otherPiece = destination.piece;
        //manage special case (castling/enPassant)
        if (tryCastling)
        {
            origin.piece = otherPiece;
            destination.piece = movedPiece;
            otherPiece.Moved(new Vector2(testedColumn, testedLine), new Vector2(originColumn, originLine));
        }
        //make the normal move
        else
        {
            if (otherPiece != null)
            {
                otherPiece.Captured();
            }
        }
        movedPiece.Moved(new Vector2(originColumn, originLine), new Vector2(testedColumn, testedLine));
    }

    public void ResetBlockedDirections()
    {
        foreach (Vector2 direction in moveDirectionsAndBlockedState.Keys)
        {
            moveDirectionsAndBlockedState[direction] = false;
        }
    }

    public virtual void Moved(Vector2 origin, Vector2 destination)
    {
        hasMoved = true;
        currentPosition = destination;
    }

    public virtual void Captured()
    {
        GameObject captured = team.piecesObjects[this];
        team.other.Capture(this, captured);
        MyEventSystem.instance.FireEvent("Captured",new GenericDictionary().Set("pieceObject",captured));
        team.piecesObjects.Remove(this);
    }

    public abstract void SetMovementLimit();
}
