using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Vgo
{
    public class WebPlayer : MonoBehaviour
    {

        public Game game;
        public Player player;

        public Player[] myPlayers;

        WebClient webClient;
        void Start()
        {
            webClient = GetComponentInParent<WebClient>();
        }

        private IEnumerator SubmitOrdersQueue()
        {

            string commanddatajson = JsonUtility.ToJson(player.ordersQueue);
            Debug.Log(commanddatajson);
            UnityWebRequest www = UnityWebRequest.Post(
                webClient.GetUrl("/player/" + player.id + "/submit"),
                new List<IMultipartFormSection> {
                new MultipartFormDataSection("commanddata", commanddatajson),
                }
            );

            yield return webClient.SendWebRequest(www);

            string json = www.downloadHandler.text;
            Debug.Log(json);
            // JsonUtility.FromJsonOverwrite(json, user);

            foreach (var s in www.GetResponseHeaders())
            {
                Debug.Log("s=" + s);
            }
        }

        private IEnumerator GetMyPlayers()
        {

            UnityWebRequest www = UnityWebRequest.Get(webClient.GetUrl("/myplayers"));

            yield return webClient.SendWebRequest(www);

            string json = www.downloadHandler.text;
            myPlayers = JsonHelper.FromJson<Player>(json);

        }

          private IEnumerator GetCommandData()
        {

            UnityWebRequest www = UnityWebRequest.Get(webClient.GetUrl(player.id+"/commanddata"));

            yield return webClient.SendWebRequest(www);

            string json = www.downloadHandler.text;
            JsonUtility.FromJsonOverwrite(json, player);
            player.PostJsonDeserialize();            

        }      

        void OnGUI()
        {
            //using the immediate GUI for ease of examples
            int y = 0;
            int h = 25;
            int w = 200;

            if (webClient.isWaiting)
            {
                GUI.Label(new Rect(10, y += h, w, h), "Waiting for Web Response...");
                return;
            }

            if (player.id == 0)
            {
                if (myPlayers == null || myPlayers.Length == 0)
                {
                    if (GUI.Button(new Rect(10, y += h, w, h), "GetMyPlayers"))
                    {
                        StartCoroutine(GetMyPlayers());
                    }
                    return;
                }

                for (int i = 0; i < myPlayers.Length; i++)
                {
                    if (GUI.Button(new Rect(10, y += h, w, h), myPlayers[i].gamename + " " + myPlayers[i].playernumber))
                    {
                        player = myPlayers[i];
                        StartCoroutine(GetCommandData());
                    }
                }
                return;
            }

            bool allChosen = true;
            for (int i = 0; i < 3; i++)
            {
                if (player.ordersQueue.choices[i] != EChoice.None)
                {
                    GUI.Label(new Rect(10, y += h, w, h), player.ordersQueue.choices[i].ToString());
                    continue;
                }
                allChosen = false;
                foreach (EChoice suit in (EChoice[])System.Enum.GetValues(typeof(EChoice)))
                {
                    if (GUI.Button(new Rect(10, y += h, w, h), suit.ToString()))
                    {
                        player.ordersQueue.choices[i] = suit;
                    }
                }
            }

            if (allChosen)
            {
                if (GUI.Button(new Rect(10, y += h, w, h), "Submit Turn"))
                {
                    StartCoroutine(SubmitOrdersQueue());
                }
            }
        }

    }
}
