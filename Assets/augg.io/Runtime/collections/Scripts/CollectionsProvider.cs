using System;
using System.Collections.Generic;
using System.Security.Authentication;
using Auggio.Plugin.SDK.Utils;
using Auggio.Utils.Serialization;
using UnityEngine;

namespace Auggio.Collections.Scripts {
    
    [RequireComponent(typeof(CollectionInitializer))]
    public class CollectionsProvider : MonoBehaviour {
        public delegate void OnProviderInitializedCallback();
        public delegate void OnProviderInitializeErrorCallback();
        public delegate void OnProviderInitializeProgressCallback(float progress);
        
        public static CollectionsProvider Instance;
        
        public OnProviderInitializedCallback OnProviderInitialized;
        public OnProviderInitializeErrorCallback OnProviderInitializeError;
        public OnProviderInitializeProgressCallback OnProviderInitializedProgress;
        
        public bool Initialized => initialized;
        private bool initialized = false;
        
        private CollectionInitializer _initializer;

        private Dictionary<string, Collection> collections;
        public Dictionary<string, Collection> Collections => collections;

        [SerializeField] private bool initializeOnAwake = true;

        private bool initializing;


        private void Awake() {
            Instance = this;
            _initializer = GetComponent<CollectionInitializer>();
            if (initializeOnAwake) {
                Initialize();
            }
        }
        
        private void OnDestroy() {
            if (Instance == this) {
                Instance = null;
            }
        }

        public void Initialize() {
            if (initialized || initializing) return;
            if (AuthFileTokenUtility.IsFileTokenAvailable()) {
                Debug.Log("Start collections initialization.");
                InitializeInternal();
            }
            else {
                throw new AuthenticationException("The project does not contain file token used for authentication. Please go to www.augg.io and download token for your application.");
            }
        }

        public bool GetCollectionByName(string name, out Collection collection) {
            return collections.TryGetValue(name, out collection);
        }

        private void InitializeInternal() {
            _initializer.Initialize(HandleOnProcessed, HandleProgress, HandleError);
            initializing = true;
        }
        
        private void HandleOnProcessed(Dictionary<string,Collection> collections) {
            this.collections = collections;
            OnProviderInitialized?.Invoke();
            Debug.Log("Collections Initialized.");
            initializing = false;
            initialized = true;
        }

        private void HandleProgress(float progress) {
            OnProviderInitializedProgress?.Invoke(progress);
        }

        private void HandleError(ErrorResponse errorResponse) {
            if (errorResponse.ErrorCode == ErrorPopupCode.AG010) {
                throw new NotSupportedException("This version of the SDK is no longer supported. Please update to a new version.");
            }
            Debug.LogError("Could not fetch collection data from the server.");
            OnProviderInitializeError?.Invoke();
            initializing = false;
        }

        
        
    }
}
