using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Testing : MonoBehaviour
{
    public Vector2 position;
    public List<Move> result;
    public List<Vector2> destinations;
    [ContextMenu("TESTING")]
    // Start is called before the first frame update
    void TEST()
    {
        ChessPiece piece = ChessEngine.instance.GetPieceAtPosition(position);
        Debug.Log(piece.name);
        result.Clear();
        result.AddRange(ChessEngine.instance.game);
    }


    [ContextMenu("moveTESTING")]
    void moveTEST()
    {
        ChessPiece piece = ChessEngine.instance.GetPieceAtPosition(position);
        piece.CalculateAvailableDestinations(false, true);
        destinations = new List<Vector2>();
        destinations.AddRange(piece.availableDestinations);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
