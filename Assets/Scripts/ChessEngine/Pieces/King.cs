using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class King : ChessPiece
{
    private Dictionary<Vector2, bool> CastlingDirectionsAndBlockedState = new Dictionary<Vector2, bool>(); 

    public override void SetMovementLimit()
    {
        canCastling = true;
        isKing = true;
        maxRange = 1;
        moveDirectionsAndBlockedState.Add(new Vector2(1f, 1f), false);
        moveDirectionsAndBlockedState.Add(new Vector2(1f, -1f), false);
        moveDirectionsAndBlockedState.Add(new Vector2(-1f, 1f), false);
        moveDirectionsAndBlockedState.Add(new Vector2(-1f, -1f), false);
        moveDirectionsAndBlockedState.Add(Vector2.up, false);
        moveDirectionsAndBlockedState.Add(Vector2.down, false);
        moveDirectionsAndBlockedState.Add(Vector2.right, false);
        moveDirectionsAndBlockedState.Add(Vector2.left, false);
        CastlingDirectionsAndBlockedState.Add(Vector2.right, false);
        CastlingDirectionsAndBlockedState.Add(Vector2.left, false);
    }

    public override void Move(Board board, int originLine, int originColumn, int testedLine, int testedColumn)
    {
        ChessboardBoxData origin = board.getBox(originColumn, originLine);
        ChessboardBoxData destination = board.getBox(testedColumn, testedLine);
        ChessPiece movedPiece = origin.piece;
        ChessPiece otherPiece = destination.piece;
        //manage castling case
        if (otherPiece.canCastling && otherPiece.team == movedPiece.team)
        {
            //piece swapping
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

    public override void Moved(Vector2 origin, Vector2 destination)
    {
        canCastling = false;
        base.Moved(origin,destination);
    }

    public override void ResetBlockedDirections()
    {
        foreach (Vector2 direction in moveDirectionsAndBlockedState.Keys)
        {
            moveDirectionsAndBlockedState[direction] = false;
        }

        foreach (Vector2 direction in CastlingDirectionsAndBlockedState.Keys)
        {
            CastlingDirectionsAndBlockedState[direction] = false;
        }
    }

    public override void CalculateAvailableDestinations(bool isSimulation = false)
    {
        base.CalculateAvailableDestinations(isSimulation);
        //if the king can castling
        if (canCastling)
        {
            int originLine = (int)currentPosition.y;
            int originColumn = (int)currentPosition.x;
            //we test that there is no piece between the King and Tower 
            for (int i = 1; i <= 5; i++)
            {
                foreach (Vector2 direction in CastlingDirectionsAndBlockedState.Keys)
                {
                    //if this direction is not blocked
                    if (!CastlingDirectionsAndBlockedState[direction])
                    {
                        Vector2 target = new Vector2(originColumn, originLine) + direction * i;
                        //test if the way between the king and tower is free of piece
                        //test if the king is safe only at the true canstling positions
                        if (isValidMovement(Linker.instance.board, (int)direction.y * i, (int)direction.x * i, (target.x == 0 || target.x == 7) && isSimulation))
                        {
                            availableDestinations.Add(target);
                        }
                        else
                        {
                            //for this calculation turn this direction is blocked
                            CastlingDirectionsAndBlockedState[direction] = true;
                        }
                    }
                }
            }
        }
    }

    //test if a movement is valid
    public override bool isValidMovement(Board board, int testedLine, int testedColumn, bool checkKingSafety = true)
    {
        ChessboardBoxData testedBox = board.boxesDatas[testedColumn][testedLine];
        ChessPiece takenPiece = testedBox.piece;
        // the destination is out of the board
        if (testedLine < 0 || testedLine > 7 || testedColumn < 0 || testedColumn > 7) return false;

        //Allow castling
        if (takenPiece != null && takenPiece.team == team && canCastling)
        {
            if (!canCastling) return false;
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
}
