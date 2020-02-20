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

        /*allowedMoveDirections = new List<Vector2>
        {
            Vector2.up,
            Vector2.down,
            Vector2.right,
            Vector2.left
        };*/
    }

    public override void Moved(Vector2 origin, Vector2 destination)
    {
        canCastling = false;
        base.Moved(origin, destination);
    }
}
