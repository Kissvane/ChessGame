using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : ChessPiece
{
    public override void SetMovementLimit()
    {
        canCastling = true;
        maxRange = 7;

        moveDirectionsAndBlockedState.Add(Vector2.up, false);
        moveDirectionsAndBlockedState.Add(Vector2.down, false);
        moveDirectionsAndBlockedState.Add(Vector2.right, false);
        moveDirectionsAndBlockedState.Add(Vector2.left, false);
    }

    public override void Moved(Vector2 origin, Vector2 destination)
    {
        canCastling = false;
        base.Moved(origin, destination);
    }

    public override void Captured()
    {
        base.Captured();
        team.towers.Remove(this);
    }
}
