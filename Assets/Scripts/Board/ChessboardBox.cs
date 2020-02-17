using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChessboardBox : MonoBehaviour
{
    public int line = 0;
    public int column = 0;
    public ChessPiece piece = null;
    public Color caseColor;
    public Renderer boxRenderer;
    public Transform boxTransform;

    public void SetColor(Color color)
    {
        caseColor = color;
        boxRenderer.material.color = caseColor;
    }

    public void SetSize(float size)
    {
        boxTransform.localScale = new Vector3(size, boxTransform.localScale.y, size);
    }

    public void OnMouseDown()
    {
        //if the player is selecting the piece to move
        if (MyEventSystem.instance.Get("originMove") == null && piece != null)
        { 
            if ((MyEventSystem.instance.Get("whiteIsPlaying") && piece.team.teamEnum == Team.White) 
            || (!MyEventSystem.instance.Get("whiteIsPlaying") && piece.team.teamEnum == Team.Black))
            {
                GenericDictionary data = new GenericDictionary().Set("originLine", line).Set("originColumn", column).Set("movedPiece",piece);
                MyEventSystem.instance.FireEvent("ShowValidMoves", data);
            }
        }
        //if the player is selecting the destination boardbox
        else if (MyEventSystem.instance.Get("pieceToMove") != null)
        {
            GenericDictionary data = new GenericDictionary().Set("destinationLine", line).Set("destinationColumn", column);
            //if the destination is valid
            if (MyEventSystem.instance.Get("isValidMovement",data))
            {
                //do the move
            }
        }
    }
}
