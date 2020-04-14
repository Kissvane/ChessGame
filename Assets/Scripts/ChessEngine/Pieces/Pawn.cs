using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Pawn : ChessPiece
{
    public Vector2 availableEnPassantPosition = new Vector2(-1,-1);
    public ChessPiece enPassantStoredPiece = null;

    public override void SetMovementLimit()
    {
        isPawn = true;
        maxRange = 2;
        moveDirectionsAndBlockedState = new Dictionary<Vector2, bool>();
        Vector2 teamForward = team.teamEnum == ChessColor.White ? Vector2.up : Vector2.down;
        moveDirectionsAndBlockedState.Add(teamForward, false);
    }

    public override void CalculateAvailableDestinations(bool isSimulation, bool verbose = false)
    {
        base.CalculateAvailableDestinations(isSimulation, verbose);
        availableEnPassantPosition = new Vector2(-1, -1);
        enPassantStoredPiece = null;
        Vector2 right = currentPosition + team.teamForward + Vector2.right;
        bool pieceTaken = false;
        if (isValidMovement(ChessEngine.instance.board,(int)right.x,(int)right.y, out pieceTaken, isSimulation))
        {
            availableDestinations.Add(right);
        }
        Vector2 left = currentPosition + team.teamForward + Vector2.left;
        if (isValidMovement(ChessEngine.instance.board, (int)left.x, (int)left.y, out pieceTaken, isSimulation))
        {
            availableDestinations.Add(left);
        }
    }

    //test if a movement is valid
    public override bool isValidMovement(Board board, int testedColumn, int testedLine, out bool pieceTaken, bool isSimulation)
    {
        pieceTaken = false;
        // the destination is out of the board
        if (testedLine < 0 || testedLine > 7 || testedColumn < 0 || testedColumn > 7) return false;

        ChessboardBoxData testedBox = board.boxesDatas[testedColumn][testedLine];
        ChessPiece takenPiece = testedBox.piece;

        ChessPiece enPassantPiece = null;
        enPassantPiece = board.boxesDatas[testedColumn][testedLine - (int)team.teamForward.y].piece;
        
        bool tryCapture = testedColumn != (int)currentPosition.x;

        //pawn try a capture
        if (tryCapture)
        {
            if (takenPiece == null)
            {
                //Manage enPassant case
                bool availableEnPassant =   enPassantPiece != null 
                                            && enPassantPiece.team != team 
                                            && enPassantPiece.enPassantAllowed;

                if (!availableEnPassant) return false;
                else
                {
                    availableEnPassantPosition = new Vector2(testedColumn, testedLine);
                    enPassantStoredPiece = enPassantPiece;
                }
            }
            else
            {
                pieceTaken = true;
                //the box contains a friend piece movement is not valid
                if (takenPiece.team == team) return false;
            }
        }
        else
        {
            if (takenPiece != null)
            {
                pieceTaken = true;
                return false;
            }
        }

        return FinalValidation(board, testedColumn, testedLine, isSimulation);
    }

    public override void Move(Board board, int testedLine, int testedColumn)
    {
        //Debug.Log(name + " MOVE " + new Vector2(testedColumn, testedLine));
        base.Move(board, testedLine, testedColumn);
        //Manage enPassant case
        if (currentPosition == availableEnPassantPosition)
        {
            board.getBox((int)enPassantStoredPiece.currentPosition.x, (int)enPassantStoredPiece.currentPosition.y).piece = null;
            enPassantStoredPiece.Captured();
            ChessEngine.instance.game[ChessEngine.instance.game.Count - 1].capturedPiece = enPassantStoredPiece;
        }
    }

    public override void Moved(Vector2 destination)
    {
        if (!hasMoved)
        {
            //if the first move is a 2 square move an enPassant capture is Possible during one turn 
            if(Mathf.Abs(destination.y - currentPosition.y) == 2)
            {
                enPassantAllowed = true;
            }

            maxRange = 1;
        }
        else
        {
            enPassantAllowed = false;
        }
        base.Moved(destination);
        //Debug.Log(name + " MOVED " + currentPosition);
        //pawn promotion
        if ((destination.y == 7 && team.teamEnum == ChessColor.White) || (destination.y == 0 && team.teamEnum == ChessColor.Black))
        {
            ChessEngine.instance.waitingPromotion = this;
        }
    }

    public override void RevertValuesIfNecessary(ChessboardBoxData box, bool liberated)
    {
        base.RevertValuesIfNecessary(box, liberated);
        if (hasMoved)
        {
            Move penultimateMove = ChessEngine.instance.GetMyPenultimateMove(this);
            if (penultimateMove != null)
            {
                enPassantAllowed = Mathf.Abs(penultimateMove.destination.y - penultimateMove.origin.y) == 2;
            }
        }
        else
        {
            maxRange = 2;
            enPassantAllowed = false;
        }

    }

    public override void RedoValuesIfNecessary(ChessboardBoxData box)
    {
        base.RedoValuesIfNecessary(box);
        if (hasMoved)
        {
            Move penultimateMove = ChessEngine.instance.GetMyPenultimateMove(this);
            if (penultimateMove != null)
            {
                enPassantAllowed = Mathf.Abs(penultimateMove.destination.y - penultimateMove.origin.y) == 2;
            }
        }
        else
        {
            maxRange = 2;
            enPassantAllowed = false;
        }
    }

    public override void Captured()
    {
        base.Captured();
        team.pawns.Remove(this);
    }

    public override void Liberated()
    {
        base.Liberated();
        team.pawns.Add(this);
    }

}
