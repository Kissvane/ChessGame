using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChessboardBoxData
{
    public int line = 0;
    public int column = 0;
    public ChessPiece piece = null;
    public ChessColor caseColor;

    public ChessboardBoxData(ChessColor caseColor, int line, int column)
    {
        this.caseColor = caseColor;
        this.line = line;
        this.column = column;
    }
}
