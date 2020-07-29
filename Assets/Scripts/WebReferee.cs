using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Vgo
{
    public class WebReferee : MonoBehaviour
    {

        public User user;
        public Game game;
        public Player[] players;
        public Game[] myGames;
        WebClient webClient;
        void Start()
        {
            webClient = GetComponentInParent<WebClient>();
        }

        private IEnumerator GetMyGames()
        {

            UnityWebRequest www = UnityWebRequest.Get(webClient.GetUrl("/mygames"));

            yield return webClient.SendWebRequest(www);
            if (www.isNetworkError || www.isHttpError) yield break;

            // Show results as text
            string json = www.downloadHandler.text;
            myGames = JsonHelper.FromJson<Game>(json);
            // Debug.Log(JsonUtility.FromJson<Players>(json));
        }
        private IEnumerator GetGameData()
        {

            {
                UnityWebRequest www = UnityWebRequest.Get(webClient.GetUrl("/referee/game/" + game.id + "/turndata"));

                yield return webClient.SendWebRequest(www);
                if (www.isNetworkError || www.isHttpError) yield break;

                string json = www.downloadHandler.text;
                JsonUtility.FromJsonOverwrite(json, game);
            }

            {
                UnityWebRequest www = UnityWebRequest.Get(webClient.GetUrl("/referee/game/" + game.id + "/commanddata"));

                yield return webClient.SendWebRequest(www);
                if (www.isNetworkError || www.isHttpError) yield break;

                string json = www.downloadHandler.text;
                players = JsonHelper.FromJson<Player>(json);
                for (int i = 0; i < players.Length; i++)
                {
                    players[i].PostJsonDeserialize();
                }
            }
        }

        private IEnumerator PostGameData()
        {

            {
                UnityWebRequest www = UnityWebRequest.Post(
                    webClient.GetUrl("/referee/game/" + game.id + "/turndata"),
                    new List<IMultipartFormSection> {
                new MultipartFormDataSection("turndata", game.turndata),
                new MultipartFormDataSection("turnnum", game.turnnum.ToString()),
                    }
                );
                yield return webClient.SendWebRequest(www);
                if (www.isNetworkError || www.isHttpError) yield break;
            }

            foreach (Player player in players)
            {
                UnityWebRequest www = UnityWebRequest.Post(
                    webClient.GetUrl("/referee/player/" + player.id + "/viewdata"),
                    new List<IMultipartFormSection> {
                new MultipartFormDataSection("viewdata", player.viewdata),
                    }
                );
                yield return webClient.SendWebRequest(www);
                if (www.isNetworkError || www.isHttpError) yield break;
            }
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


            if (game.id == 0)
            {
                if (myGames == null || myGames.Length == 0)
                {
                    if (GUI.Button(new Rect(10, y += h, w, h), "GetMyGames"))
                    {
                        StartCoroutine(GetMyGames());
                    }
                    return;
                }

                for (int i = 0; i < myGames.Length; i++)
                {
                    if (GUI.Button(new Rect(10, y += h, w, h), myGames[i].gamename + " " + myGames[i].turnnum))
                    {
                        game = myGames[i];
                        StartCoroutine(GetGameData());
                    }
                }
                return;
            }

            if (GUI.Button(new Rect(10, y += h, w, h), "Simulate " + game.gamename))
            {
                game.RunSimulation(players);
                StartCoroutine(PostGameData());

            }
        }

    }
}
