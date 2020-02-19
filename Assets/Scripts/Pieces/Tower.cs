using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : ChessPiece
{
    public override void SetMovementLimit()
    {
        canCastling = true;
        maxRange = 8;

        directionsAndDestination.Add(Vector2.up, new List<Vector2>());
        directionsAndDestination.Add(Vector2.down, new List<Vector2>());
        directionsAndDestination.Add(Vector2.right, new List<Vector2>());
        directionsAndDestination.Add(Vector2.left, new List<Vector2>());

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
