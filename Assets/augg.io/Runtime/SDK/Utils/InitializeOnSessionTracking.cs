using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Authentication;
using Auggio.Plugin.SDK.Runtime;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace Auggio.Plugin.SDK.Utils {
    
    [AddComponentMenu("augg.io/InitializeOnSessionTracking")]
    public class InitializeOnSessionTracking : MonoBehaviour {
    
        [SerializeField] private AuggioTrackingManager manager;

        //[SerializeField] private List<GameObject> arObjects;
        // Start is called before the first frame update
        void Awake() {
            if (!AuthFileTokenUtility.IsFileTokenAvailable()) {
                throw new AuthenticationException("The project does not contain file token used for authentication. Please go to www.augg.io and download token for your application.");
                return;
            }
            ARSession.stateChanged += OnARSessionStateChanged;
            /*foreach (GameObject arObject in arObjects)
            {
                arObject.SetActive(true);
            }*/
        }

        private void OnDestroy() {
            ARSession.stateChanged -= OnARSessionStateChanged;
        }

        private void OnARSessionStateChanged(ARSessionStateChangedEventArgs obj) {
            try {
                Debug.Log("ARSession state: " + obj.state);
                if (obj.state == ARSessionState.SessionTracking && !manager.Initialized) {
                    manager.Initialize();
                }
            }
            catch (NullReferenceException e) {
                Debug.LogWarning("ARSession was null!");
            }
            
        }

    }
}
