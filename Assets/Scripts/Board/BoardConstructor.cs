using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardConstructor : MonoBehaviour
{
    public Color blackColor;
    public Color whiteColor;
    public Vector3 boardOrigin = Vector3.zero;
    public float caseSize = 0.5f;
    public float pawnSize = 0.5f;
    public GameObject casePrefab;

    public GameObject KingPfb;
    public GameObject QueenPfb;
    public GameObject FoolPfb;
    public GameObject TowerPfb;
    public GameObject PawnPfb;
    public GameObject KnightPfb;

    //construct the board
    public void ConstructBoard()
    {
        bool isWhite = true;
        List<List<ChessboardBox>> chessboardBoxes = new List<List<ChessboardBox>>();
        for (int caseColumn = 0; caseColumn < 8; caseColumn++)
        {
            List<ChessboardBox> currentColumn = new List<ChessboardBox>();
            for (int caseLine = 0; caseLine < 8; caseLine++)
            {
                Vector3 offset = new Vector3(caseSize * caseColumn, 0f, caseSize * caseLine);
                GameObject chessboardBox = Instantiate(casePrefab, boardOrigin + offset, Quaternion.identity, transform);
                ChessboardBox box = chessboardBox.GetComponent<ChessboardBox>();
                box.SetColor(isWhite ? whiteColor : blackColor);
                box.SetSize(caseSize);
                box.line = caseLine;
                box.column = caseColumn;
                currentColumn.Add(box);
                isWhite = !isWhite;
            }
            chessboardBoxes.Add(currentColumn);
            isWhite = !isWhite;
        }

        //send an event with chessboardBoxes
    }

    public void SetPieceOnBox(ChessPiece piece, int lineDestination, int columnDestination, TeamManager team)
    {
        GenericDictionary data = new GenericDictionary()
            .Set("lineDestination", lineDestination)
            .Set("columnDestination", columnDestination);

        ChessboardBox target = MyEventSystem.instance.Get("ChessboardBox", data);
        target.piece = piece;
        piece.transform.position = target.transform.position;
        piece.transform.position += Vector3.up * 0.125f;
        piece.transform.localScale = new Vector3(pawnSize, piece.transform.localScale.y, pawnSize);
        piece.team = team;
        piece.pawnRenderer.material.color = team.pawnColor;
        piece.iconRenderer.color = team.spriteColor;
    }
}
