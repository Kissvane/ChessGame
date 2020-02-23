using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pawn : ChessPiece
{
    public bool enPassantAllowed = false;

    public override void SetMovementLimit()
    {
        isPawn = true;
        maxRange = 2;
        moveDirectionsAndBlockedState = new Dictionary<Vector2, bool>();
        moveDirectionsAndBlockedState.Add(team.teamEnum == Team.White ? Vector2.up : Vector2.down, false);
    }

    public override void Moved(Vector2 origin,Vector2 destination)
    {
        if (!hasMoved)
        {
            //if the first move is a 2 square move an enPassant capture is Possible during one turn 
            if(Mathf.Abs(destination.y - origin.y) == 2)
            {
                enPassantAllowed = true;
            }

            maxRange = 1;
        }
        else
        {
            enPassantAllowed = false;
        }
        base.Moved(origin, destination);
    }

    public override void Captured()
    {
        base.Captured();
        team.pawns.Remove(this);
    }
}
