using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Board board;
    public TeamManager whiteTeam;
    public TeamManager blackTeam;
    public bool whiteIsPlaying = true;

    public void Start()
    {
        board.ConstructBoard();
        whiteTeam.positionTeam(board);
        blackTeam.positionTeam(board);
        MyEventSystem.instance.Set("whiteIsPlaying", whiteIsPlaying);

        
    }

    

    public void NextTurn()
    {
        whiteIsPlaying = !whiteIsPlaying;
        MyEventSystem.instance.Set("originMove", null);
        MyEventSystem.instance.Set("destinationMove",null);
        MyEventSystem.instance.Set("whiteIsPlaying", whiteIsPlaying);
    }

}
