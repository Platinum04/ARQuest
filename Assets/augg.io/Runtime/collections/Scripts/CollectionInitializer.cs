using System.Collections.Generic;
using System.Net.Http;
using Auggio.Collections.Http;
using Auggio.Plugin.SDK.Utils;
using Auggio.Utils.Serialization;
using UnityEngine;

namespace Auggio.Collections.Scripts
{
    internal class CollectionInitializer : MonoBehaviour {
        internal delegate void OnCollectionsProcessed(Dictionary<string,Collection> collections);
        internal delegate void OnCollectionsProcessError(ErrorResponse response);
        private delegate void OnCollectionsFetched(List<Collection> collections);
        private delegate void OnCollectionsFetchError(string response);
        
        private Coroutine initializeWebRequest;

        private OnCollectionsProcessed onCollectionsProcessed;
        private CollectionsProvider.OnProviderInitializeProgressCallback onProgress;
        private OnCollectionsProcessError onError;
        
        public void Initialize(OnCollectionsProcessed onCollectionsProcessed, CollectionsProvider.OnProviderInitializeProgressCallback onProgress, OnCollectionsProcessError onError) {
            this.onCollectionsProcessed = onCollectionsProcessed;
            this.onProgress = onProgress;
            this.onError = onError;
            FetchCollectionData(HandleOnFetch, HandleError);
        }

        private void HandleOnFetch(List<Collection> collections) {
            CollectionsProcessor processor = new CollectionsProcessor();
            Dictionary<string,Collection> collectionsDictionary = processor.ProcessReferences(collections);
            onCollectionsProcessed?.Invoke(collectionsDictionary);
        }

        private void HandleError(string error) {
            onError?.Invoke(ErrorResponse.Get(error));
        }
        
        private void FetchCollectionData(OnCollectionsFetched onFetchCompleted, OnCollectionsFetchError onError) {
            string fileToken = AuthFileTokenUtility.GetToken();
            initializeWebRequest = StartCoroutine(WebRequestUtility.WebRequestSDK(HttpMethod.Post, "/sdk/collections/all", string.Empty, fileToken, (response) => {
                initializeWebRequest = null;
                GetCollectionsResponse getCollectionsResponse = JsonUtility.FromJson<GetCollectionsResponse>(response);
                
                List<Collection> collections = new List<Collection>();
                foreach (CollectionDetail collectionDetail in getCollectionsResponse.Collections)
                {
                    collections.Add(collectionDetail);
                }

                onFetchCompleted?.Invoke(collections);
            }, (s) => {
               onError?.Invoke(s);
            }, (progress) => {
                onProgress?.Invoke(progress);
            }));
        }
    }
}
