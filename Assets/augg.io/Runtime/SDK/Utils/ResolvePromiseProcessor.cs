using System;
using Google.XR.ARCoreExtensions;
using UnityEngine;

namespace Auggio.Plugin.SDK.Utils {
    internal class ResolvePromiseProcessor
    {

        public bool Process(ResolveCloudAnchorPromise promise, out ARCloudAnchor anchor) {
            if (promise.State == PromiseState.Done) {
                switch (promise.Result.CloudAnchorState) {
                    case CloudAnchorState.Success:
                        anchor = promise.Result.Anchor;
                        return true;
                    default:
                        LogProcess("Could not resolve anchor. Reason: " + promise.Result.CloudAnchorState);
                        anchor = null;
                        return false;
                }
            }
            else {
                LogProcess("Promise state not done.");
                anchor = null;
                return false;
            }
        }
        
        internal void LogProcess(string log) {
            Debug.LogError(log);
            if (DebugUI.Instance != null) {
                DebugUI.Instance.Log(log);
            }
        }
    }
}
