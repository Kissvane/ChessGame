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

    Board board;

    //construct the board
    public void ConstructBoard()
    {
        bool isWhite = true;
        List<List<ChessboardBoxData>> chessboardBoxes = new List<List<ChessboardBoxData>>();
        for (int caseColumn = 0; caseColumn < 8; caseColumn++)
        {
            List<ChessboardBoxData> currentColumn = new List<ChessboardBoxData>();
            for (int caseLine = 0; caseLine < 8; caseLine++)
            {
                Vector3 offset = new Vector3(caseSize * caseColumn, 0f, caseSize * caseLine);
                GameObject chessboardBox = Instantiate(casePrefab, boardOrigin + offset, Quaternion.identity, transform);
                ChessboardBoxData box = new ChessboardBoxData(isWhite ? whiteColor : blackColor, caseLine, caseColumn, chessboardBox.GetComponent<Renderer>(),chessboardBox.transform);
                box.SetColor(isWhite ? whiteColor : blackColor);
                currentColumn.Add(box);
                isWhite = !isWhite;
            }
            chessboardBoxes.Add(currentColumn);
            isWhite = !isWhite;
        }

        board = new Board(chessboardBoxes);
        MyEventSystem.instance.Set("Board",board);
    }

    //position a piece on the board
    public void SetPieceOnBox(ChessPiece piece, int lineDestination, int columnDestination, TeamManager team, string pieceType)
    {
        //get the chessboard box
        ChessboardBoxData target = board.getBox(columnDestination,lineDestination); 

        //assign a piece to the targeted chessboard box
        target.piece = piece;
        //create the piece in 3D
        GameObject toInstantiate = null;
        switch (pieceType)
        {
            case "K":
                toInstantiate = KingPfb;
                break;
            case "Q":
                toInstantiate = QueenPfb;
                break;
            case "KN":
                toInstantiate = KnightPfb;
                break;
            case "F":
                toInstantiate = FoolPfb;
                break;
            case "T":
                toInstantiate = TowerPfb;
                break;
            default:
                toInstantiate = PawnPfb;
                break;
        }

        GameObject instantiated = Instantiate(toInstantiate, target.boxTransform.position, Quaternion.identity, transform);
        instantiated.transform.position += Vector3.up * 0.125f;
        instantiated.transform.localScale = new Vector3(pawnSize, instantiated.transform.localScale.y, pawnSize);

        //assign the piece team
        piece.team = team;
        piece.pawnRenderer.material.color = team.pawnColor;
        piece.iconRenderer.color = team.spriteColor;
        piece.currentPosition = new Vector2(columnDestination, lineDestination);

        //add the piece in team piece's list
        team.piecesObjects.Add(piece,instantiated);
    }
}
