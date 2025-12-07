using Auggio.Plugin.SDK.Model.Ids;
using Google.XR.ARCoreExtensions;
using UnityEngine;

namespace Auggio.Runtime.SDK.Utils.AnchorAfterResolveStrategy
{
    public abstract class AbstractAnchorAfterResolveStrategy : MonoBehaviour {
        
        public abstract void Process(AuggioAnchorId auggioAnchorId, ARCloudAnchor anchor);
        
    }
}
