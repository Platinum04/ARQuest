using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Auggio.Plugin.SDK.Utils
{
    public class DebugUI : MonoBehaviour {

        public static DebugUI Instance;
        [SerializeField] private GameObject debugMessagePrefab;
        [SerializeField] private GameObject messageWrapper;
        [SerializeField] private GameObject messagesRoot;


        private List<string> messages;
        private bool visible;
        
        // Start is called before the first frame update
        void Awake() {
            Instance = this;
            messages = new List<string>();
            visible = false;
        }

        private void OnDestroy() {
            Instance = null;
        }

        public void Toggle() {
            if (visible) {
                messageWrapper.SetActive(false);
                visible = false;
            }
            else {
                foreach (Transform child in messagesRoot.transform) {
                    Destroy(child.gameObject);
                }
                foreach (string message in messages) {
                    AddMessageToView(message);
                }
                messageWrapper.SetActive(true);
                visible = true;
            }
        }

        public void Log(string message) {
            messages.Add(message);
            if (visible) {
                AddMessageToView(message);
            }
        }

        private void AddMessageToView(string message) {
            GameObject instantiate = Instantiate(debugMessagePrefab, messagesRoot.transform);
            DebugMessage debugMessage = instantiate.GetComponent<DebugMessage>();
            debugMessage.Bind(message);
        }
    }
}
