using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board
{
    public List<List<ChessboardBox>> chessboardBoxes = new List<List<ChessboardBox>>();
    
    public Board(List<List<ChessboardBox>> boxes)
    {
        chessboardBoxes = boxes;
    }

    public Board(Board original)
    {
        for (int i = 0; i < original.chessboardBoxes.Count; i++)
        {
            chessboardBoxes.AddRange(original.chessboardBoxes);
        }
    }


    /*private void Start()
    {
        MyEventSystem.instance.Set("Board", this);
        MyEventSystem.instance.RegisterDynamicData("isMovementValid", this, isValidMovement);
        MyEventSystem.instance.RegisterDynamicData("getBox",this,getBox);
    }*/

    dynamic getBox(int line, int column)
    {
        return chessboardBoxes[column][line];
    }

    //test if a movement is valid
    dynamic isValidMovement(int originLine, int originColumn,int testedLine, int testedColumn)
    {
        ChessboardBox testedBox = chessboardBoxes[testedColumn][testedLine];
        ChessPiece movedPiece = chessboardBoxes[originColumn][originLine].piece;
        ChessPiece takenPiece = testedBox.piece;
        bool tryCastling = false;
        // the destination is out of the board
        if (testedLine < 0 || testedLine > 7 || testedColumn < 0 || testedColumn > 7) return false;

        if (takenPiece != null && takenPiece.team == movedPiece.team)
        {
            if(!takenPiece.canCastling || !movedPiece.canCastling) return false;
            tryCastling = true;
        }

        //copy the current state of board
        Board simulation =  new Board(this);
        //simulate the move
        simulation.Move( originLine, originColumn, testedLine, testedColumn, tryCastling);
        //simulate the move and check if the king is safe
        return simulation.IsTheKingSafe(movedPiece.team);
    }

    public bool IsTheKingSafe(TeamManager playingTeam)
    {


        return false;
    }

    

    public void Move(int originLine, int originColumn, int testedLine, int testedColumn, bool tryCastling = false)
    {
        //manage special case (castling/enPassant)
        if (tryCastling)
        {

        }
        //make the normal move
        else
        {

        }
    }
}
