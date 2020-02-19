using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public Color blackColor;
    public Color whiteColor;
    public Vector3 boardOrigin = Vector3.zero;
    public float caseSize = 0.5f;
    public float pawnSize = 0.5f;
    public GameObject casePrefab;
    public List<List<ChessboardBox>> chessboardBoxes;

    public GameObject KingPfb;
    public GameObject QueenPfb;
    public GameObject FoolPfb;
    public GameObject TowerPfb;
    public GameObject PawnPfb;
    public GameObject KnightPfb;

    private void Start()
    {
        MyEventSystem.instance.RegisterToEvent("ShowValidMoves", this, ShowValidMoves);
    }

    //test if a movement is valid
    bool isValidMovement(ChessPiece piece, int testedLine, int testedColumn)
    {
        // the destination is out of the board
        if (testedLine < 0 || testedLine > 7 || testedColumn < 0 || testedColumn > 7) return false;
        //castling case


        return true;
    }

    void ShowValidMoves(string name, GenericDictionary args)
    {
        ChessPiece piece = args.Get("movedPiece");
        int originLine = args.Get("originLine");
        int originColumn = args.Get("originColumn");
        //calculte coord to test
        foreach (Vector2 direction in piece.directionsAndDestination.Keys)
        {

            if (piece.directionsAndDestination[direction] != null)
            {

            }
        }
    }

    public void SetPieceOnBox(ChessPiece piece,int lineDestination, int columnDestination, TeamManager team)
    {
        ChessboardBox target = chessboardBoxes[lineDestination][columnDestination];
        target.piece = piece;
        piece.transform.position = target.transform.position;
        piece.transform.position += Vector3.up * 0.125f;
        piece.transform.localScale = new Vector3(pawnSize, piece.transform.localScale.y, pawnSize);
        piece.team = team;
        piece.pawnRenderer.material.color = team.pawnColor;
        piece.iconRenderer.color = team.spriteColor;
    }

    public void Move(Vector2 origin, Vector2 destination)
    {

    }

    //construct the board
    public void ConstructBoard()
    {
        bool isWhite = true;
        chessboardBoxes = new List<List<ChessboardBox>>();
        for (int caseColumn = 0; caseColumn < 8; caseColumn++)
        {
            List<ChessboardBox> currentColumn = new List<ChessboardBox>();
            for (int caseLine = 0; caseLine < 8; caseLine++)
            {
                Vector3 offset = new Vector3(caseSize*caseColumn,0f,caseSize*caseLine);
                GameObject chessboardBox = Instantiate(casePrefab, boardOrigin+offset, Quaternion.identity,transform);
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
    }

    
}
