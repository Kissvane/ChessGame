using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameManager : MonoBehaviour
{
    //x for column and y for line
    public Vector2 originMove = new Vector2(-1,-1);
    //public Vector2 destinationMove = new Vector2(-1, -1);
    public ChessPiece movingPiece = null;
    public List<Vector2> selectedAvailableMoves;
    public List<Move> moves;
    public List<Move> cancelledMoves;
    public List<string> WhiteCapturedPieceName;
    public List<string> BlackCapturedPieceName;
    public GameObject promotionUI;
    public bool gameOver = false;
    
    #region UnityMethods
    public void Start()
    {
        StartGame();
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            ChessEngine.instance.CancelLastMove(true);
            //manage physical unpromotion
            if (ChessEngine.instance.beforePromotion != null)
            {
                ActivateGoodPhysicalPiece(ChessEngine.instance.beforePromotion, ChessEngine.instance.afterPromotion);
            }
            RevertPhysicalPiece();
            LiberatePhysicalPiece();
            
            //manage next turn
            NextTurn();
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            ChessEngine.instance.PlayLastCancelledMove();
            MovePhysicalPiece();
            CapturePhysicalPiece();
            //manage promotion
            if (ChessEngine.instance.beforePromotion != null)
            {
                ActivateGoodPhysicalPiece(ChessEngine.instance.beforePromotion, ChessEngine.instance.afterPromotion);
            }

            //manage next turn
            NextTurn();
        }
    }
    #endregion

    #region Logic
    public void StartGame()
    {
        ChessEngine.instance.StartAGame();
        Linker.instance.physicalBoardManager.ConstructPhysicalBoard();
        WaitForInputs();
    }

    public void SelectPieceToMove(Vector2 selected)
    {
        originMove = selected;
        movingPiece = ChessEngine.instance.GetPieceAtPosition(originMove);
        selectedAvailableMoves = movingPiece.availableDestinations;
        Linker.instance.physicalBoardManager.UpdateCollidersForThisTurn();
        Linker.instance.physicalBoardManager.EnablePossibleMoveColliders(movingPiece);
    }

    public void MakeAMove(Vector2 destination)
    {
        ChessEngine.instance.PlayerMove(originMove, destination);
        MovePhysicalPiece();
        CapturePhysicalPiece();

        if (ChessEngine.instance.waitingPromotion != null)
        {
            PromotionChoice();
            return;
        }

        NextTurn();
        moves.Clear();
        moves.AddRange(ChessEngine.instance.game);
        cancelledMoves.Clear();
        cancelledMoves.AddRange(ChessEngine.instance.cancelledMoves);
        foreach (ChessPiece piece in ChessEngine.instance.blackTeam.capturedPieces)
        {
            if(!BlackCapturedPieceName.Contains(piece.name)) BlackCapturedPieceName.Add(piece.name);
        }
        foreach (ChessPiece piece in ChessEngine.instance.whiteTeam.capturedPieces)
        {
            if (!WhiteCapturedPieceName.Contains(piece.name)) WhiteCapturedPieceName.Add(piece.name);
        }
    }

    public void NextTurn()
    {
        ChessEngine.instance.StartNextTurn();
        //show winner if necessary
        if (ChessEngine.instance.winningTeam != null)
        {
            ShowWinner(ChessEngine.instance.winningTeam);
            return;
        }

        WaitForInputs();
    }

    public void WaitForInputs()
    {
        originMove = new Vector2(-1, -1);
        movingPiece = null;
        if (!gameOver) Linker.instance.physicalBoardManager.UpdateCollidersForThisTurn();
    }
    #endregion

    #region pawn promotion
    public void PromotionChoice()
    {
        //show choice UI
        promotionUI.SetActive(true);
    }

    public void KnightPromotion()
    {
        Promotion(PieceType.Knight);
    }

    public void QueenPromotion()
    {
        Promotion(PieceType.Queen);
    }

    public void RookPromotion()
    {
        Promotion(PieceType.Rook);
    }

    public void BishopPromotion()
    {
        Promotion(PieceType.Bishop);
    }

    public void Promotion(PieceType type)
    {
        //save promoted gameobject
        Linker.instance.physicalBoardManager.promoted.Add(Linker.instance.physicalBoardManager.piecesToObjects[ChessEngine.instance.waitingPromotion]);
        promotionUI.SetActive(false);
        ChessEngine.instance.Promote(type);
        
        TransformPhysicalPiece(ChessEngine.instance.beforePromotion, ChessEngine.instance.afterPromotion);
        NextTurn();
    }

    #endregion

    #region visualization
    void ShowWinner(TeamManager winningTeam)
    {
        gameOver = true;
        Linker.instance.physicalBoardManager.DisableAllColliders();
        Debug.Log(winningTeam.teamEnum+" win !");
    }

    void MovePhysicalPiece()
    {
        foreach (ChessPiece piece in ChessEngine.instance.movedDuringThisTurn)
        {
            Linker.instance.physicalBoardManager.MovePieceOnBox(piece,piece.currentPosition);
        }
    }

    void CapturePhysicalPiece()
    {
        if (ChessEngine.instance.capturedDuringThisTurn != null)
        {
            Linker.instance.physicalBoardManager.piecesToObjects[ChessEngine.instance.capturedDuringThisTurn].SetActive(false);
        }
    }

    void RevertPhysicalPiece()
    {
        foreach (ChessPiece piece in ChessEngine.instance.reverted)
        {
            Linker.instance.physicalBoardManager.MovePieceOnBox(piece, piece.currentPosition);
        }
    }

    void LiberatePhysicalPiece()
    {
        if (ChessEngine.instance.liberated != null)
        {
            Linker.instance.physicalBoardManager.piecesToObjects[ChessEngine.instance.liberated].SetActive(true);
            Linker.instance.physicalBoardManager.MovePieceOnBox(ChessEngine.instance.liberated, ChessEngine.instance.liberated.currentPosition);
        }
    }

    void TransformPhysicalPiece(ChessPiece oldPiece, ChessPiece newPiece)
    {
        string oldName = Linker.instance.physicalBoardManager.piecesToObjects[oldPiece].name;
        Linker.instance.physicalBoardManager.piecesToObjects[oldPiece].SetActive(false);
        string promotionType = "";
        switch (newPiece)
        {
            case Rook rook:
                promotionType = "Rook";
                break;
            case Queen queen:
                promotionType = "Queen";
                break;
            case Knight knight:
                promotionType = "Knight";
                break;
            case Bishop bishop:
                promotionType = "Bishop";
                break;
            default:
                promotionType = "Pawn";
                break;
        }
        Linker.instance.physicalBoardManager.SetPieceOnBox(newPiece, oldName+"_Promoted_"+promotionType);
    }

    void ActivateGoodPhysicalPiece(ChessPiece oldPiece, ChessPiece newPiece)
    {
        Debug.Log(oldPiece.name+" "+newPiece.name);
        Linker.instance.physicalBoardManager.piecesToObjects[oldPiece].SetActive(false);
        Linker.instance.physicalBoardManager.piecesToObjects[newPiece].SetActive(true);
        Linker.instance.physicalBoardManager.MovePieceOnBox(newPiece, newPiece.currentPosition);
    }
    #endregion

}
