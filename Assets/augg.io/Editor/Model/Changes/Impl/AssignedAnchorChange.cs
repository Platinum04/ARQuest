#if UNITY_EDITOR
using Auggio.Plugin.SDK.Model;
using Auggio.Utils.Serialization.Plugin;
using Auggio.Utils.Serialization.Plugin.Experience;
using UnityEngine.SceneManagement;

namespace Auggio.Plugin.Editor.Model.Changes.Impl
{
    internal class AssignedAnchorChange : StringValueChange<AuggioObjectTracker>
    {

        private Experience experience;
        
        internal AssignedAnchorChange(Experience experience, string objectId, AuggioObjectTracker sceneComponent, string objectName, string oldValue, string newValue) : base(experience.ID, objectId, sceneComponent, objectName, oldValue, newValue)
        {
            this.experience = experience;
        }

        internal override string OldValueToString()
        {
            return TryConvertAnchorIdToName(oldValue);
        }

        internal override string NewValueToString()
        {
            return TryConvertAnchorIdToName(newValue);
        }

        internal override string UIStringName()
        {
            return "Anchor changed";
        }

        protected override AuggioObjectTracker DiscardChange(AuggioObjectTracker tracker , Scene scene, Experience experience, AuggioObject oldData)
        {
            tracker.AnchorId = oldData.AssignedAnchor;
            return tracker;
        }

        internal override void ApplyToExperience(Experience cachedExperience)
        {
            AuggioObject auggioObjectData = cachedExperience.FindObjectByObjectId(objectId);
            auggioObjectData.AssignedAnchor = newValue;
        }

        private string TryConvertAnchorIdToName(string anchorId)
        {
            foreach (Location location in experience.AssignedLocations)
            {
                foreach (SingleAnchor anchor in location.SingleAnchorList)
                {
                    if (anchor.AuggioId.Equals(anchorId))
                    {
                        return anchor.Name;
                    }
                }
            }
            return anchorId;
        }
    }
}
#endif
