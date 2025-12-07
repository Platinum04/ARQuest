using UnityEngine;

namespace Auggio.Plugin.SDK.Utils.DetachStrategy
{
    public abstract class AbstractDetachStrategy : MonoBehaviour
    {
        public abstract void Initialize();
        public abstract void OnUpdate();

        public abstract void Disable();

    }
}
