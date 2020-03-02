using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameManager : MonoBehaviour
{
    public BoardConstructor constructor;
    public TeamManager whiteTeam;
    public TeamManager blackTeam;

    public List<Color> colors;

    public bool whiteIsPlaying = true;

    public Vector2 originMove;
    public Vector2 destinationMove;

    public void Start()
    {
        //whiteTeam = new TeamManager(ChessColor.White, constructor.whiteColor, constructor.blackColor);
        //blackTeam = new TeamManager(ChessColor.Black, constructor.blackColor, constructor.whiteColor);
        //constructor.ConstructBoard();
        //whiteTeam.positionTeam(constructor);
        //blackTeam.positionTeam(constructor);
    }

    public void NextTurn()
    {
        whiteIsPlaying = !whiteIsPlaying;

        bool hasValidMove = false;
        TeamManager playingTeam = whiteIsPlaying ? whiteTeam : blackTeam;
        foreach (ChessPiece piece in playingTeam.piecesObjects.Keys)
        {
            piece.CalculateAvailableDestinations();
            if (piece.availableDestinations.Count > 0)
            {
                hasValidMove = true;
            }
        }

        if (!hasValidMove)
        {
            ShowWinner(playingTeam.other);
        }
    }

    public void ShowWinner(TeamManager winningTeam)
    {
        Debug.Log(winningTeam.teamEnum+" win !");
    }

}
