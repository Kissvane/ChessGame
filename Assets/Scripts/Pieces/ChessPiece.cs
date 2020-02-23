using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ChessPiece
{
    public Dictionary<Vector2,bool> moveDirectionsAndBlockedState = new Dictionary<Vector2, bool>();
    //protected List<Vector2> allowedMoveDirections;
    public int maxRange = 0;
    public List<Vector2> availableDestinations;
    public TeamManager team;
    public bool hasMoved = false;
    public Renderer pawnRenderer;
    public SpriteRenderer iconRenderer;
    public bool canCastling = false;
    public bool isKing = false;
    public bool isPawn = false;
    //when i use vector2 to represent positions on the board x is the column and y the line
    public Vector2 currentPosition;

    public virtual void CalculateAvailableDestinations(bool isSimulation = false)
    {
        ResetBlockedDirections();
        availableDestinations.Clear();
        Board board = MyEventSystem.instance.Get("Board");
        int originLine = (int)currentPosition.y;
        int originColumn = (int)currentPosition.x;
        for (int i = 1; i <= maxRange; i++)
        {
            foreach (Vector2 direction in moveDirectionsAndBlockedState.Keys)
            {
                //if this direction is not blocked
                if (!moveDirectionsAndBlockedState[direction])
                {
                    if (board.isValidMovement(originLine,originColumn,(int)direction.y*i,(int)direction.x*i,isSimulation))
                    {
                        availableDestinations.Add(new Vector2(originColumn, originLine)+direction*i);
                    }
                    else
                    {
                        //for this calculation turn this direction is blocked
                        moveDirectionsAndBlockedState[direction] = true;
                    }
                }
            }
        }
    }

    public void ResetBlockedDirections()
    {
        foreach (Vector2 direction in moveDirectionsAndBlockedState.Keys)
        {
            moveDirectionsAndBlockedState[direction] = false;
        }
    }

    public virtual void Moved(Vector2 origin, Vector2 destination)
    {
        hasMoved = true;
        currentPosition = destination;
    }

    public virtual void Captured()
    {
        GameObject captured = team.piecesObjects[this];
        team.other.Capture(this, captured);
        MyEventSystem.instance.FireEvent("Captured",new GenericDictionary().Set("pieceObject",captured));
        team.piecesObjects.Remove(this);
    }

    public abstract void SetMovementLimit();
}
