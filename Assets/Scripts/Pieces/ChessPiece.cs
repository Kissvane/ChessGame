using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ChessPiece : MonoBehaviour
{
    protected List<Vector2> allowedMoveDirections;
    protected int maxRange = 0;
    public List<Vector2> possibleDestinations;
    public TeamManager team;
    public bool hasMoved = false;
    public Renderer pawnRenderer;
    public SpriteRenderer iconRenderer;
    public bool canCastling = false;

    public virtual void CalculatePossibleDestinations()
    {
        SetMovementLimit();

        for (int i = 1; i < maxRange; i++)
        {
            foreach (Vector2 direction in allowedMoveDirections)
            {
                possibleDestinations.Add(direction * i);
            }
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
        CalculatePossibleDestinations();
    }

}
