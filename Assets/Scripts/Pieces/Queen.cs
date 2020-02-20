using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Queen : ChessPiece
{
    public override void SetMovementLimit()
    {
        maxRange = 7;
        moveDirectionsAndBlockedState.Add(new Vector2(1f, 1f), false);
        moveDirectionsAndBlockedState.Add(new Vector2(1f, -1f), false);
        moveDirectionsAndBlockedState.Add(new Vector2(-1f, 1f), false);
        moveDirectionsAndBlockedState.Add(new Vector2(-1f, -1f), false);
        moveDirectionsAndBlockedState.Add(Vector2.up, false);
        moveDirectionsAndBlockedState.Add(Vector2.down, false);
        moveDirectionsAndBlockedState.Add(Vector2.right, false);
        moveDirectionsAndBlockedState.Add(Vector2.left, false);
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
