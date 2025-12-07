#if UNITY_EDITOR
using Auggio.Plugin.Editor.Utils;
using Auggio.Plugin.SDK.Model;
using Auggio.Utils.Serialization.Plugin;
using Auggio.Utils.Serialization.Plugin.Experience;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Auggio.Plugin.Editor.Model.Changes.Impl
{
    internal class PlaceholderDeleted : StringValueChange<AuggioObjectPlaceholderModel>
    {

        private string _placeholderId; 
        
        internal PlaceholderDeleted(string experienceId, string objectId, string placeholderId, AuggioObjectPlaceholderModel sceneComponent, string objectName, string oldValue, string newValue) : base(experienceId, objectId, sceneComponent, objectName, oldValue, newValue)
        {
            _placeholderId = placeholderId;
        }

        internal override string UIStringName()
        {
            return "Removed object placeholder";
        }

        protected override AuggioObjectPlaceholderModel DiscardChange(AuggioObjectPlaceholderModel model, Scene scene, Experience experience, AuggioObject oldData)
        {
            AuggioObjectTracker tracker = AuggioSceneManager.GetAuggioObjectTrackerFromScene(scene, experienceId, objectId);

            if (tracker == null)
            {
                string error = "Missing augg.io tracker in scene";
                AuggioEditorPlugin.ShowErrorDialog(error);
                Debug.LogError(error);
                return model;
            }

            return AuggioSceneImporter.ImportAuggioObjectPlaceholder(scene, experienceId, tracker,
                oldData.FindPlaceholderById(_placeholderId), tracker.transform);
         
        }

        internal override void ApplyToExperience(Experience cachedExperience)
        {
            cachedExperience.FindObjectByObjectId(objectId).PlaceholderModels
                .RemoveAll(placeholder => placeholder.ID.Equals(_placeholderId));
        }
    }
}
#endif
