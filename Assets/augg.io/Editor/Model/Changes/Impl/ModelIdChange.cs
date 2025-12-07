#if UNITY_EDITOR
using Auggio.Plugin.SDK.Model;
using Auggio.Plugin.SDK.Utils;
using Auggio.Utils.Serialization.Plugin;
using Auggio.Utils.Serialization.Plugin.Experience;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Auggio.Plugin.Editor.Model.Changes.Impl
{
    internal class ModelIdChange : StringValueChange<AuggioObjectPlaceholderModel>
    {

        private string _placeholderId;
        
        public ModelIdChange(string experienceId, string objectId, string placeholderId, AuggioObjectPlaceholderModel sceneComponent, string objectName, string oldValue, string newValue) : base(experienceId, objectId, sceneComponent, objectName, oldValue, newValue)
        {
            _placeholderId = placeholderId;
        }

        internal override string UIStringName()
        {
            return "Model representation changed";
        }

        internal override string OldValueToString()
        {
            return PrimitiveTypeHelper.GetPrimitiveTypeByModelId(oldValue).ToString();
        }

        internal override string NewValueToString()
        {
            return PrimitiveTypeHelper.GetPrimitiveTypeByModelId(newValue).ToString();
        }

        protected override AuggioObjectPlaceholderModel DiscardChange(AuggioObjectPlaceholderModel component, Scene scene, Experience experience, AuggioObject oldData)
        {
            AuggioObjectPlaceholder objectPlaceholder = oldData.FindPlaceholderById(_placeholderId);
            if (objectPlaceholder == null)
            {
                string error = "Cannot find placeholder model with this ID in cached data";
                AuggioEditorPlugin.ShowErrorDialog(error);
                Debug.LogError(error);
                return component;
            }

            component.ModelId = objectPlaceholder.ModelId;
            return component;
        }

        internal override void ApplyToExperience(Experience cachedExperience)
        {
            AuggioObject objectData = cachedExperience.FindObjectByObjectId(objectId);
            AuggioObjectPlaceholder objectPlaceholder = objectData.FindPlaceholderById(_placeholderId);
            if (objectPlaceholder == null)
            {
                string error = "Cannot find placeholder model with this ID in cached data";
                AuggioEditorPlugin.ShowErrorDialog(error);
                Debug.LogError(error);
                return;
            }
            objectPlaceholder.ModelId = newValue;
        }
    }
}
#endif
