using System.Collections;
using System.Collections.Generic;
using Auggio.Plugin.SDK.Model;
using Auggio.Plugin.SDK.Model.Ids;
using Auggio.Plugin.SDK.Utils;
using Auggio.Utils.Serialization.Plugin;
using Auggio.Utils.Serialization.Plugin.Experience;
using Google.XR.ARCoreExtensions;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace Auggio.Plugin.SDK.Runtime {
    
    public class Resolver {

        public delegate void OnAnchorResolved(AuggioAnchorId id, ARCloudAnchor anchor);
        /**
         * Serves as a queue from which the resolve process is fed ids.
         */
        //private Dictionary<AuggioAnchorId, CloudAnchorId> anchorsToResolve = new Dictionary<AuggioAnchorId, CloudAnchorId>();

        /**
         * Anchors that are pending resolve. The request is being processed by Google.
         */
        private Dictionary<AuggioAnchorId, AnchorResolveData> pendingAnchors = new Dictionary<AuggioAnchorId, AnchorResolveData>();
        
        /**
         *  Resolved cloud anchors. Key is auggio anchor id
         */
        private Dictionary<AuggioAnchorId, ARCloudAnchor> resolvedCloudAnchors = new Dictionary<AuggioAnchorId, ARCloudAnchor>();

        internal Dictionary<AuggioAnchorId, ARCloudAnchor> ResolvedCloudAnchors => resolvedCloudAnchors;


        private MonoBehaviour coroutineHelper;
        private ARAnchorManager anchorManager;
        private OnAnchorResolved onAnchorResolved;
        
        internal Resolver(MonoBehaviour coroutineHelper, ARAnchorManager anchorManager, OnAnchorResolved onAnchorResolved) {
            this.coroutineHelper = coroutineHelper;
            this.anchorManager = anchorManager;
            this.onAnchorResolved = onAnchorResolved;
        }
        
        public void Resolve(SingleAnchor anchor) {
            //check if anchor is already not resolved
            //check if anchor is not being resolved
            if (IsAnchorBeingResolved(anchor)) return;
            if (IsAnchorResolved(anchor, out ARCloudAnchor cloudAnchor)) {
                //call callback again
                onAnchorResolved?.Invoke(anchor.AuggioId, cloudAnchor);
                return;
            }
            LogProcess("Started resolving " + anchor.Name);
            ResolveCloudAnchorPromise promise = anchorManager.ResolveCloudAnchorAsync(anchor.GoogleAnchorId);
            Coroutine handleCoroutine = StartCoroutine(HandleResolvePromise(promise, anchor.AuggioId));
            AnchorResolveData data = new AnchorResolveData(handleCoroutine, promise);
            pendingAnchors.Add(anchor.AuggioId, data);
        }
        
        private IEnumerator HandleResolvePromise(ResolveCloudAnchorPromise promise, string auggioAnchorId) {
            yield return promise;
            pendingAnchors.Remove(auggioAnchorId);
            ResolvePromiseProcessor processor = new ResolvePromiseProcessor();
            if (processor.Process(promise, out ARCloudAnchor anchor)) {
                resolvedCloudAnchors.Add(auggioAnchorId, anchor);
                onAnchorResolved?.Invoke(auggioAnchorId, anchor);
            }
        }

        private bool IsAnchorResolved(SingleAnchor anchor, out ARCloudAnchor cloudAnchor) {
            return resolvedCloudAnchors.TryGetValue(anchor.AuggioId, out cloudAnchor);
        }

        private bool IsAnchorBeingResolved(SingleAnchor anchor) {
            return pendingAnchors.ContainsKey(anchor.AuggioId);
        }
        
        
        
        

        internal void Clear() {
            ClearTrackingState();
        }
        
        private void ClearTrackingState() {
            if (resolvedCloudAnchors == null) return;
            CancelAllPendingResolves();
            foreach (KeyValuePair<AuggioAnchorId,ARCloudAnchor> cloudAnchor in resolvedCloudAnchors) {
                Object.Destroy(cloudAnchor.Value.gameObject);
            }
            resolvedCloudAnchors.Clear();
        }
        
        internal void CancelAllPendingResolves() {
            if (pendingAnchors.Count == 0) return;
            foreach (KeyValuePair<AuggioAnchorId, AnchorResolveData> pendingAnchorPair in pendingAnchors) {
                AnchorResolveData pendingAnchor = pendingAnchorPair.Value;
                StopCoroutine(pendingAnchor.Coroutine);
                pendingAnchor.Promise.Cancel();
            }
            pendingAnchors.Clear();
        }
        
        private Coroutine StartCoroutine(IEnumerator routine) {
            return coroutineHelper.StartCoroutine(routine);
        }
        private void StopCoroutine(Coroutine routine) {
            coroutineHelper.StopCoroutine(routine);
        }

        internal void LogProcess(string log) {
            Debug.Log(log);
            if (DebugUI.Instance != null) {
                DebugUI.Instance.Log(log);
            }
        }
        
        

        
    }
}
