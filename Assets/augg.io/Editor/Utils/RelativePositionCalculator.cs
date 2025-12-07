using Auggio.Utils.Serialization.Plugin;
using UnityEngine;

namespace Auggio.Plugin.Editor.Utils
{
    internal class RelativePositionCalculator : MonoBehaviour
    {
        internal static Vector3 ObjectPositionRelativeToAnchor(Transform relativeToAnchor, AuggioObject auggioObject)
        {
            return relativeToAnchor.TransformPoint(auggioObject.Position.Deserialize());
        }

        internal static Quaternion ObjectRotationRelativeToAnchor(Transform relativeToAnchor, AuggioObject auggioObject)
        {
            return relativeToAnchor.rotation * Quaternion.Euler(auggioObject.Rotation.Deserialize());
        }

        internal static Vector3 ObjectScaleRelativeToAnchor(Transform relativeToAnchor, AuggioObject auggioObject)
        {
            return Vector3.Scale(auggioObject.Scale.Deserialize(), relativeToAnchor.localScale);
        }

        internal static Vector3 InversedObjectPositionRelativeToAnchor(Transform relativeToAnchor, Vector3 position)
        {
            return relativeToAnchor.InverseTransformPoint(position);
        }

        internal static Quaternion InversedObjectRotationRelativeToAnchor(Transform relativeToAnchor,
            Quaternion rotation)
        {
            return Quaternion.Inverse(relativeToAnchor.rotation) * rotation;
        }

        internal static Vector3 InversedObjectScaleRelativeToAnchor(Transform relativeToAnchor, Vector3 scale)
        {
            return new Vector3(scale.x / relativeToAnchor.localScale.x, scale.y / relativeToAnchor.localScale.y,
                scale.z / relativeToAnchor.localScale.z);
        }
    }
}