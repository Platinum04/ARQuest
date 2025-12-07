#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
#if UNITY_EDITOR
using Unity.EditorCoroutines.Editor;
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Networking;

namespace Auggio.Plugin.Editor.Utils
{
    public static class WebRequestUtility
    {
      
        public static EditorCoroutine WebRequestPlugin(AuggioEditorPlugin editorWindow, HttpMethod httpMethod, string url,string body, string apiKey, Action<string> callback, Action<string> onError) {
            List<Header> headers = new List<Header>();
            headers.Add(new Header("Content-Type", "application/json"));
            headers.Add(new Header("Auggio-Api-Key", apiKey));
            headers.Add(new Header("Auggio-Plugin-Version", Plugin.SDK.Utils.Version.pluginVersion));
            if (editorWindow.SelectedOrganizationId != null)
            {
                headers.Add(new Header("Selected-Organization", editorWindow.SelectedOrganizationId));
            }
            //headers.Add(new Header("Auggio-SDK-Version", Version.sdkVersion));
            
            return EditorCoroutineUtility.StartCoroutine(SendWebRequest(httpMethod, editorWindow.ServerUrl + url, body,headers, callback, onError), editorWindow);
        }

        private static IEnumerator SendWebRequest(HttpMethod httpMethod, string url, string body, List<Header> headers, Action<string> callback, Action<string> onError, Action<float> onProgress = null)
        {
            using (UnityWebRequest webRequest = new UnityWebRequest(url, httpMethod.ToString()))
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

#endif