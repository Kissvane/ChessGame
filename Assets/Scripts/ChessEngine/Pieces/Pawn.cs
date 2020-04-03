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

    public override void CalculateAvailableDestinations(bool isSimulation = false)
    {
        base.CalculateAvailableDestinations(isSimulation);
        availableEnPassantPosition = new Vector2(-1, -1);
        enPassantStoredPiece = null;
        Vector2 right = currentPosition + team.teamForward + Vector2.right;
        bool pieceTaken = false;
        if (isValidMovement(ChessEngine.instance.board,(int)right.x,(int)right.y, out pieceTaken,isSimulation))
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
    public override bool isValidMovement(Board board, int testedColumn, int testedLine, out bool pieceTaken, bool checkKingSafety = true)
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
            if (takenPiece != null) return false;
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
        base.Move(board, originLine, originColumn, testedLine, testedColumn);
        //Manage enPassant case
        if (currentPosition == availableEnPassantPosition)
        {
            board.getBox((int)enPassantStoredPiece.currentPosition.x, (int)enPassantStoredPiece.currentPosition.y).piece = null;
            enPassantStoredPiece.Captured();
        }
    }

    public override void Moved(Vector2 origin,Vector2 destination)
    {
        if (!hasMoved)
        {
            //if the first move is a 2 square move an enPassant capture is Possible during one turn 
            if(Mathf.Abs(destination.y - origin.y) == 2)
            {
                enPassantAllowed = true;
            }

            maxRange = 1;
        }
        else
        {
            enPassantAllowed = false;
        }
        base.Moved(origin, destination);
        //pawn promotion
        if ((destination.y == 7 && team.teamEnum == ChessColor.White) || (destination.y == 0 && team.teamEnum == ChessColor.Black))
        {
            ChessEngine.instance.waitingPromotion = this;
        }
    }

    public override void Captured()
    {
        base.Captured();
        team.pawns.Remove(this);
    }
}
