using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Logic and data models to play a chess game
/// </summary>
/// TODO set it to abstract when debugging is over
public class ChessEngine
{
    #region variables
    public TeamManager whiteTeam;
    public TeamManager blackTeam;
    public TeamManager whoIsPlaying;
    public Board board;
    public List<Move> game = new List<Move>();
    public List<Move> cancelledMoves = new List<Move>();
    public List<ChessPiece> movedDuringThisTurn = new List<ChessPiece>();
    public ChessPiece capturedDuringThisTurn = null;

    public List<ChessPiece> promoted = new List<ChessPiece>();
    public List<ChessPiece> unpromoted = new List<ChessPiece>();

    public List<ChessPiece> reverted = new List<ChessPiece>();
    public ChessPiece liberated = null;
    public ChessPiece beforePromotion = null;
    public ChessPiece afterPromotion = null;

    public Pawn waitingPromotion = null;
    public TeamManager winningTeam = null;

    #endregion

    #region singleton
    static ChessEngine _instance;
    public static ChessEngine instance
    { get
        {
            if (_instance == null)
            {
                _instance = new ChessEngine();
            }
            return _instance;
        }
    }

    ChessEngine()
    {
        _instance = this;
    }
    #endregion

    #region Initialization

    public void StartAGame()
    {
        movedDuringThisTurn = new List<ChessPiece>();
        capturedDuringThisTurn = null;
        waitingPromotion = null;
        winningTeam = null;

        board = ConstructBoard();
        //team initialization
        whiteTeam = new TeamManager(ChessColor.White);
        blackTeam = new TeamManager(ChessColor.Black);
        whiteTeam.other = blackTeam;
        blackTeam.other = whiteTeam;
        positionTeam(whiteTeam);
        positionTeam(blackTeam);
        //round initialization
        whoIsPlaying = whiteTeam;
        CalculateAvaibleMoves(false);
    }

    Board ConstructBoard()
    {
        bool isWhite = true;
        List<List<ChessboardBoxData>> chessboardBoxes = new List<List<ChessboardBoxData>>();
        for (int caseColumn = 0; caseColumn < 8; caseColumn++)
        {
            List<ChessboardBoxData> currentColumn = new List<ChessboardBoxData>();
            for (int caseLine = 0; caseLine < 8; caseLine++)
            {

                ChessboardBoxData box = new ChessboardBoxData(isWhite ? ChessColor.White : ChessColor.Black, caseLine, caseColumn);
                currentColumn.Add(box);
                isWhite = !isWhite;
            }
            chessboardBoxes.Add(currentColumn);
            isWhite = !isWhite;
        }

        return new Board(chessboardBoxes);
    }

    //create and position the pieces
    void positionTeam(TeamManager team)
    {
        int KingLine = -1;
        int PawnLine = -1;

        if (team.teamEnum == ChessColor.White)
        {
            KingLine = 0;
            PawnLine = 1;
        }
        else
        {
            KingLine = 7;
            PawnLine = 6;
        }

        int KingColumn = 4;

        team.king = new King();
        board.SetPieceOnBox(team.king, KingLine, KingColumn, team);
        team.king.name = "King_" + team.teamEnum;

        Queen queen = new Queen();
        board.SetPieceOnBox(queen, KingLine, KingColumn - 1, team);
        team.queens[0].name = "Queen_" + team.teamEnum;

        Bishop fool = new Bishop();
        board.SetPieceOnBox(fool, KingLine, KingColumn - 2, team);
        team.bishops[0].name = "Bishop1_" + team.teamEnum;

        Bishop fool2 = new Bishop();
        board.SetPieceOnBox(fool2, KingLine, KingColumn + 1, team);
        team.bishops[1].name = "Bishop2_" + team.teamEnum;

        Knight knight = new Knight();
        board.SetPieceOnBox(knight, KingLine, KingColumn - 3, team);
        team.knights[0].name = "Knight1_" + team.teamEnum;

        Knight knight2 = new Knight();
        board.SetPieceOnBox(knight2, KingLine, KingColumn + 2, team);
        team.knights[1].name = "Knight2_" + team.teamEnum;

        Rook rook = new Rook();
        board.SetPieceOnBox(rook, KingLine, KingColumn - 4, team);
        team.rooks[0].name = "Rook1_" + team.teamEnum;

        Rook rook2 = new Rook();
        board.SetPieceOnBox(rook2, KingLine, KingColumn + 3, team);
        team.rooks[1].name = "Rook2_" + team.teamEnum;

        Pawn pawn = null;
        for (int i = 0; i < 8; i++)
        {
            pawn = new Pawn();
            board.SetPieceOnBox(pawn, PawnLine, i, team);
            team.pawns[i].name = "Pawn"+i+"_"+ team.teamEnum;
        }
    }

    #endregion

    #region game management

    public void PlayerMove(Vector2 origin, Vector2 destination)
    {
        cancelledMoves.Clear();
        unpromoted.Clear();
        Move(origin, destination);
    }

    internal void Move(Vector2 origin, Vector2 destination)
    {
        Move((int) origin.y, (int) origin.x, (int)destination.y, (int)destination.x);
    }

    void Move(int originLine, int originColumn, int destinationLine, int destinationColumn)
    {
        ChessPiece movedPiece = board.getBox(originColumn, originLine).piece;
        movedPiece.Move(board, destinationLine, destinationColumn);
    }

    public void CancelLastMove(bool saveInCancelledMoves)
    {
        reverted.Clear();
        liberated = null;
        if (game.Count == 0) return;
        Move toCancel = game[game.Count - 1];
        //get the moved piece
        ChessPiece movedPiece = toCancel.movingPiece;
        TeamManager playingTeam = movedPiece.team;
        //if there was a promotion during this move
        //unpromote the piece in pawn
        if (toCancel.promotionType != PieceType.Pawn)
        {
            beforePromotion = board.getBox(toCancel.destination).piece;

            //manage unpromotion
            playingTeam.pieces.Remove(beforePromotion);
            switch (beforePromotion)
            {
                case Rook rook:
                    playingTeam.rooks.Remove(rook);
                    break;
                case Bishop bishop:
                    playingTeam.bishops.Remove(bishop);
                    break;
                case Queen queen:
                    playingTeam.queens.Remove(queen);
                    break;
                case Knight knight:
                    playingTeam.knights.Remove(knight);
                    break;
                default:
                    throw new System.Exception("The unpromoted piece must be a rook, a knight, a bishop or a queen");
            }
            promoted.RemoveAt(promoted.Count - 1);
            unpromoted.Add(beforePromotion);

            afterPromotion = toCancel.movingPiece;
            Debug.Log("CANCELLATION " + movedPiece.name + " " + beforePromotion.name + " " + afterPromotion.name+" "+ playingTeam.teamEnum);
            
            board.SetPieceOnBox(afterPromotion, afterPromotion.currentPosition, afterPromotion.team);
        }
        //make the reverse move
        movedPiece.ForcedReverseMove(board,toCancel);
        if (saveInCancelledMoves)
        {
            //add cancelled move to the cancelled move list
            cancelledMoves.Add(toCancel);
        }
        else
        {
            ResetMoveLists();
        }
        //remove the cancelled move from the game move list
        game.RemoveAt(game.Count - 1);
    }

    public void PlayLastCancelledMove()
    {
        if (cancelledMoves.Count == 0) return;

        Move lastCancelledMove = cancelledMoves[cancelledMoves.Count - 1];
        cancelledMoves.RemoveAt(cancelledMoves.Count-1);
        lastCancelledMove.movingPiece.ForcedMove(board, lastCancelledMove);

        //this is a promotion move
        if (lastCancelledMove.promotionType != PieceType.Pawn)
        {
            beforePromotion = lastCancelledMove.movingPiece;
            afterPromotion = unpromoted[unpromoted.Count - 1];

            TeamManager playingTeam = beforePromotion.team;

            //manage repromotion
            playingTeam.pieces.Remove(beforePromotion);
            playingTeam.pawns.Remove((Pawn)beforePromotion);
            promoted.Add(beforePromotion);
            unpromoted.RemoveAt(unpromoted.Count-1);
            afterPromotion.hasMoved = true;

            board.SetPieceOnBox(afterPromotion, lastCancelledMove.destination, afterPromotion.team);
        }
        game.Add(lastCancelledMove);
    }

    public void StartNextTurn()
    {
        ResetMoveLists();
        whoIsPlaying = whoIsPlaying.other;
        bool hasValidMove = CalculateAvaibleMoves(false);

        if (!hasValidMove)
        {
            winningTeam = whoIsPlaying.other;  
        }
    }

    bool CalculateAvaibleMoves(bool isSimulation)
    {
        bool hasValidMove = false;
        foreach (ChessPiece piece in whoIsPlaying.pieces)
        {
            piece.CalculateAvailableDestinations(isSimulation);
            if (piece.availableDestinations.Count > 0)
            {
                hasValidMove = true;
            }
        }
        //reset enPassant for Pawns
        foreach(Pawn pawn in whoIsPlaying.pawns)
        {
            pawn.enPassantAllowed = false;
        }

        return hasValidMove;
    }

    public void Promote(PieceType type)
    {
        Move lastMove = game[game.Count - 1];
        lastMove.promotionType = type;
        beforePromotion = waitingPromotion;
        promoted.Add(waitingPromotion);
        afterPromotion = board.PromotePawn(waitingPromotion.currentPosition,type);
    }

    void ResetMoveLists()
    {
        movedDuringThisTurn.Clear();
        capturedDuringThisTurn = null;
        waitingPromotion = null;
        winningTeam = null;
        reverted.Clear();
        liberated = null;
        beforePromotion = null;
        afterPromotion = null;
    }
    #endregion

    public ChessPiece GetPieceAtPosition(Vector2 position)
    {
        return board.getBox(position).piece;
    }

    public bool IsValidCoordinate(Vector2 position)
    {
        return position.x < 8 && position.x >= 0 && position.y < 8 && position.y >= 0;
    }

    public bool IsValidCoordinate(int column, int line)
    {
        return column < 8 && column >= 0 && line < 8 && line >= 0;
    }

    public bool hasMovedInPreviousMove(ChessPiece piece)
    {
        //exclude the last move
        for (int i = 0; i < game.Count-1; i++)
        {
            if (game[i].movingPiece == piece || game[i].castlingPiece == piece)
            {
                return true;
            }
        }

        return false;
    }

    public Move GetMyPenultimateMove(ChessPiece piece)
    {
        for (int i = game.Count-2; i >= 0; i--)
        {
            if (game[i].movingPiece == piece || game[i].castlingPiece == piece)
            {
                return game[i];
            }
        }

        return null;
    }

}

public enum PieceType
{
    King,
    Queen,
    Knight,
    Rook,
    Bishop,
    Pawn
}
