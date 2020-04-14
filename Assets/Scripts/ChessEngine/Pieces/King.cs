using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class King : ChessPiece
{
    private Dictionary<Vector2, bool> CastlingDirectionsAndBlockedState = new Dictionary<Vector2, bool>(); 

    public override void SetMovementLimit()
    {
        canCastling = true;
        isKing = true;
        maxRange = 1;
        moveDirectionsAndBlockedState.Add(new Vector2(1f, 1f), false);
        moveDirectionsAndBlockedState.Add(new Vector2(1f, -1f), false);
        moveDirectionsAndBlockedState.Add(new Vector2(-1f, 1f), false);
        moveDirectionsAndBlockedState.Add(new Vector2(-1f, -1f), false);
        moveDirectionsAndBlockedState.Add(Vector2.up, false);
        moveDirectionsAndBlockedState.Add(Vector2.down, false);
        moveDirectionsAndBlockedState.Add(Vector2.right, false);
        moveDirectionsAndBlockedState.Add(Vector2.left, false);
        CastlingDirectionsAndBlockedState.Add(Vector2.right, false);
        CastlingDirectionsAndBlockedState.Add(Vector2.left, false);
    }

    public override void Move(Board board, int testedLine, int testedColumn)
    {
        Vector2 originPosition = currentPosition;
        ChessboardBoxData origin = board.getBox(originPosition);
        ChessboardBoxData destination = board.getBox(testedColumn, testedLine);
        ChessPiece otherPiece = destination.piece;
        bool isCastling = false;
        
        //manage castling case
        if (otherPiece != null && otherPiece.canCastling && otherPiece.team == team)
        {
            //piece swapping
            origin.piece = otherPiece;
            otherPiece.Moved(originPosition);
            isCastling = true;
        }
        //make the normal move
        else
        {
            origin.piece = null;
            if (otherPiece != null)
            {
                otherPiece.Captured();
            }
        }

        destination.piece = this;
        Move moveToSave = new Move(originPosition, new Vector2(testedColumn, testedLine), this, otherPiece, isCastling);
        ChessEngine.instance.game.Add(moveToSave);
        //Debug.Log("MOVE SAVED " + moveToSave.movingPiece.name + " " + moveToSave.destination);
        Moved(new Vector2(testedColumn, testedLine));
    }

    public override void ForcedReverseMove(Board board, Move move)
    {
        ChessboardBoxData originBox = board.getBox(move.destination);
        ChessboardBoxData destinationBox = board.getBox(move.origin);
        if (move.capturedPiece != null)
        {
            originBox.piece = move.capturedPiece;
            move.capturedPiece.RevertValuesIfNecessary(originBox, true);
        }
        else if (move.castlingPiece != null)
        {
            originBox.piece = move.castlingPiece;
            move.castlingPiece.RevertValuesIfNecessary(originBox,false);
        }
        else
        {
            originBox.piece = null;
        }
        destinationBox.piece = this;
        RevertValuesIfNecessary(destinationBox,false);
        //Debug.Log("REVERTED");
    }

    public override void ForcedMove(Board board, Move move)
    {
        ChessboardBoxData originBox = board.getBox(move.origin);
        ChessboardBoxData destinationBox = board.getBox(move.destination);
        if (move.capturedPiece != null)
        {
            move.capturedPiece.Captured();
        }
        else if (move.castlingPiece != null)
        {
            originBox.piece = move.castlingPiece;
            move.castlingPiece.RedoValuesIfNecessary(originBox);
        }
        else
        {
            originBox.piece = null;
        }
        destinationBox.piece = this;
        RedoValuesIfNecessary(destinationBox);
    }

    public override void Moved(Vector2 destination)
    {
        canCastling = false;
        base.Moved(destination);
    }

    public override void ResetBlockedDirections()
    {
        List<Vector2> temp = new List<Vector2>();
        temp.AddRange(moveDirectionsAndBlockedState.Keys);

        foreach (Vector2 direction in temp)
        {
            moveDirectionsAndBlockedState[direction] = false;
        }

        temp = new List<Vector2>();
        temp.AddRange(CastlingDirectionsAndBlockedState.Keys);

        foreach (Vector2 direction in temp)
        {
            CastlingDirectionsAndBlockedState[direction] = false;
        }
    }

    public override void CalculateAvailableDestinations(bool isSimulation, bool verbose = false)
    {
        base.CalculateAvailableDestinations(isSimulation, verbose);
        
        //if the king can castling
        if (canCastling && !isSimulation)
        {
            int originLine = (int)currentPosition.y;
            int originColumn = (int)currentPosition.x;
            //avoiding InvalidOperationException
            List<Vector2> temp = new List<Vector2>();
            temp.AddRange(CastlingDirectionsAndBlockedState.Keys);
            //we test that there is no piece between the King and Tower 
            for (int i = 1; i <= 5; i++)
            {
                foreach (Vector2 direction in temp)
                {
                    //if this direction is not blocked
                    if (!CastlingDirectionsAndBlockedState[direction])
                    {
                        Vector2 target = new Vector2(originColumn, originLine) + direction * i;
                        bool pieceTaken = false;
                        bool forceSimulation = false;
                        //if the tested position is not the castling position
                        //this is a simulation move
                        if (target.x != 0 && target.x != 7 && !isSimulation)
                        {
                            forceSimulation = true;
                        }
                        //test if the way between the king and tower is free of piece
                        //test if the king is safe only at the true castling positions
                        if (isValidMovement(ChessEngine.instance.board, (int)target.x, (int)target.y, out pieceTaken, forceSimulation))
                        {
                            ChessPiece otherPiece = ChessEngine.instance.board.getBox(target).piece;
                            if ((target.x == 0 || target.x == 7) && otherPiece != null && otherPiece.team == team)
                            {
                                availableDestinations.Add(target);
                            }
                            if (pieceTaken)
                            {
                                //Debug.Log(otherPiece.name+" prevent castling "+target);
                                //for this calculation turn this direction is blocked
                                CastlingDirectionsAndBlockedState[direction] = true;
                            }
                        }
                        else
                        {
                            //if (pieceTaken && target.x != 0 && target.x != 7) Debug.Log(otherPiece.name + " prevent castling " + target);
                            //else Debug.Log("invalid move prevent castling "+target);
                            //for this calculation turn this direction is blocked
                            CastlingDirectionsAndBlockedState[direction] = true;
                        }
                    }
                }
            }
        }
    }

    //test if a movement is valid
    public override bool isValidMovement(Board board, int testedColumn, int testedLine, out bool pieceTaken, bool isSimulation)
    {
        pieceTaken = false;
        // the destination is out of the board
        if (testedLine < 0 || testedLine > 7 || testedColumn < 0 || testedColumn > 7) return false;

        ChessboardBoxData testedBox = board.boxesDatas[testedColumn][testedLine];
        ChessPiece takenPiece = testedBox.piece;
        float moveRange = Vector2.Distance(currentPosition, new Vector2(testedColumn,testedLine));

        if (takenPiece != null)
        {
            pieceTaken = true;
            //Allow castling
            if (moveRange >= 3)
            {
                if (takenPiece.team != team) return false;
                else
                {
                    if(!canCastling || !takenPiece.canCastling) return false;
                }
            }
            //manage capture
            else
            {
                if (takenPiece.team == team) return false;
            }
        }

        return FinalValidation(board, testedColumn, testedLine, isSimulation);
    }

    public override void RevertValuesIfNecessary(ChessboardBoxData box, bool liberated)
    {
        base.RevertValuesIfNecessary(box,liberated);
        canCastling = !hasMoved;
    }

    public override void RedoValuesIfNecessary(ChessboardBoxData box)
    {
        base.RedoValuesIfNecessary(box);
        canCastling = !hasMoved;
    }
}
