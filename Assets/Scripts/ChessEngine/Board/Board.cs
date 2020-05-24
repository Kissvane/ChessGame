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

    public ChessboardBoxData getBox(Vector2 position)
    {
        return boxesDatas[(int)position.x][(int)position.y];
    }

    public bool IsMyKingSafe(TeamManager playingTeam)
    {
        ChessPiece testedPiece = null;
        foreach(ChessPiece piece in playingTeam.other.pieces)
        {
            piece.CalculateAvailableDestinations(true);
            foreach (Vector2 destination in piece.availableDestinations)
            {
                testedPiece = getBox(destination).piece;
                if (testedPiece != null && testedPiece.team == playingTeam && testedPiece.isKing)
                {
                    return false;
                }
            }
        }
        return true;
    }

    public ChessPiece PromotePawn(Vector2 destination, PieceType type)
    {
        ChessboardBoxData birthCase = getBox((int)destination.x, (int)destination.y);
        Pawn pawn = (Pawn)birthCase.piece;
        TeamManager playingTeam = pawn.team;

        ChessPiece newPiece = null;

        switch (type)
        {
            case PieceType.Bishop:
                Bishop bishop = new Bishop();
                playingTeam.bishops.Add(bishop);
                newPiece = bishop;
                break;
            case PieceType.Knight:
                Knight knight = new Knight();
                playingTeam.knights.Add(knight);
                newPiece = knight;
                break;
            case PieceType.Rook:
                Rook rook = new Rook();
                rook.canCastling = false;
                playingTeam.rooks.Add(rook);
                newPiece = rook;
                break;
            default:
                Queen queen = new Queen();
                playingTeam.queens.Add(queen);
                newPiece = queen;
                break;
        }

        newPiece.name = pawn.name + "_promoted_"+type;
        newPiece.hasMoved = true;
        SetPieceOnBox(newPiece, pawn.currentPosition, playingTeam);

        playingTeam.pieces.Remove(pawn);
        playingTeam.pawns.Remove(pawn);

        return newPiece;
    }

    public Pawn UnpromotePawn(Board board, Move toCancel)
    {
        ChessboardBoxData birthCase = getBox(toCancel.destination);
        Pawn pawn = new Pawn();
        ChessPiece promotedPiece = toCancel.movingPiece;
        TeamManager playingTeam = promotedPiece.team;

        SetPieceOnBox(pawn, promotedPiece.currentPosition, playingTeam);

        playingTeam.pieces.Remove(promotedPiece);
        switch (promotedPiece)
        {
            case Rook rook:
                playingTeam.rooks.Remove(rook);
                break;
            case Knight knight:
                playingTeam.knights.Remove(knight);
                break;
            case Bishop bishop:
                playingTeam.bishops.Remove(bishop);
                break;
            case Queen queen:
                playingTeam.queens.Remove(queen);
                break;
            default:
                throw new System.Exception("The unpromoted piece must be a rook, a knight, a bishop or a queen");
        }

        pawn.name = promotedPiece.name.Replace("_Promoted", "");
        return pawn;
    }

    public void SetPieceOnBox(ChessPiece piece, Vector2 destination, TeamManager team)
    {
        SetPieceOnBox(piece,(int)destination.y, (int)destination.x, team);
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
        team.pieces.Add(piece);
        switch (piece)
        {
            case Rook rook:
                team.rooks.Add(rook);
                break;
            case Bishop bishop:
                team.bishops.Add(bishop);
                break;
            case Knight knight:
                team.knights.Add(knight);
                break;
            case Queen queen:
                team.queens.Add(queen);
                break;
            case Pawn pawn:
                team.pawns.Add(pawn);
                break;
            default:
                break;
        }
    }
}
