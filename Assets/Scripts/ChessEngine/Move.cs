using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Move
{
    public Vector2 origin = new Vector2(-1,-1);
    public Vector2 destination = new Vector2 (-1,-1);
    public ChessPiece movingPiece = null;
    public ChessPiece castlingPiece = null;
    public ChessPiece capturedPiece = null;
    public PieceType promotionType = PieceType.Pawn;

    public Move(Vector2 origin, Vector2 destination, ChessPiece movingPiece,ChessPiece affectedPiece = null,bool isCastling = false, PieceType promotionType = PieceType.Pawn)
    {
        this.origin = origin;
        this.destination = destination;
        this.movingPiece = movingPiece;
        if (isCastling) castlingPiece = affectedPiece;
        else capturedPiece = affectedPiece;
        this.promotionType = promotionType;
    }
}
