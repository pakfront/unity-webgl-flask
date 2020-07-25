using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Vgo
{
    public class WebClient : MonoBehaviour
    {
        public User user;
        public Game game;
        public Player player;

        public Player[] myPlayers;

        public string urlbase = "http://127.0.0.1:5000";
        public string args = "?type=json";

        public string sessionCookie;
        void Start()
        {
            Debug.Log("WebClient url: " + Application.absoluteURL);
            Debug.Log("Application.platform:" + Application.platform);


            // #if UNITY_WEBGL
            //             //deduce the webstie name. there more robust ways to do this, it should be done in the index.html wrapper
            //             if (Application.platform == RuntimePlatform.WebGLPlayer)
            //             {
            //                 string[] parts = Application.absoluteURL.Split('/');
            //                 urlbase = parts[0] + "://" + parts[2];
            //                 StartCoroutine(GetUserInfo());
            //             }
            //             else
            //             {
            //                 string[] parts = urlbase.Split('/');
            //                 urlbase = parts[0] + "://" + parts[2];
            //             }
            // #endif

        }


        private IEnumerator Login()
        {

            UnityWebRequest www = UnityWebRequest.Post(
                urlbase + "/auth/login" + args,
                new List<IMultipartFormSection> {
                new MultipartFormDataSection("username", user.username),
                new MultipartFormDataSection("password", user.password),
                }
            );
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(urlbase + "/auth/login" + args + ":" + www.error);
                yield break;

            }

            string json = www.downloadHandler.text;
            Debug.Log(json);
            JsonUtility.FromJsonOverwrite(json, user);

            foreach (var s in www.GetResponseHeaders())
            {
                Debug.Log("s=" + s);
            }
        }

        private IEnumerator GetMyPlayers()
        {

            UnityWebRequest www = UnityWebRequest.Get(urlbase + "/myplayers");
            Debug.Log(www.url);


            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.LogError(www.error);
            }
            else
            {
                // Show results as text
                string json = www.downloadHandler.text;
                Debug.Log(json);
                myPlayers = JsonHelper.FromJson<Player>(json);
                // Debug.Log(JsonUtility.FromJson<Players>(json));
            }
        }

        IEnumerator GetUserInfo()
        {
            UnityWebRequest www = UnityWebRequest.Get(urlbase + "/auth/myprofile?type=json");
            Debug.Log(www.url);

            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.LogError(www.error);
            }
            else
            {
                // Show results as text
                string json = www.downloadHandler.text;
                Debug.Log(json);
                // User [] users = JsonHelper.FromJson<User>(json);

                // user = users[0];
                JsonUtility.FromJsonOverwrite(json, user);
                // // Or retrieve results as binary data
                // byte[] results = www.downloadHandler.data;
            }
        }

        void OnGUI()
        {
            //using the immediate GUI for ease of examples
            int y = 0;
            int h = 25;
            int w = 200;

            GUI.Label(new Rect(10, y += h, w, h), "urlbase:" + urlbase);
            GUI.Label(new Rect(10, y += h, w, h), "absUrl:" + Application.absoluteURL);
            GUI.Label(new Rect(10, y += h, w, h), "id:" + user.id + " " + user.username);

            w = 100;
            if (user.id == 0)
            {
                user.username = GUI.TextField(new Rect(10, y += h, w, h), user.username);
                user.password = GUI.TextField(new Rect(10, y += h, w, h), user.password);

                if (GUI.Button(new Rect(10, y += h, w, h), "Login"))
                {
                    StartCoroutine(Login());
                }
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
                    if (GUI.Button(new Rect(10, y += h, w, h), myPlayers[i].gamename+" "+myPlayers[i].playernumber))
                    {
                        player = myPlayers[i];
                    }
                }
                return;
            }

        }

    }
}
