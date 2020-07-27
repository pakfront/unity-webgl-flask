using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Vgo
{
    public class WebClient : MonoBehaviour
    {

        public User user;
        public Player[] myPlayers;

        [SerializeField] private string urlbase = "http://127.0.0.1:5000";
        [SerializeField] private string args = "?type=json";

        internal bool isWaiting;

        WebPlayer webPlayer;
        WebReferee webReferee;

        void Start()
        {
            webPlayer = GetComponentInChildren<WebPlayer>();
            webReferee = GetComponentInChildren<WebReferee>();
            webPlayer.enabled = false;
            webReferee.enabled = false;
            Debug.Log("WebClient url: " + Application.absoluteURL);
            Debug.Log("Application.platform:" + Application.platform);
        }

        public string GetUrl(string route)
        {
            string url = urlbase + route + args; 
            Debug.Log(url);
            return url;
        }

        public IEnumerator SendWebRequest(UnityWebRequest www)
        {
            isWaiting = true;
            yield return www.SendWebRequest();
            isWaiting = false;
            if (www.isNetworkError || www.isHttpError)
            {
                Debug.LogError(www.url+" : "+www.error);
                //TODO show error in ui
                yield break;
            }
            Debug.Log(www.downloadHandler.text);
        }

        private IEnumerator Login()
        {
            UnityWebRequest www = UnityWebRequest.Post(
                GetUrl("/auth/login"),
                new List<IMultipartFormSection> {
                new MultipartFormDataSection("username", user.username),
                new MultipartFormDataSection("password", user.password),
                }
            );

            yield return this.SendWebRequest(www);

            string json = www.downloadHandler.text;
            Debug.Log(json);
            JsonUtility.FromJsonOverwrite(json, user);

            foreach (var s in www.GetResponseHeaders())
            {
                Debug.Log("s=" + s);
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

            if (GUI.Button(new Rect(10, y += h, w, h), "Player"))
            {
                webPlayer.enabled = true;
                this.enabled = false;
            }

            if (GUI.Button(new Rect(10, y += h, w, h), "Referee"))
            {
                webReferee.enabled = true;
                this.enabled = false;
            }
        }


    }
}
