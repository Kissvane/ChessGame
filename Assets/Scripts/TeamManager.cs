using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ChessColor
{
    White = 0,
    Black = 1,
}

public class TeamManager
{
    public TeamManager other;
    public ChessColor teamEnum;
    public Vector2 teamForward;
    public List<ChessPiece> capturedPieces =  new List<ChessPiece>();
    public King king;
    public List<Pawn> pawns = new List<Pawn>();
    public List<Queen> queens = new List<Queen>();
    public List<Knight> knights = new List<Knight>();
    public List<Bishop> bishops = new List<Bishop>();
    public List<Rook> rooks = new List<Rook>();
    public List<ChessPiece> pieces = new List<ChessPiece>();
    public List<ChessPiece> movedPieces = new List<ChessPiece>();

    public TeamManager(ChessColor team)
    {
        teamEnum = team;
        if (team == ChessColor.White)
        {
            teamForward = Vector2.up;
        }
        else
        {
            teamForward = Vector2.down;
        }
    }

    public void Capture(ChessPiece piece)
    {
        capturedPieces.Add(piece);
    }

    public void Liberate(ChessPiece piece)
    {
        capturedPieces.Remove(piece);
    }
}
