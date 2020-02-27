using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Board
{
    public List<List<ChessboardBoxData>> boxesDatas = new List<List<ChessboardBoxData>>();
    
    public Board(List<List<ChessboardBoxData>> boxes)
    {
        boxesDatas = boxes;
    }

    public Board(Board original)
    {
        for (int i = 0; i < original.boxesDatas.Count; i++)
        {
            boxesDatas.AddRange(original.boxesDatas);
        }
    }

    public ChessboardBoxData getBox(int column, int line)
    {
        return boxesDatas[column][line];
    }

    //test if a movement is valid
    /*public bool isValidMovement(int originLine, int originColumn,int testedLine, int testedColumn, bool checkKingSafety = true)
    {
        ChessboardBoxData testedBox = boxesDatas[testedColumn][testedLine];
        ChessPiece movedPiece = boxesDatas[originColumn][originLine].piece;
        ChessPiece takenPiece = testedBox.piece;
        bool tryCastling = false;
        // the destination is out of the board
        if (testedLine < 0 || testedLine > 7 || testedColumn < 0 || testedColumn > 7) return false;

        if (takenPiece != null && takenPiece.team == movedPiece.team)
        {
            if(!takenPiece.canCastling || !movedPiece.canCastling) return false;
            tryCastling = true;
        }
  
        if (checkKingSafety)
        {
            //copy the current state of board
            Board simulation = new Board(this);
            //simulate the move
            simulation.Move(originLine, originColumn, testedLine, testedColumn, tryCastling);
            //simulate the move and check if the king is safe
            return simulation.IsMyKingSafe(movedPiece.team);
        }
        else
        {
            return true;
        }
    }*/

    public bool IsMyKingSafe(TeamManager playingTeam)
    {
        foreach(ChessPiece piece in playingTeam.other.piecesObjects.Keys)
        {
            piece.CalculateAvailableDestinations(true);
            foreach (Vector2 destination in piece.availableDestinations)
            {
                if (getBox((int)destination.x, (int)destination.y).piece.isKing) return false;
            }
        }
        return true;
    }

    /*public void Move(int originLine, int originColumn, int testedLine, int testedColumn, bool tryCastling = false)
    {
        ChessboardBoxData origin = getBox(originColumn, originLine);
        ChessboardBoxData destination = getBox(testedColumn, testedLine);
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
    }*/
}
