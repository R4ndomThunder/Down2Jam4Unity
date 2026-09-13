using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using Random = UnityEngine.Random;

namespace Down2Jam4Unity.Utility
{
    public static class CRUDUtility
    {
        /// <summary>
        /// Execute an HTTP POST Request
        /// </summary>
        /// <typeparam name="TResultType">Type to return as response</typeparam>
        /// <param name="url">API endpoint</param>
        /// <param name="body">Request body</param>
        /// <param name="contentType">Content Type of the body</param>
        /// <param name="customHeaders">Request additional headers</param>
        /// <returns></returns>
        public static async Task<TResultType> Post<TResultType>(string url, string body, string contentType = "application/json", List<RequestHeader> customHeaders = null)
        {
            Debug.Log($"[Post] ~ url: {url} \nbody: {body}");
            using var www = UnityWebRequest.Post($"{url}", body, contentType);

            www.SetRequestHeader("Content-Type", contentType);

            if (customHeaders != null)
                foreach (var header in customHeaders)
                {
                    www.SetRequestHeader(header.name, header.value);
                }

            var operation = www.SendWebRequest();

            while (!operation.isDone)
                await Task.Yield();

            var jsonResponse = www.downloadHandler.text;
            if (www.result != UnityWebRequest.Result.Success)
                Debug.LogError($"Failed: {www.error}");

            try
            {
                if (string.IsNullOrEmpty(jsonResponse)) return default;

                var result = JsonUtility.FromJson<TResultType>(jsonResponse);
                Debug.Log($"Success: {www.downloadHandler.text}");
                www.Dispose();

                return result;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Could not parse response {jsonResponse}.\n{ex.Message}");
                www.Dispose();

                return default;
            }
        }

        /// <summary>
        /// Execture an HTTP POST request uploading a file
        /// </summary>
        /// <typeparam name="TResultType"></typeparam>
        /// <param name="url">Endpoint</param>
        /// <param name="fieldName">Field of the body where upload</param>
        /// <param name="data">Byte[] data of the file</param>
        /// <param name="contentType">Should be always multipart/form-data</param>
        /// <param name="customHeaders">Other needed headers (such as auth). Optional.</param>
        /// <returns></returns>
        public static async Task<TResultType> Upload<TResultType>(string url, List<IMultipartFormSection> form, byte[] boundary, string contentType = "multipart/form-data", List<RequestHeader> customHeaders = null)
        {
            Debug.Log($"[Upload] ~ url: {url}");

            using var www = new UnityWebRequest(url, "POST");

            www.SetRequestHeader("Content-Type", contentType);
            www.downloadHandler = new DownloadHandlerBuffer();
            byte[] payload = null;

            if (form != null && form.Count != 0)
            {
                payload = SerializeFormSections(form, boundary);
            }

            if (payload == null)
            {
                Debug.LogError($"Payload is empty.");
                return default;
            }

            var content = System.Text.Encoding.UTF8.GetString(payload);

            Debug.Log(content);

            UploadHandler uploadHandler = new UploadHandlerRaw(payload)
            {
                contentType = "multipart/form-data; boundary=" + System.Text.Encoding.UTF8.GetString(boundary, 0, boundary.Length)
            };

            www.uploadHandler = uploadHandler;

            if (customHeaders != null)
                foreach (var header in customHeaders)
                {
                    www.SetRequestHeader(header.name, header.value);
                }

            var operation = www.SendWebRequest();

            while (!operation.isDone)
                await Task.Yield();

            var jsonResponse = www.downloadHandler.text;
            if (www.result != UnityWebRequest.Result.Success)
                Debug.LogError($"Error: {www.error}");

            try
            {
                if (string.IsNullOrEmpty(jsonResponse)) return default;

                var result = JsonUtility.FromJson<TResultType>(jsonResponse);
                Debug.Log($"Response: {www.downloadHandler.text}");
                www.Dispose();

                return result;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Could not parse response {jsonResponse}.\n{ex.Message}");
                www.Dispose();

                return default;
            }
        }

        /// <summary>
        /// Execute an HTTP GET Request
        /// </summary>
        /// <typeparam name="TResultType">Type to return as response</typeparam>
        /// <param name="url">API Endpoint</param>
        /// <param name="customHeaders">Request additional headers</param>
        /// <returns></returns>
        public static async Task<TResultType> Get<TResultType>(string url, List<RequestHeader> customHeaders = null)
        {
            using var www = UnityWebRequest.Get(url);

            www.SetRequestHeader("Content-Type", "application/json");

            if (customHeaders != null)
                foreach (var header in customHeaders)
                {
                    www.SetRequestHeader(header.name, header.value);
                }

            var operation = www.SendWebRequest();

            while (!operation.isDone)
                await Task.Yield();

            var jsonResponse = www.downloadHandler.text;
            Debug.Log(jsonResponse);
            if (www.result != UnityWebRequest.Result.Success)
                Debug.LogError($"Failed: {www.error}");

            try
            {
                var result = JsonUtility.FromJson<TResultType>(jsonResponse);
                Debug.Log($"Success {www.downloadHandler.text}");
                www.Dispose();

                return result;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Could not parse response {jsonResponse}.\n{ex.Message}");
                www.Dispose();

                return default;
            }
        }

        /// <summary>
        /// Execute an HTTP PUT Request
        /// </summary>
        /// <typeparam name="TResultType">Type to return as response</typeparam>
        /// <param name="url">API endpoint</param>
        /// <param name="body">Request body</param>
        /// <param name="contentType">Content Type of the body</param>
        /// <param name="customHeaders">Request additional headers</param>
        /// <returns></returns>
        public static async Task<TResultType> Put<TResultType>(string url, string body, string contentType = "application/json", List<RequestHeader> customHeaders = null)
        {
            Debug.Log($"[Post] ~ url: {url} \nbody: {body}");
            using var www = UnityWebRequest.Put($"{url}", body);

            www.SetRequestHeader("Content-Type", contentType);

            if (customHeaders != null)
                foreach (var header in customHeaders)
                {
                    www.SetRequestHeader(header.name, header.value);
                }

            var operation = www.SendWebRequest();

            while (!operation.isDone)
                await Task.Yield();

            var jsonResponse = www.downloadHandler.text;
            if (www.result != UnityWebRequest.Result.Success)
                Debug.LogError($"Failed: {www.error}");

            try
            {
                if (string.IsNullOrEmpty(jsonResponse)) return default;

                var result = JsonUtility.FromJson<TResultType>(jsonResponse);
                Debug.Log($"Success: {www.downloadHandler.text}");
                www.Dispose();

                return result;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Could not parse response {jsonResponse}.\n{ex.Message}");
                www.Dispose();

                return default;
            }
        }

        /// <summary>
        /// Execute an HTTP DELETE Request
        /// </summary>
        /// <typeparam name="TResultType">Type to return as response</typeparam>
        /// <param name="url">API endpoint</param>
        /// <param name="contentType">Content Type of the body</param>
        /// <param name="customHeaders">Request additional headers</param>
        /// <returns></returns>
        public static async Task<TResultType> Delete<TResultType>(string url, string contentType = "application/json", List<RequestHeader> customHeaders = null)
        {
            Debug.Log($"[Delete] ~ url: {url}");
            using var www = UnityWebRequest.Delete($"{url}");

            www.SetRequestHeader("Content-Type", contentType);

            if (customHeaders != null)
                foreach (var header in customHeaders)
                {
                    www.SetRequestHeader(header.name, header.value);
                }

            var operation = www.SendWebRequest();

            while (!operation.isDone)
                await Task.Yield();

            if (www.result != UnityWebRequest.Result.Success)
                Debug.LogError($"Failed: {www.error}");

            try
            {
                var jsonResponse = www.downloadHandler?.text;

                if (string.IsNullOrEmpty(jsonResponse)) return default;

                var result = JsonUtility.FromJson<TResultType>(jsonResponse);
                Debug.Log($"Success: {www.downloadHandler.text}");
                www.Dispose();

                return result;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Could not parse response.\n{ex.Message}");
                www.Dispose();

                return default;
            }
        }

        public static byte[] GenerateBoundary()
        {
            // Generate a random boundary
            byte[] boundary = new byte[40];
            for (int i = 0; i < 40; i++)
            {
                int randomChar = Random.Range(48, 110);
                if (randomChar > 57) // skip unprintable chars between 57 and 64 (inclusive)
                    randomChar += 7;
                if (randomChar > 90) // and 91 and 96 (inclusive)
                    randomChar += 6;
                boundary[i] = (byte)randomChar;
            }
            return boundary;
        }

        ///<summary>Converts a List of IMultipartFormSection objects into a byte array containing raw multipart form data.</summary>
        ///<param name="multipartFormSections">A List of <see cref="IMultipartFormSection" /> objects.</param>
        ///<param name="boundary">A unique boundary string to separate the form sections.</param>
        ///<returns>A byte array of raw multipart form data.</returns>
        ///<seealso cref="GenerateBoundary" />
        public static byte[] SerializeFormSections(List<IMultipartFormSection> multipartFormSections, byte[] boundary)
        {
            if (multipartFormSections == null || multipartFormSections.Count == 0)
                return null;

            byte[] crlf = System.Text.Encoding.UTF8.GetBytes("\r\n");
            byte[] dDash = System.Text.Encoding.ASCII.GetBytes("--");

            int estimatedSize = 0;
            foreach (IMultipartFormSection section in multipartFormSections)
            {
                estimatedSize += 64 + section.sectionData.Length;
            }

            List<byte> formData = new List<byte>(estimatedSize);
            foreach (IMultipartFormSection section in multipartFormSections)
            {
                string disposition = "form-data";

                string sectionName = section.sectionName;
                string fileName = section.fileName;

                string header = "Content-Disposition: " + disposition;

                if (!string.IsNullOrEmpty(sectionName))
                {
                    header += "; name=\"" + sectionName + "\"";
                }

                if (!string.IsNullOrEmpty(fileName))
                {
                    header += "; filename=\"" + fileName + "\"";
                }

                header += "\r\n";

                string contentType = section.contentType;
                if (!string.IsNullOrEmpty(contentType))
                {
                    header += "Content-Type: " + contentType + "\r\n";
                }

                formData.AddRange(crlf);
                formData.AddRange(dDash);
                formData.AddRange(boundary);
                formData.AddRange(crlf);
                formData.AddRange(System.Text.Encoding.UTF8.GetBytes(header));
                formData.AddRange(crlf);
                formData.AddRange(section.sectionData);
            }

            // end sections with boundary delimiter (https://tools.ietf.org/html/rfc2046)
            formData.AddRange(crlf);
            formData.AddRange(dDash);
            formData.AddRange(boundary);
            formData.AddRange(dDash);
            formData.AddRange(crlf);
            return formData.ToArray();
        }
    }
}