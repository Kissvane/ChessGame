using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class King : ChessPiece
{
    public override void SetMovementLimit()
    {
        canCastling = true;
        isKing = true;
        maxRange = 1;
        moveDirectionsAndBlockedState.Add(new Vector2(1f, 1f), false);
        moveDirectionsAndBlockedState.Add(new Vector2(1f, -1f), false);
        moveDirectionsAndBlockedState.Add(new Vector2(-1f, 1f), false);
        moveDirectionsAndBlockedState.Add(new Vector2(-1f, -1f), false);
        moveDirectionsAndBlockedState.Add(Vector2.up, false);
        moveDirectionsAndBlockedState.Add(Vector2.down, false);
        moveDirectionsAndBlockedState.Add(Vector2.right, false);
        moveDirectionsAndBlockedState.Add(Vector2.left, false);
    }

    public override void Moved(Vector2 origin, Vector2 destination)
    {
        canCastling = false;
        base.Moved(origin,destination);
    }
}
