using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

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
        public static async Task<TResultType> Upload<TResultType>(string url, WWWForm form, string contentType = "multipart/form-data", List<RequestHeader> customHeaders = null)
        {
            Debug.Log($"[Upload] ~ url: {url}");

            using var www = UnityWebRequest.Post($"{url}", form);

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
    }
}