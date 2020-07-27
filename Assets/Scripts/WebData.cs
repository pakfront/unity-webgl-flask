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
        public string turndata;

        public void PostJsonDeserialize()
        {
            // Debug.LogError("Not implemented");
            //JsonUtility.FromJsonOverwrite(turnnum, ordersQueue);
        }

        public void RunSimulation()
        {
            Debug.LogError("Not implemented");
        }
    }

    [System.Serializable]
    public class Player
    {
        public int id;
        public string gamename;
        public int playernumber;
        public string username;
        public string commanddata;
        public OrdersQueue ordersQueue;
        public int viewdata;

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


}
