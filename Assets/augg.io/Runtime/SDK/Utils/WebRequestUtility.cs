using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace Auggio.Plugin.SDK.Utils
{
    public static class WebRequestUtility
    {
        private const string SERVER_URL = "https://api.augg.io";
        
        public static IEnumerator WebRequestSDK(HttpMethod httpMethod, string url, string body, string tokenKey, Action<string> callback, Action<string> onError, Action<float> onProgress = null) {
            List<Header> headers = new List<Header>();
            //TODO headers
            headers.Add(new Header("Content-Type", "application/json"));
            headers.Add(new Header("Auggio-Application-Token", tokenKey));
            headers.Add(new Header("Auggio-SDK-Version", Version.sdkVersion));
            
            return SendWebRequest(httpMethod, url, body, headers, callback, onError, onProgress);
        }
        private static IEnumerator SendWebRequest(HttpMethod httpMethod, string url, string body, List<Header> headers, Action<string> callback, Action<string> onError, Action<float> onProgress = null)
        {
            using (UnityWebRequest webRequest = new UnityWebRequest(SERVER_URL + url, httpMethod.ToString()))
            {
                webRequest.downloadHandler = new DownloadHandlerBuffer();
                foreach (Header header in headers) {
                    webRequest.SetRequestHeader(header.Name, header.Value);
                }
                
                if (!string.IsNullOrEmpty(body))
                {
                    byte[] bodyData = Encoding.UTF8.GetBytes(body);
                    webRequest.uploadHandler = new UploadHandlerRaw(bodyData);
                }


                UnityWebRequestAsyncOperation operation = webRequest.SendWebRequest();
                while (!operation.isDone) {
                    //TODO progress
                    onProgress?.Invoke(operation.progress);
                    yield return null;
                }

                if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
                {
                    string error = string.IsNullOrEmpty(webRequest.downloadHandler.text)
                        ? webRequest.error
                        : webRequest.downloadHandler.text;
                    Debug.LogError(webRequest.error);
                    onError?.Invoke(error);
                }
                else
                {
                    string responseBody = webRequest.downloadHandler.text;
                    callback?.Invoke(responseBody);
                }
            }
        }

        public class Header {
            private string name;
            private string value;

            public Header(string name, string value) {
                this.name = name;
                this.value = value;
            }

            public string Name => name;

            public string Value => value;
        }
    }
}