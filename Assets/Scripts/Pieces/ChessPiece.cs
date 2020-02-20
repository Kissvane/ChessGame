using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ChessPiece : MonoBehaviour
{
    public Dictionary<Vector2,bool> moveDirectionsAndBlockedState = new Dictionary<Vector2, bool>();
    //protected List<Vector2> allowedMoveDirections;
    public int maxRange = 0;
    public List<Vector2> availableMoves;
    public TeamManager team;
    public bool hasMoved = false;
    public Renderer pawnRenderer;
    public SpriteRenderer iconRenderer;
    public bool canCastling = false;

    public virtual void CalculateAvailableMoves(GenericDictionary args)
    {
        ResetBlockedDirections();
        availableMoves.Clear();
        for (int i = 1; i <= maxRange; i++)
        {
            foreach (Vector2 direction in moveDirectionsAndBlockedState.Keys)
            {
                //if this direction is not blocked
                if (!moveDirectionsAndBlockedState[direction])
                {
                    GenericDictionary data = new GenericDictionary();
                    data.Set("originLine", args.Get("originLine"));
                    data.Set("originColumn", args.Get("originColumn"));
                    data.Set("destinationLine", direction.y * i);
                    data.Set("destinationColumn", direction.x * i);
                    data.Set("movedPiece", this);
                    if (MyEventSystem.instance.Get("isValidMovement", data))
                    {
                        availableMoves.Add(direction*i);
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
    }

    public void Captured()
    {
        gameObject.SetActive(false);
    }

    public abstract void SetMovementLimit();

    // Start is called before the first frame update
    void Start()
    {
    }

}
