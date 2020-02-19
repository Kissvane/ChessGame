using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Queen : ChessPiece
{
    public override void SetMovementLimit()
    {
        maxRange = 8;
        directionsAndDestination.Add(new Vector2(1f, 1f), new List<Vector2>());
        directionsAndDestination.Add(new Vector2(1f, -1f), new List<Vector2>());
        directionsAndDestination.Add(new Vector2(-1f, 1f), new List<Vector2>());
        directionsAndDestination.Add(new Vector2(-1f, -1f), new List<Vector2>());
        directionsAndDestination.Add(Vector2.up, new List<Vector2>());
        directionsAndDestination.Add(Vector2.down, new List<Vector2>());
        directionsAndDestination.Add(Vector2.right, new List<Vector2>());
        directionsAndDestination.Add(Vector2.left, new List<Vector2>());
        /*allowedMoveDirections = new List<Vector2>
        {
            Vector2.up,
            Vector2.down,
            Vector2.right,
            Vector2.left,
            new Vector2(1f,1f),
            new Vector2(1f,-1f),
            new Vector2(-1f,1f),
            new Vector2(-1f,-1f),
        };*/
    }
}
