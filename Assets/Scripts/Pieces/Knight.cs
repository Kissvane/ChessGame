using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knight : ChessPiece
{
    public override void SetMovementLimit()
    {
        
    }

    public override void CalculatePossibleDestinations()
    {
        directionsAndDestination.Add(new Vector2(1f, 2f), null);
        directionsAndDestination.Add(new Vector2(2f, 1f), null);
        directionsAndDestination.Add(new Vector2(-1f, 2f), null);
        directionsAndDestination.Add(new Vector2(-2f, 1f), null);
        directionsAndDestination.Add(new Vector2(1f, -2f), null);
        directionsAndDestination.Add(new Vector2(2f, -1f), null);
        directionsAndDestination.Add(new Vector2(-1f, -2f), null);
        directionsAndDestination.Add(new Vector2(-2f, -1f), null);

        /*possibleDestinations.Add(new Vector2(1f, 2f));
        possibleDestinations.Add(new Vector2(2f, 1f));
        possibleDestinations.Add(new Vector2(-1f, 2f));
        possibleDestinations.Add(new Vector2(-2f, 1f));
        possibleDestinations.Add(new Vector2(1f, -2f));
        possibleDestinations.Add(new Vector2(2f, -1f));
        possibleDestinations.Add(new Vector2(-1f, -2f));
        possibleDestinations.Add(new Vector2(-2f, -1f));*/
    }
}
