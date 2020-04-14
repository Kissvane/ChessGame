using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public Camera myCamera;
    public LayerMask mask;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = myCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit result;
            if (Physics.Raycast(ray, out result,50f,mask))
            {
                ManageSelection(result.transform.gameObject);
            }
        }
        if (Input.GetMouseButtonDown(1))
        {
            Linker.instance.gameManager.WaitForInputs();
        }
    }

    private void ManageSelection(GameObject box)
    {
        Vector2 position = box.GetComponent<PhysicalChessboardBox>().position;
        ChessPiece piece = ChessEngine.instance.GetPieceAtPosition(position);
        if (piece != null)
        {
            if (Linker.instance.gameManager.originMove == new Vector2(-1, -1))
            {
                Linker.instance.gameManager.SelectPieceToMove(position);
            }
            else
            {
                if (Linker.instance.gameManager.movingPiece.availableDestinations.Contains(position))
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
}
