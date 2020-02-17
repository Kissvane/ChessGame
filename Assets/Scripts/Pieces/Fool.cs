using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fool : ChessPiece
{
    public override void SetMovementLimit()
    {
        maxRange = 8;
        allowedMoveDirections = new List<Vector2>
        {
            new Vector2(1f,1f),
            new Vector2(1f,-1f),
            new Vector2(-1f,1f),
            new Vector2(-1f,-1f)
        };
    }
}
