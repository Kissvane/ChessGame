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
    #region UnityMethods
    public void Start()
    {
        StartGame();
    }
    #endregion

    #region Logic
    public void StartGame()
    {
        ChessEngine.instance.StartAGame();
        Linker.instance.physicalBoardManager.ConstructPhysicalBoard();
    }

    public void SelectPieceToMove(Vector2 selected)
    {
        originMove = selected;
        movingPiece = ChessEngine.instance.GetPieceAtPosition((int)originMove.x, (int)originMove.y);
        selectedAvailableMoves = movingPiece.availableDestinations;
        Linker.instance.physicalBoardManager.UpdateCollidersForThisTurn();
        Linker.instance.physicalBoardManager.EnablePossibleMoveColliders(movingPiece);
    }

    public void MakeAMove(Vector2 destination)
    {
        ChessEngine.instance.Move((int)originMove.y, (int)originMove.x, (int)destination.y, (int)destination.x);
        MovePhysicalPiece();
        CapturePhysicalPiece();

        if (ChessEngine.instance.waitingPromotion != null)
        {
            PromotionChoice();
            return;
        }

        NextTurn();
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

    void WaitForInputs()
    {
        originMove = new Vector2(-1, -1);
        movingPiece = null;
        Linker.instance.physicalBoardManager.UpdateCollidersForThisTurn();
    }
    #endregion

    #region pawn promotion
    public void PromotionChoice()
    {
        //show choice UI
    }

    public void Promotion(PieceType type)
    {
        ChessEngine.instance.Promote(type);
        NextTurn();
    }

    #endregion

    #region visualization
    void ShowWinner(TeamManager winningTeam)
    {
        Debug.Log(winningTeam.teamEnum+" win !");
    }

    void MovePhysicalPiece()
    {
        foreach (ChessPiece piece in ChessEngine.instance.movedDuringThisTurn)
        {
            GameObject toMove = piece.team.piecesObjects[piece];
            Linker.instance.physicalBoardManager.MovePieceOnBox(toMove,piece.currentPosition);
        }
    }

    void CapturePhysicalPiece()
    {
        ChessPiece piece = ChessEngine.instance.capturedDuringThisTurn;
        if (piece != null)
        {
            ChessEngine.instance.capturedDuringThisTurnGameobject.SetActive(false);
        }
    }
    #endregion

}
