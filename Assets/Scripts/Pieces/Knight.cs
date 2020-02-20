using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knight : ChessPiece
{
    public override void SetMovementLimit()
    {
        maxRange = 1;
        moveDirectionsAndBlockedState.Add(new Vector2(1f, 2f), false);
        moveDirectionsAndBlockedState.Add(new Vector2(2f, 1f), false);
        moveDirectionsAndBlockedState.Add(new Vector2(-1f, 2f), false);
        moveDirectionsAndBlockedState.Add(new Vector2(-2f, 1f), false);
        moveDirectionsAndBlockedState.Add(new Vector2(1f, -2f), false);
        moveDirectionsAndBlockedState.Add(new Vector2(2f, -1f), false);
        moveDirectionsAndBlockedState.Add(new Vector2(-1f, -2f), false);
        moveDirectionsAndBlockedState.Add(new Vector2(-2f, -1f), false);
    }
}
