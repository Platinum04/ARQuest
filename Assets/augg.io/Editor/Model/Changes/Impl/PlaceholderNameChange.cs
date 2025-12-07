#if UNITY_EDITOR
using Auggio.Plugin.SDK.Model;
using Auggio.Plugin.SDK.Utils;
using Auggio.Utils.Serialization.Plugin;
using Auggio.Utils.Serialization.Plugin.Experience;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Auggio.Plugin.Editor.Model.Changes.Impl
{
    internal class PlaceholderNameChange : StringValueChange<AuggioObjectPlaceholderModel>
    {
        private string _placeholderId;
        
        internal PlaceholderNameChange(string experienceId, string objectId,  string placeholderId, AuggioObjectPlaceholderModel sceneComponent, string objectName, string oldValue, string newValue) : base(experienceId, objectId, sceneComponent, objectName, oldValue, newValue)
        {
            _placeholderId = placeholderId;
        }

        internal override string UIStringName()
        {
            return "Placeholder renamed \n["+(string.IsNullOrEmpty(sceneComponent.PlaceholderName) ? "empty name" : sceneComponent.PlaceholderName)+"]";
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
            model.PlaceholderName = auggioObjectPlaceholder.Name;
            model.gameObject.name = AuggioUtils.GetGameObjectPlaceholderName(auggioObjectPlaceholder.Name);

            return model;
        }

        internal override void ApplyToExperience(Experience cachedExperience)
        {
            AuggioObject objectData = cachedExperience.FindObjectByObjectId(objectId);
            AuggioObjectPlaceholder auggioObjectPlaceholder = objectData.FindPlaceholderById(_placeholderId);
            if (auggioObjectPlaceholder == null)
            {
                string error = "Cannot apply to experience because old data are missing placeholder with this ID.";
                AuggioEditorPlugin.ShowErrorDialog(error);
                Debug.LogError(error);
                return;
            }
            auggioObjectPlaceholder.Name = newValue;
        }
    }
}
#endif
