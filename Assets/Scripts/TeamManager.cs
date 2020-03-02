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
    public Dictionary<ChessPiece, GameObject> capturedPieces =  new Dictionary<ChessPiece, GameObject>();
    //public List<ChessPiece> pieces;
    public King king;
    public List<Pawn> pawns = new List<Pawn>();
    public List<Queen> queens = new List<Queen>();
    public List<Knight> knights = new List<Knight>();
    public List<Fool> fools = new List<Fool>();
    public List<Tower> towers = new List<Tower>();
    public Dictionary<ChessPiece, GameObject> piecesObjects = new Dictionary<ChessPiece, GameObject>();

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

    public void Capture(ChessPiece piece, GameObject captured)
    {
        capturedPieces.Add(piece, captured);
        //TODO physical case
    }

    
}
