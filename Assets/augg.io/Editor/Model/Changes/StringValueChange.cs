#if UNITY_EDITOR
using UnityEngine;

namespace Auggio.Plugin.Editor.Model.Changes
{
    internal abstract class StringValueChange<C> : AuggioObjectChange<C, string> where C : MonoBehaviour
    {
        protected StringValueChange(string experienceId, string objectId, C sceneComponent, string objectName, string oldValue, string newValue) : base(experienceId, objectId, sceneComponent, objectName, oldValue, newValue)
        {
        }

        internal override string OldValueToString()
        {
            return oldValue;
        }

        internal override string NewValueToString()
        {
            return newValue;
        }
    }
}
#endif
