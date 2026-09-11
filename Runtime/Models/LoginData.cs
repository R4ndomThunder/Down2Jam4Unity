using System;
using UnityEngine;

namespace Down2Jam4Unity.Models
{
    [Serializable]
    public class LoginData
    {
        [Serializable]
        public class Request
        {
            public string username;
            public string password;
        }

        [Serializable]
        public class Response
        {
            public bool success;
            public ResponseData data;
        }

        [Serializable]
        public class ResponseData
        {
            public UserData user;
            public string token;
        }

        [Serializable]
        public class UserData
        {
            public string slug;
            public int id;
            public string password;
        }
    }
}
