using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Logic and data models to play a chess game
/// </summary>
public class ChessEngine
{
    #region variables
    //public bool whiteIsPlaying = true;
    public TeamManager whiteTeam;
    public TeamManager blackTeam;
    public TeamManager whoIsPlaying;
    public Board board;
    public List<ChessPiece> movedDuringThisTurn = new List<ChessPiece>();
    public ChessPiece capturedDuringThisTurn = null;
    public GameObject capturedDuringThisTurnGameobject = null;
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
        CalculateAvaibleMoves();
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

        Queen queen = new Queen();
        team.queens.Add(queen);
        board.SetPieceOnBox(queen, KingLine, KingColumn - 1, team);

        Fool fool = new Fool();
        team.fools.Add(fool);
        board.SetPieceOnBox(fool, KingLine, KingColumn - 2, team);

        Fool fool2 = new Fool();
        team.fools.Add(fool2);
        board.SetPieceOnBox(fool2, KingLine, KingColumn + 1, team);

        Knight knight = new Knight();
        team.knights.Add(knight);
        board.SetPieceOnBox(knight, KingLine, KingColumn - 3, team);

        Knight knight2 = new Knight();
        team.knights.Add(knight2);
        board.SetPieceOnBox(knight2, KingLine, KingColumn + 2, team);

        Tower tower = new Tower();
        team.towers.Add(tower);
        board.SetPieceOnBox(tower, KingLine, KingColumn - 4, team);

        Tower tower2 = new Tower();
        team.towers.Add((Tower)tower2);
        board.SetPieceOnBox(tower2, KingLine, KingColumn + 3, team);

        Pawn pawn = null;
        for (int i = 0; i < 8; i++)
        {
            pawn = new Pawn();
            team.pawns.Add(pawn);
            board.SetPieceOnBox(pawn, PawnLine, i, team);
        }
    }

    #endregion

    #region game management

    public void Move(int originLine, int originColumn, int destinationLine, int destinationColumn)
    {
        ChessPiece pieceToMove = board.getBox(originColumn, originLine).piece;
        if (whoIsPlaying.piecesObjects.ContainsKey(pieceToMove))
        {
            pieceToMove.Move(board, originLine, originColumn, destinationLine, destinationColumn);
        }
    }

    public void StartNextTurn()
    {
        ResetMoveLists();
        whoIsPlaying = whoIsPlaying.other;
        bool hasValidMove = CalculateAvaibleMoves();

        if (!hasValidMove)
        {
            winningTeam = whoIsPlaying.other;  
        }
    }

    bool CalculateAvaibleMoves()
    {
        bool hasValidMove = false;
        foreach (ChessPiece piece in whoIsPlaying.piecesObjects.Keys)
        {
            piece.CalculateAvailableDestinations();
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
        board.PromotePawn(waitingPromotion.currentPosition,type);
    }

    void ResetMoveLists()
    {
        movedDuringThisTurn.Clear();
        capturedDuringThisTurn = null;
        capturedDuringThisTurnGameobject = null;
        waitingPromotion = null;
        winningTeam = null;
    }
    #endregion

    public ChessPiece GetPieceAtPosition(int column, int line)
    {
        return board.getBox(column, line).piece;
    }

    public bool IsValidCoordinate(Vector2 position)
    {
        return position.x < 8 && position.x >= 0 && position.y < 8 && position.y >= 0;
    }

    public bool IsValidCoordinate(int column, int line)
    {
        return column < 8 && column >= 0 && line < 8 && line >= 0;
    }

}

public enum PieceType
{
    King,
    Queen,
    Knight,
    Tower,
    Fool,
    Pawn
}
