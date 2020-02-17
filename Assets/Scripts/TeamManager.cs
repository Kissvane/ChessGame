using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Team
{
    White = 0,
    Black = 1,
}

public class TeamManager : MonoBehaviour
{
    public Team teamEnum;
    public Color pawnColor;
    public Color spriteColor;
    public List<Transform> capturedSpot; 

    public void positionTeam(Board board)
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

        GameObject kingObject = Instantiate(board.KingPfb,transform);
        ChessPiece King = kingObject.GetComponent<ChessPiece>();
        board.SetPieceOnBox(King, KingLine, KingColumn,this);

        GameObject queenObject = Instantiate(board.QueenPfb, transform);
        ChessPiece queen = queenObject.GetComponent<ChessPiece>();
        board.SetPieceOnBox(queen, KingLine, KingColumn - 1, this);

        GameObject foolObject = Instantiate(board.FoolPfb, transform);
        ChessPiece fool = foolObject.GetComponent<ChessPiece>();
        board.SetPieceOnBox(fool, KingLine, KingColumn - 2, this);

        GameObject foolObject2 = Instantiate(board.FoolPfb, transform);
        ChessPiece fool2 = foolObject2.GetComponent<ChessPiece>();
        board.SetPieceOnBox(fool2, KingLine, KingColumn + 1, this);

        GameObject knightObject = Instantiate(board.KnightPfb, transform);
        ChessPiece knight = knightObject.GetComponent<ChessPiece>();
        board.SetPieceOnBox(knight, KingLine, KingColumn - 3, this);

        GameObject knightObject2 = Instantiate(board.KnightPfb, transform);
        ChessPiece knight2 = knightObject2.GetComponent<ChessPiece>();
        board.SetPieceOnBox(knight2, KingLine, KingColumn + 2, this);

        GameObject towerObject = Instantiate(board.TowerPfb, transform);
        ChessPiece tower = towerObject.GetComponent<ChessPiece>();
        board.SetPieceOnBox(tower, KingLine, KingColumn - 4, this);

        GameObject towerObject2 = Instantiate(board.TowerPfb, transform);
        ChessPiece tower2 = towerObject2.GetComponent<ChessPiece>();
        board.SetPieceOnBox(tower2, KingLine, KingColumn + 3, this);

        GameObject pawnObject = null;
        ChessPiece pawn = null;
        for (int i = 0; i < 8; i++)
        {
            pawnObject = Instantiate(board.PawnPfb, transform);
            pawn = pawnObject.GetComponent<ChessPiece>();
            board.SetPieceOnBox(pawn, PawnLine, i, this);
        }
    }


}
