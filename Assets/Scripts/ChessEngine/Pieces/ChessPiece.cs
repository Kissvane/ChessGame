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
    //when i use vector2 to represent positions on the board x is the column and y the line
    public Vector2 currentPosition;

    public virtual void CalculateAvailableDestinations(bool isSimulation = false)
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
                    if (isValidMovement(ChessEngine.instance.board, (int)destinationToTest.x, (int)destinationToTest.y, out pieceTaken,isSimulation))
                    {
                        availableDestinations.Add(new Vector2(originColumn, originLine)+direction*i);
                        if (pieceTaken)
                        {
                            //for this calculation turn this direction is blocked
                            moveDirectionsAndBlockedState[direction] = true;
                        }
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
    public virtual bool isValidMovement(Board board, int testedColumn, int testedLine, out bool pieceTaken,bool checkKingSafety = true)
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

        origin.piece = null;
        destination.piece = movedPiece;

        movedPiece.Moved(new Vector2(originColumn, originLine), new Vector2(testedColumn, testedLine));
    }

    public virtual void ResetBlockedDirections()
    {
        List<Vector2> temp = new List<Vector2>();
        temp.AddRange(moveDirectionsAndBlockedState.Keys);

        foreach (Vector2 direction in temp)
        {
            moveDirectionsAndBlockedState[direction] = false;
        }
    }

    public virtual void Moved(Vector2 origin, Vector2 destination)
    {
        hasMoved = true;
        currentPosition = destination;
        ChessEngine.instance.movedDuringThisTurn.Add(this);
    }

    public virtual void Captured()
    {
        GameObject captured = team.piecesObjects[this];
        team.other.Capture(this, captured);
        ChessEngine.instance.capturedDuringThisTurn = this;
        ChessEngine.instance.capturedDuringThisTurnGameobject = captured;
        team.piecesObjects.Remove(this);
    }

    public abstract void SetMovementLimit();
}
