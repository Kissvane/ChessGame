﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class King : ChessPiece
{
    public override void SetMovementLimit()
    {
        canCastling = true;
        maxRange = 2;
        allowedMoveDirections = new List<Vector2>
        {
            Vector2.up,
            Vector2.down,
            Vector2.right,
            Vector2.left,
            new Vector2(1f,1f),
            new Vector2(1f,-1f),
            new Vector2(-1f,1f),
            new Vector2(-1f,-1f),
        };
    }

    public override void Moved(Vector2 origin, Vector2 destination)
    {
        canCastling = false;
        base.Moved(origin,destination);
    }
}
