﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Queen : ChessPiece
{
    public override void SetMovementLimit()
    {
        moveDirectionsAndBlockedState = new Dictionary<Vector2, bool>();
        maxRange = 7;
        moveDirectionsAndBlockedState.Add(new Vector2(1f, 1f), false);
        moveDirectionsAndBlockedState.Add(new Vector2(1f, -1f), false);
        moveDirectionsAndBlockedState.Add(new Vector2(-1f, 1f), false);
        moveDirectionsAndBlockedState.Add(new Vector2(-1f, -1f), false);
        moveDirectionsAndBlockedState.Add(Vector2.up, false);
        moveDirectionsAndBlockedState.Add(Vector2.down, false);
        moveDirectionsAndBlockedState.Add(Vector2.right, false);
        moveDirectionsAndBlockedState.Add(Vector2.left, false);
    }

    public override void Captured()
    {
        base.Captured();
        team.queens.Remove(this);
    }

    public override void Liberated()
    {
        base.Liberated();
        team.queens.Add(this);
    }

}
