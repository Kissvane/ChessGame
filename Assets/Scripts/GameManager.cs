using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public BoardConstructor constructor;
    public Board board;
    public TeamManager whiteTeam;
    public TeamManager blackTeam;

    public List<Color> colors;

    public bool whiteIsPlaying = true;

    public void Start()
    {
        whiteTeam = new TeamManager("white", constructor.whiteColor, constructor.blackColor);
        blackTeam = new TeamManager("black", constructor.blackColor, constructor.whiteColor);
        constructor.ConstructBoard();
        board = MyEventSystem.instance.Get("Board");
        whiteTeam.positionTeam(constructor);
        blackTeam.positionTeam(constructor);
        MyEventSystem.instance.Set("whiteIsPlaying", whiteIsPlaying);
    }

    public void NextTurn()
    {
        whiteIsPlaying = !whiteIsPlaying;
        MyEventSystem.instance.Set("originMove", null);
        MyEventSystem.instance.Set("destinationMove",null);
        MyEventSystem.instance.Set("whiteIsPlaying", whiteIsPlaying);
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
