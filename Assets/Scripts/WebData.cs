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
    public class Game
    {
        public int id;
        public string gamename;
        public int turnnum;
    }

    [System.Serializable]
    public class Player
    {
        public int id;
        public string gamename;
        public int playernumber;
        public string username;
        public int commanddata;
        public int viewdata;
    }

    [System.Serializable]
    public class Players
    {
        public Player[] players;
    }
}
