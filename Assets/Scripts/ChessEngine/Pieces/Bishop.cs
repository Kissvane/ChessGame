using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Bishop : ChessPiece
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
        team.bishops.Remove(this);
    }

    public override void Liberated()
    {
        base.Liberated();
        team.bishops.Add(this);
    }
}
