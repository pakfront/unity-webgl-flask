using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Vgo
{
    [System.Serializable]
    public class User
    {
        public int id;
        public string username = "tim";
        public string password = "test";
    }


    [System.Serializable]
    public class Player
    {
        public int id;
        public string gamename;
        public int playernumber;
        public string username;
        public string commanddata;
        public string viewdata;
        public OrdersQueue ordersQueue;

        public void PostJsonDeserialize()
        {
            JsonUtility.FromJsonOverwrite(commanddata, ordersQueue);
        }

    }

    [System.Serializable]
    public class OrdersQueue
    {
        public EChoice[] choices = new EChoice[3];

    }




    public enum EChoice { None, Forward, Left, Right }

    [System.Serializable]
    public class Game
    {
        public int id;
        public string gamename;
        public int turnnum;
        public string turndata;
        public Board board;

        public void PostJsonDeserialize()
        {
            Debug.LogError("Not implemented");
            JsonUtility.FromJsonOverwrite(turndata, board);
        }

        public void RunSimulation(Player [] players)
        {
            if (board == null) 
            {
                Debug.Log("Creating Board");
                board = new Board();
            }
            
            if ( board.pawns == null || board.pawns.Length == 0)
            {
                Debug.Log("Initing Board");
                board.Init(players);
            }

            for(int i = 0; i < players.Length; i++)
            {
                Player player = players[i];
                Pawn pawn = board.pawns[player.playernumber];
                for (int j = 0; j < player.ordersQueue.choices.Length; j++)
                {
                    switch (players[i].ordersQueue.choices[j])
                    {
                        case EChoice.Forward:
                            pawn.y++;
                            break;
                        case EChoice.Left:
                            pawn.x--;
                            break;
                        case EChoice.Right:
                            pawn.x++;
                            break;
                            
                        default:
                            Debug.LogError("illegal value for "+player.username);
                            break;
                    }
                }
            }
            turndata = JsonUtility.ToJson(board);
            turnnum++;

            for(int i = 0; i < players.Length; i++)
            {
                Player player = players[i];
                player.commanddata = null;
                player.viewdata = JsonUtility.ToJson(board);
            }
        }
    }

    [System.Serializable]
    public class Board
    {
        public Pawn [] pawns;

        public void Init(Player[] players)
        {
            pawns = new Pawn[players.Length];
            for (int i = 0; i < players.Length; i++)
            {
                pawns[i] = new Pawn();
                pawns[i].playernumber = players[i].playernumber;
                pawns[i].x = i;
                pawns[i].y = 0;
            }
        }
    }

    [System.Serializable]
    public class Pawn
    {
        public int playernumber;
        public int x, y;
    }

}
