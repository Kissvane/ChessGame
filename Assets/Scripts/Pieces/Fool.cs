using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fool : ChessPiece
{
    public override void SetMovementLimit()
    {
        maxRange = 7;
        moveDirectionsAndBlockedState.Add(new Vector2(1f, 1f),false);
        moveDirectionsAndBlockedState.Add(new Vector2(1f, -1f), false);
        moveDirectionsAndBlockedState.Add(new Vector2(-1f, 1f), false);
        moveDirectionsAndBlockedState.Add(new Vector2(-1f, -1f), false);
        /*allowedMoveDirections = new List<Vector2>
        {
            new Vector2(1f,1f),
            new Vector2(1f,-1f),
            new Vector2(-1f,1f),
            new Vector2(-1f,-1f)
        };*/
    }
}
