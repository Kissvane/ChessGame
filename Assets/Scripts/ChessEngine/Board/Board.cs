using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    //only in queen at the moment
    public void PromotePawn(Vector2 destination, PieceType type)
    {
        ChessboardBoxData birthCase = getBox((int)destination.x, (int)destination.y);
        Pawn pawn = (Pawn)birthCase.piece;
        TeamManager playingTeam = pawn.team;
        GameObject PawnObject = playingTeam.piecesObjects[pawn];

        Queen queen = new Queen();
        queen.team = playingTeam;
        queen.currentPosition = destination;
        
        playingTeam.piecesObjects.Add(queen, PawnObject);
        playingTeam.queens.Add(queen);

        playingTeam.piecesObjects.Remove(pawn);
        playingTeam.pawns.Remove(pawn);
    }

    public void SetPieceOnBox(ChessPiece piece, int lineDestination, int columnDestination, TeamManager team)
    {
        //get the chessboard box
        ChessboardBoxData target = ChessEngine.instance.board.getBox(columnDestination, lineDestination);
        //assign a piece to the targeted chessboard box
        target.piece = piece;
        //assign the piece team
        piece.team = team;
        piece.currentPosition = new Vector2(columnDestination, lineDestination);
        piece.SetMovementLimit();
        //add the piece in team piece's list
        team.piecesObjects.Add(piece, null);
    }
}
