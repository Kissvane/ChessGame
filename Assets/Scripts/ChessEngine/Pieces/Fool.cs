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
    }

    public override void Captured()
    {
        base.Captured();
        team.fools.Remove(this);
    }
}
