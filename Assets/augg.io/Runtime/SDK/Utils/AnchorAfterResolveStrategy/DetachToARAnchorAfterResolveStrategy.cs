using System.Collections;
using Auggio.Plugin.SDK.Model.Ids;
using Google.XR.ARCoreExtensions;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace Auggio.Runtime.SDK.Utils.AnchorAfterResolveStrategy
{
    public class DetachToARAnchorAfterResolveStrategy : AbstractAnchorAfterResolveStrategy {

        [Tooltip("The amount of time in seconds it takes for the cloud anchor to detach.")]
        [SerializeField] private float detachTime = 2f;
        
        public override void Process(AuggioAnchorId auggioAnchorId, ARCloudAnchor anchor) {
            StartCoroutine(DetachCoroutine(auggioAnchorId, anchor));
        }

        private IEnumerator DetachCoroutine(AuggioAnchorId auggioAnchorId, ARCloudAnchor anchor) {
            yield return new WaitForSeconds(detachTime);
            GameObject anchorGO = anchor.gameObject;
            Destroy(anchor);
            anchorGO.AddComponent<ARAnchor>();
        }
    }
}