using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicalBoardManager : MonoBehaviour
{
    #region Variables
    public Color blackColor;
    public Color whiteColor;
    public Color hilightedColor;
    public Vector3 boardOrigin = Vector3.zero;
    public float caseSize = 0.5f;
    public float pawnSize = 0.5f;
    public GameObject casePrefab;

    public GameObject kingPfb;
    public GameObject queenPfb;
    public GameObject bishopPfb;
    public GameObject rookPfb;
    public GameObject pawnPfb;
    public GameObject knightPfb;

    public List<List<PhysicalChessboardBox>> boxes = new List<List<PhysicalChessboardBox>>();
    public Dictionary<ChessPiece, GameObject> piecesToObjects = new Dictionary<ChessPiece, GameObject>();
    public List<GameObject> promoted = new List<GameObject>();
    public Material boxMaterial;
    Material whiteMaterial;
    Material blackMaterial;
    #endregion

    #region Initialization
    //construct the board
    public void ConstructPhysicalBoard()
    {
        whiteMaterial = new Material(boxMaterial);
        whiteMaterial.color = whiteColor;
        blackMaterial = new Material(boxMaterial);
        blackMaterial.color = blackColor;

        bool isWhite = true;
        boxes = new List<List<PhysicalChessboardBox>>();
        for (int caseColumn = 0; caseColumn < 8; caseColumn++)
        {
            List<PhysicalChessboardBox> currentColumn = new List<PhysicalChessboardBox>();
            for (int caseLine = 0; caseLine < 8; caseLine++)
            {
                Vector3 offset = new Vector3(caseSize * caseColumn, 0f, caseSize * caseLine);
                GameObject chessboardBox = Instantiate(casePrefab, boardOrigin + offset, Quaternion.identity, transform);
                chessboardBox.transform.localScale = new Vector3(caseSize,chessboardBox.transform.localScale.y,caseSize);
                chessboardBox.GetComponent<Renderer>().sharedMaterial = isWhite ? whiteMaterial : blackMaterial;
                PhysicalChessboardBox physicalChessboardBox = chessboardBox.GetComponent<PhysicalChessboardBox>();
                physicalChessboardBox.position = new Vector2(caseColumn,caseLine);
                physicalChessboardBox.isWhite = isWhite;
                physicalChessboardBox.ResetColor();
                currentColumn.Add(physicalChessboardBox);
                isWhite = !isWhite;
            }
            boxes.Add(currentColumn);
            isWhite = !isWhite;
        }

        //Instantiate and position pieces
        PositionTeam(ChessEngine.instance.whiteTeam);
        PositionTeam(ChessEngine.instance.blackTeam);
    }

    void PositionTeam(TeamManager team)
    {
        SetPieceOnBox(team.king,"King"+team.teamEnum);
        SetPieceOnBox(team.queens[0], "Queen" + team.teamEnum);
        SetPieceOnBox(team.bishops[0], "Bishop" + team.teamEnum + "1");
        SetPieceOnBox(team.bishops[1], "Bishop" + team.teamEnum + "2");
        SetPieceOnBox(team.rooks[0], "Tower" + team.teamEnum + "1");
        SetPieceOnBox(team.rooks[1], "Tower" + team.teamEnum + "2");
        SetPieceOnBox(team.knights[0], "Knight" + team.teamEnum + "1");
        SetPieceOnBox(team.knights[1], "Knight" + team.teamEnum + "2");
        int pawnCount = 0;
        foreach (Pawn pawn in team.pawns)
        {
            pawnCount++;
            SetPieceOnBox(pawn, "Pawn" + team.teamEnum + pawnCount);
        }
    }

    //position a piece on the board
    //make the link between physical piece and engine piece
    public void SetPieceOnBox(ChessPiece piece, string pieceName)
    {
        //get the chessboard box
        GameObject target = boxes[(int)piece.currentPosition.x][(int)piece.currentPosition.y].gameObject; 

        //create the piece in 3D
        GameObject toInstantiate = null;
        switch (piece)
        {
            case King king:
                toInstantiate = kingPfb;
                break;
            case Queen queen:
                toInstantiate = queenPfb;
                break;
            case Knight knight:
                toInstantiate = knightPfb;
                break;
            case Bishop bishop:
                toInstantiate = bishopPfb;
                break;
            case Rook rook:
                toInstantiate = rookPfb;
                break;
            default:
                toInstantiate = pawnPfb;
                break;
        }

        GameObject instantiated = Instantiate(toInstantiate, target.transform.position, Quaternion.identity, transform);
        instantiated.transform.position += Vector3.up * 0.125f;
        instantiated.transform.localScale = new Vector3(pawnSize, instantiated.transform.localScale.y, pawnSize);
        instantiated.name = pieceName;

        //set piece color
        if (piece.team.teamEnum == ChessColor.White)
        {
            instantiated.GetComponent<Renderer>().material.color = Color.white;
            instantiated.transform.GetChild(0).GetComponent<SpriteRenderer>().color = Color.black;
        }
        else
        {
            instantiated.GetComponent<Renderer>().material.color = Color.black;
            instantiated.transform.GetChild(0).GetComponent<SpriteRenderer>().color = Color.white;
        }

        //add the piece in team piece's list
        //piece.team.piecesObjects[piece] = instantiated;
        piecesToObjects.Add(piece,instantiated);
    }
    #endregion

    public void MovePieceOnBox(ChessPiece toMove, Vector2 destination)
    {
        //Debug.Log(toMove.name);
        piecesToObjects[toMove].transform.position = boxes[(int)destination.x][(int)destination.y].transform.position + Vector3.up * 0.125f;
    }

    #region Colliders activation & disactivation
    public void UpdateCollidersForThisTurn()
    {
        DisableForbiddenColliders();
    }

    void DisableForbiddenColliders()
    {
        DisableAllColliders();
        EnablePlayingTeamColliders();
    }

    void EnablePlayingTeamColliders()
    {
        foreach (ChessPiece piece in ChessEngine.instance.whoIsPlaying.pieces)
        {
            if(piece.availableDestinations.Count > 0) EnableCollider(piece.currentPosition, true);
        }
    }

    public void EnablePossibleMoveColliders(ChessPiece piece)
    {
        foreach (Vector2 availableDestination in piece.availableDestinations)
        {
            EnableCollider(availableDestination, true);
            boxes[(int)availableDestination.x][(int)availableDestination.y].SetHilightedColor();
        }
    }

    public void DisableAllColliders()
    {
        foreach (List<PhysicalChessboardBox> list in boxes)
        {
            foreach (PhysicalChessboardBox box in list)
            {
                box.myCollider.enabled = false;
                box.ResetColor();
            }
        }
    }

    void EnableCollider(Vector2 position, bool enabled)
    {
        boxes[(int)position.x][(int)position.y].myCollider.enabled = enabled;
    }
    #endregion
}
