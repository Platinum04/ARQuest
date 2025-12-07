using UnityEngine;

namespace Auggio.Plugin.SDK.Utils.DetachStrategy
{
    [RequireComponent(typeof(BoxCollider))]
    public class AuggioPlaneFinder : MonoBehaviour
    {

        private BoxCollider _collider;
        
        private void Awake()
        {
            _collider = GetComponent<BoxCollider>();
            GetComponent<Renderer>().enabled = false;
        }

        public BoxCollider Collider => _collider;
    }
}
