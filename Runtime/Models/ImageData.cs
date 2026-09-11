using UnityEngine;

namespace Down2Jam4Unity.Models
{
    public class ImageData 
    {
        public class Request
        {
            public byte[] upload;
        }

        public class Response
        {
            public bool success;
            public string data;
            public string message;
        }
    }
}
