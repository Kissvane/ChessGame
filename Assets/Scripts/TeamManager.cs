using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Team
{
    White = 0,
    Black = 1,
}

public class TeamManager
{
    public TeamManager other;
    public Team teamEnum;
    public Color pawnColor;
    public Color spriteColor;
    public Dictionary<ChessPiece, GameObject> capturedPieces =  new Dictionary<ChessPiece, GameObject>();
    //public List<ChessPiece> pieces;
    public King king;
    public List<Pawn> pawns = new List<Pawn>();
    public List<Queen> queens = new List<Queen>();
    public List<Knight> knights = new List<Knight>();
    public List<Fool> fools = new List<Fool>();
    public List<Tower> towers = new List<Tower>();
    public Dictionary<ChessPiece, GameObject> piecesObjects = new Dictionary<ChessPiece, GameObject>();

    public TeamManager(string team, Color pawnColor, Color spriteColor)
    {
        if (team == "white")
        {
            this.pawnColor = pawnColor;
            this.spriteColor = spriteColor;
        }
    }

    public void Capture(ChessPiece piece, GameObject captured)
    {
        capturedPieces.Add(piece, captured);
        //TODO physical case
    }

    //instantiate and set the pieces
    public void positionTeam(BoardConstructor constructor)
    {
        int KingLine = -1;
        int PawnLine = -1;

        if (teamEnum == Team.White)
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

        king = new King();
        constructor.SetPieceOnBox(king, KingLine, KingColumn,this, "K");

        Queen queen = new Queen();
        queens.Add(queen);
        constructor.SetPieceOnBox(queen, KingLine, KingColumn - 1, this, "Q");

        Fool fool = new Fool();
        fools.Add(fool);
        constructor.SetPieceOnBox(fool, KingLine, KingColumn - 2, this,"F");

        Fool fool2 = new Fool();
        fools.Add(fool2);
        constructor.SetPieceOnBox(fool2, KingLine, KingColumn + 1, this,"F");

        Knight knight = new Knight();
        knights.Add(knight);
        constructor.SetPieceOnBox(knight, KingLine, KingColumn - 3, this,"KN");

        Knight knight2 = new Knight();
        knights.Add(knight2);
        constructor.SetPieceOnBox(knight2, KingLine, KingColumn + 2, this,"KN");

        Tower tower = new Tower();
        towers.Add(tower);
        constructor.SetPieceOnBox(tower, KingLine, KingColumn - 4, this,"T");

        Tower tower2 = new Tower();
        towers.Add((Tower)tower2);
        constructor.SetPieceOnBox(tower2, KingLine, KingColumn + 3, this,"T");

        Pawn pawn = null;
        for (int i = 0; i < 8; i++)
        {
            pawn = new Pawn();
            pawns.Add(pawn);
            constructor.SetPieceOnBox(pawn, PawnLine, i, this,"P");
        }
    }
}
