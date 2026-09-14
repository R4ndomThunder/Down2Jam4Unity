using System;

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
        public class TokenRequest
        {
            public string clientName;
            public string gameSlug;
        }

        [Serializable]
        public class TokenResponse
        {
            public bool success;
            public TokenResponseData data;
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
        public class TokenResponseData
        {
            public string deviceCode;
            public string userCode;
            public string verificationUri;
            public int expiresId;
            public int interval;
        }

        [Serializable]
        public class UserData
        {
            public string slug;
            public int id;
            public string password;
        }

        [Serializable]
        public class TokenPollRequest
        {
            public string deviceCode;

        }

        [Serializable]
        public class TokenPollResponse
        {
            public bool success;
            public TokenPollResponseData data;
        }

        [Serializable]
        public class TokenPollResponseData
        {
            public string status;
            public string token;
            public UserDataTokenResponse user;
        }


        [Serializable]
        public class UserDataTokenResponse
        {
            public int id;
            public string slug;
            public string name;
            public string profilePicture;
        }

        [Serializable]
        public class TokenRevokeRequest
        {
            public string id;
        }

        [Serializable]
        public class TokenRevokeResponse
        {
            public bool success;
            public string message;
        }

        [Serializable]
        public class TokenListResponse
        {
            public bool success;
            public TokenListData[] data;
        }


        [Serializable]
        public class TokenListData
        {
            public string id;
            public string name;
            public string keyPrefix;
            public string createAt;
            public string lastUsedAt;
        }
    }
}
