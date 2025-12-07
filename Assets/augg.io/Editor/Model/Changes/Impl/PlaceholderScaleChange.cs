#if UNITY_EDITOR
using Auggio.Plugin.SDK.Model;
using Auggio.Utils.Serialization.Model;
using Auggio.Utils.Serialization.Plugin;
using Auggio.Utils.Serialization.Plugin.Experience;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Auggio.Plugin.Editor.Model.Changes.Impl
{
    internal class PlaceholderScaleChange : Vector3ValueChange<AuggioObjectPlaceholderModel>
    {
        private string _placeholderId;
        
        internal PlaceholderScaleChange(string experienceId, string objectId,  string placeholderId, AuggioObjectPlaceholderModel sceneComponent, string objectName, SerializedVector3 oldValue, SerializedVector3 newValue) : base(experienceId, objectId, sceneComponent, objectName, oldValue, newValue)
        {
            _placeholderId = placeholderId;
        }

        internal override string UIStringName()
        {
            return "Placeholder Scale changed \n["+(string.IsNullOrEmpty(sceneComponent.PlaceholderName) ? "empty name" : sceneComponent.PlaceholderName)+"]";
        }

        protected override AuggioObjectPlaceholderModel DiscardChange(AuggioObjectPlaceholderModel model, Scene scene, Experience experience, AuggioObject oldData)
        {
            AuggioObjectPlaceholder auggioObjectPlaceholder = oldData.FindPlaceholderById(model.PlaceholderId);
            if (auggioObjectPlaceholder == null)
            {
                string error = "Cannot discard change because old data are missing placeholder with this ID.";
                AuggioEditorPlugin.ShowErrorDialog(error);
                Debug.LogError(error);
                return model;
            }

            model.transform.localScale = auggioObjectPlaceholder.Scale.Deserialize();
            return model;
        }

        internal override void ApplyToExperience(Experience cachedExperience)
        {
            AuggioObject objectData = cachedExperience.FindObjectByObjectId(objectId);
            AuggioObjectPlaceholder auggioObjectPlaceholder = objectData.FindPlaceholderById(_placeholderId);
            if (auggioObjectPlaceholder == null)
            {
                string error = "Cannot discard change because old data are missing placeholder with this ID.";
                AuggioEditorPlugin.ShowErrorDialog(error);
                Debug.LogError(error);
                return;
            }
            auggioObjectPlaceholder.Scale = newValue;
        }
    }
}
#endif
