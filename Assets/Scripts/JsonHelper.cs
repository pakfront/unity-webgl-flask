using System;

// namespace Vgo
// {
    //  based on https://www.boxheadproductions.com.au/deserializing-top-level-arrays-in-json-with-unity/
    public static class JsonHelper
    {

        public static T[] FromJson<T>(string json)
        {
            Wrapper<T> wrapper = UnityEngine.JsonUtility.FromJson<Wrapper<T>>(json);
            return wrapper.Items;
        }

        public static T[] FromUnwrappedJson<T>(string json)
        {
            Wrapper<T> wrapper = UnityEngine.JsonUtility.FromJson<Wrapper<T>>(
              @"{""Items"":"+json+@"]"
                );
            return wrapper.Items;
        }

        public static string ToJson<T>(T[] array)
        {
            Wrapper<T> wrapper = new Wrapper<T>();
            wrapper.Items = array;
            return UnityEngine.JsonUtility.ToJson(wrapper);
        }

        [Serializable]
        private class Wrapper<T>
        {
            public T[] Items;
        }
    }
// }
