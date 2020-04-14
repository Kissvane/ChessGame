using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Rook : ChessPiece
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
    public override bool isValidMovement(Board board, int testedColumn , int testedLine, out bool pieceTaken, bool isSimulation)
    {
        pieceTaken = false;
        // the destination is out of the board
        if (testedLine < 0 || testedLine > 7 || testedColumn < 0 || testedColumn > 7) return false;

        ChessboardBoxData testedBox = board.boxesDatas[testedColumn][testedLine];
        ChessPiece takenPiece = testedBox.piece;

        //Allow castling
        if (takenPiece != null)
        {
            pieceTaken = true;
            if (takenPiece.team == team && (!canCastling || !takenPiece.canCastling)) return false;
        }

        return FinalValidation(board, testedColumn, testedLine, isSimulation);
    }

    public override void Move(Board board, int testedLine, int testedColumn)
    {
        ChessboardBoxData origin = board.getBox(currentPosition);
        ChessboardBoxData destination = board.getBox(testedColumn, testedLine);
 
        ChessPiece otherPiece = destination.piece;
        Vector2 originPosition = currentPosition;
        
        bool isCastling = false;
        //manage castling case
        if (otherPiece != null && otherPiece.canCastling && otherPiece.team == this.team)
        {
            //piece swapping
            origin.piece = otherPiece;
            otherPiece.Moved(originPosition);
            isCastling = true;
        }
        //make the normal move
        else
        {
            origin.piece = null;
            if (otherPiece != null)
            {
                otherPiece.Captured();
            }
        }
        destination.piece = this;

        Move moveToSave = new Move(originPosition, new Vector2(testedColumn, testedLine), this, otherPiece, isCastling);
        ChessEngine.instance.game.Add(moveToSave);
        //Debug.Log("MOVE SAVED " + moveToSave.movingPiece.name + " " + moveToSave.destination);
        Moved(new Vector2(testedColumn, testedLine));
    }

    public override void ForcedReverseMove(Board board, Move move)
    {
        ChessboardBoxData originBox = board.getBox(move.destination);
        ChessboardBoxData destinationBox = board.getBox(move.origin);
        if (move.capturedPiece != null)
        {
            originBox.piece = move.capturedPiece;
            move.capturedPiece.RevertValuesIfNecessary(originBox,true);
        }
        else if (move.castlingPiece != null)
        {
            originBox.piece = move.castlingPiece;
            move.castlingPiece.RevertValuesIfNecessary(originBox, false);
        }
        else
        {
            originBox.piece = null;
        }
        destinationBox.piece = this;
        RevertValuesIfNecessary(destinationBox,false);
        //Debug.Log("REVERTED");
    }

    public override void ForcedMove(Board board, Move move)
    {
        ChessboardBoxData originBox = board.getBox(move.origin);
        ChessboardBoxData destinationBox = board.getBox(move.destination);
        if (move.capturedPiece != null)
        {
            move.capturedPiece.Captured();
        }
        else if (move.castlingPiece != null)
        {
            originBox.piece = move.castlingPiece;
            move.castlingPiece.RedoValuesIfNecessary(originBox);
        }
        else
        {
            originBox.piece = null;
        }
        destinationBox.piece = this;
        RedoValuesIfNecessary(destinationBox);
    }

    public override void Moved(Vector2 destination)
    {
        canCastling = false;
        base.Moved(destination);
    }

    public override void Captured()
    {
        base.Captured();
        team.rooks.Remove(this);
    }

    public override void Liberated()
    {
        base.Liberated();
        team.rooks.Add(this);
    }


    public override void RevertValuesIfNecessary(ChessboardBoxData box, bool liberated)
    {
        base.RevertValuesIfNecessary(box,liberated);
        canCastling = !hasMoved;
    }

    public override void RedoValuesIfNecessary(ChessboardBoxData box)
    {
        base.RedoValuesIfNecessary(box);
        canCastling = !hasMoved;
    }
}
