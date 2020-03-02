using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class ChessPiece
{
    public Dictionary<Vector2,bool> moveDirectionsAndBlockedState = new Dictionary<Vector2, bool>();
    //protected List<Vector2> allowedMoveDirections;
    public int maxRange = 0;
    public List<Vector2> availableDestinations;
    public TeamManager team;
    public bool hasMoved = false;
    //public Renderer pawnRenderer;
    //public SpriteRenderer iconRenderer;
    public bool canCastling = false;
    public bool isKing = false;
    public bool isPawn = false;
    public bool enPassantAllowed = false;
    //when i use vector2 to represent positions on the board x is the column and y the line
    public Vector2 currentPosition;

    public virtual void CalculateAvailableDestinations(bool isSimulation = false)
    {
        ResetBlockedDirections();
        availableDestinations.Clear();
        int originLine = (int)currentPosition.y;
        int originColumn = (int)currentPosition.x;
        for (int i = 1; i <= maxRange; i++)
        {
            foreach (Vector2 direction in moveDirectionsAndBlockedState.Keys)
            {
                //if this direction is not blocked
                if (!moveDirectionsAndBlockedState[direction])
                {
                    if (isValidMovement(ChessEngine.instance.board, (int)direction.y*i, (int)direction.x*i, isSimulation))
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
        ChessboardBoxData testedBox = board.boxesDatas[testedColumn][testedLine];
        ChessPiece takenPiece = testedBox.piece;
        // the destination is out of the board
        if (testedLine < 0 || testedLine > 7 || testedColumn < 0 || testedColumn > 7) return false;

        //the box contains a friend piece movement is not valid
        if (takenPiece != null && takenPiece.team == team)
        {
            return false;
        }

        if (checkKingSafety)
        {
            //copy the current state of board
            Board simulation = new Board(board);
            //simulate the move
            Move(simulation, (int)currentPosition.y, (int)currentPosition.x, testedLine, testedColumn);
            //simulate the move and check if the king is safe
            return simulation.IsMyKingSafe(team);
        }
        else
        {
            return true;
        }
    }

    public virtual void Move(Board board, int originLine, int originColumn, int testedLine, int testedColumn)
    {
        ChessboardBoxData origin = board.getBox(originColumn, originLine);
        ChessboardBoxData destination = board.getBox(testedColumn, testedLine);
        ChessPiece movedPiece = origin.piece;
        ChessPiece otherPiece = destination.piece;
 
        if (otherPiece != null)
        {
            otherPiece.Captured();
        }

        movedPiece.Moved(new Vector2(originColumn, originLine), new Vector2(testedColumn, testedLine));
    }

    public virtual void ResetBlockedDirections()
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
        team.piecesObjects.Remove(this);
    }

    public abstract void SetMovementLimit();
}
