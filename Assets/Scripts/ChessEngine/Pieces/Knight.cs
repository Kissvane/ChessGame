﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
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

    public override void Captured()
    {
        base.Captured();
        team.knights.Remove(this);
    }

    public override void Liberated()
    {
        base.Liberated();
        team.knights.Add(this);
    }

}
