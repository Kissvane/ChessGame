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
    public bool whiteIsPlaying = true;
    public TeamManager whiteTeam;
    public TeamManager blackTeam;
    public Board board;
    #endregion

    #region singleton
    static ChessEngine _instance;
    public static ChessEngine instance { get { return _instance; } }
    ChessEngine()
    {
        _instance = this;
    }
    #endregion

    #region Initialization

    public void StartAGame()
    {
        board = ConstructBoard();
        whiteTeam = new TeamManager(ChessColor.White);
        blackTeam = new TeamManager(ChessColor.Black);
        positionTeam(whiteTeam);
        positionTeam(blackTeam);
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
    public void positionTeam(TeamManager team)
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
        throw new NotImplementedException();
    }

    #endregion

}
