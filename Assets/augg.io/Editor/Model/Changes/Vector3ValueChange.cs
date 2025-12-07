#if UNITY_EDITOR
using Auggio.Utils.Serialization.Model;
using UnityEngine;

namespace Auggio.Plugin.Editor.Model.Changes
{
    internal abstract class Vector3ValueChange<C> : AuggioObjectChange<C, SerializedVector3> where C : MonoBehaviour
    {
        protected Vector3ValueChange(string experienceId, string objectId,  C sceneComponent, string objectName, SerializedVector3 oldValue, SerializedVector3 newValue) 
            : base(experienceId, objectId, sceneComponent, objectName, oldValue, newValue)
        {
        }

        internal override string OldValueToString()
        {
            return oldValue.Deserialize().ToString();
        }

        internal override string NewValueToString()
        {
            return newValue.Deserialize().ToString();
        }
    }
}
#endif