using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : ChessPiece
{
    public override void SetMovementLimit()
    {
        canCastling = true;
        maxRange = 7;

        moveDirectionsAndBlockedState.Add(Vector2.up, false);
        moveDirectionsAndBlockedState.Add(Vector2.down, false);
        moveDirectionsAndBlockedState.Add(Vector2.right, false);
        moveDirectionsAndBlockedState.Add(Vector2.left, false);
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
        base.Moved(origin, destination);
    }

    public override void Captured()
    {
        base.Captured();
        team.towers.Remove(this);
    }
}
