using Auggio.Plugin.SDK.Runtime;
using Google.XR.ARCoreExtensions;
using UnityEngine;

namespace Auggio.Plugin.SDK.Model {
    internal class AnchorResolveData {
        private Coroutine coroutine;
        private ResolveCloudAnchorPromise promise;

        public AnchorResolveData(Coroutine coroutine, ResolveCloudAnchorPromise promise) {
            this.coroutine = coroutine;
            this.promise = promise;
        }

        public Coroutine Coroutine => coroutine;

        public ResolveCloudAnchorPromise Promise => promise;

    }
}
