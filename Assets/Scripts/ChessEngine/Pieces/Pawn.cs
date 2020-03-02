using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pawn : ChessPiece
{
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

        availableDestinations.Add(team.teamForward+Vector2.right);
        availableDestinations.Add(team.teamForward + Vector2.left);
    }

    //test if a movement is valid
    public override bool isValidMovement(Board board, int testedLine, int testedColumn, bool checkKingSafety = true)
    {
        ChessboardBoxData testedBox = board.boxesDatas[testedColumn][testedLine];
        ChessPiece takenPiece = testedBox.piece;
        ChessPiece enPassantLeftPiece = null;
        ChessPiece enPassantRightPiece = null;

        if (currentPosition.x > 0)
        {
            enPassantLeftPiece = board.boxesDatas[testedColumn - 1][testedLine].piece;
        }
        if (currentPosition.x < 7)
        {
            enPassantRightPiece = board.boxesDatas[testedColumn + 1][testedLine].piece;
        }
        

        bool tryCapture = testedColumn != (int)currentPosition.x;
        
        // the destination is out of the board
        if (testedLine < 0 || testedLine > 7 || testedColumn < 0 || testedColumn > 7) return false;

        //pawn try a capture
        if (tryCapture)
        {
            //the box contains a friend piece movement is not valid
            if (takenPiece == null)
            {
                //Manage enPassant case
                if (enPassantLeftPiece == null || enPassantRightPiece == null || enPassantLeftPiece.enPassantAllowed || enPassantRightPiece.enPassantAllowed) return false;
            }
            if (takenPiece.team == team) return false;
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
        //TODO allow promotion in knight
        if ((destination.y == 7 && team.teamEnum == ChessColor.White) || (destination.y == 0 && team.teamEnum == ChessColor.Black))
        {
            Linker.instance.board.ChangePawnInQueen(destination);
        }
    }

    public override void Captured()
    {
        base.Captured();
        team.pawns.Remove(this);
    }
}
