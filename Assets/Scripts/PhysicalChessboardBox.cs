using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicalChessboardBox : MonoBehaviour
{
    public Vector2 position;
    public Collider myCollider;
    public Renderer myRenderer;
    public bool isWhite = false;

    private void OnMouseDown()
    {
        ChessPiece piece = ChessEngine.instance.GetPieceAtPosition((int)position.x,(int)position.y);
        if (piece != null)
        {
            if (Linker.instance.gameManager.originMove == new Vector2(-1,-1))
            {
                Linker.instance.gameManager.SelectPieceToMove(position);
            }
            else
            {
                if(Linker.instance.gameManager.movingPiece.availableDestinations.Contains(position))
                {
                    Linker.instance.gameManager.MakeAMove(position);
                }
                else
                {
                    Linker.instance.gameManager.SelectPieceToMove(position);
                }
            }
        }
        else
        {
            if (Linker.instance.gameManager.originMove != new Vector2(-1, -1))
            {
                Linker.instance.gameManager.MakeAMove(position);
            }
        }
    }

    public void SetHilightedColor()
    {
        myRenderer.material.EnableKeyword("_EMISSION");
    }

    public void ResetColor()
    {
        myRenderer.material.DisableKeyword("_EMISSION");
    }
}
