#if UNITY_EDITOR
using System.Collections.Generic;
using Auggio.Plugin.SDK.Model;
using UnityEngine;

namespace Auggio.Plugin.Editor.Validation.Validators
{
    internal class AuggioObjectPlaceholderValidator : IValidator<AuggioObjectPlaceholderModel>
    {
           

        public Dictionary<ErrorCode, string> Validate(AuggioObjectPlaceholderModel value)
        {
            Dictionary<ErrorCode, string> errors = new Dictionary<ErrorCode, string>();

            if (IsPlaceholderNameEmpty(value))
            {
                errors.Add(ErrorCode.AUGGIO_PLACEHOLDER_NAME_EMPTY, "Placeholder name cannot be empty");
            }

            if (!IsPlaceholderModelUnique(value))
            {
                errors.Add(ErrorCode.AUGGIO_PLACEHOLDER_DUPLICATE, "Duplicate placeholder ID found");
            }

            if (!IsRegisteredInObjectTracker(value))
            {
                errors.Add(ErrorCode.AUGGIO_PLACEHOLDER_NOT_REGISTERED, "Placeholder object is no assigned to any augg.io object");

            }

            if (!IsChildOfCorrectObjectTracker(value))
            {
                errors.Add(ErrorCode.AUGGIO_PLACEHOLDER_DIFFERENT_PARENT, "Placeholder object is child of different object than it should be");
            }
            
            return errors;
        }
        
        private bool IsChildOfCorrectObjectTracker(AuggioObjectPlaceholderModel model)
        {
            AuggioObjectTracker tracker = model.GetComponentInParent<AuggioObjectTracker>(true);
            if (tracker == null)
            {
                return false;
            }

            if (model.AuggioObjectTracker == null)
            {
                return false;
            }

            return tracker.ObjectId == model.AuggioObjectTracker.ObjectId;
        }
        
        private bool IsRegisteredInObjectTracker(AuggioObjectPlaceholderModel model)
        {
            return model.AuggioObjectTracker != null && model.AuggioObjectTracker.Models.Contains(model);
        }
        
        private bool IsPlaceholderModelUnique(AuggioObjectPlaceholderModel model)
        {
            if (string.IsNullOrEmpty(model.PlaceholderId))
            {
                return true;
            }

            int count = 0;
            foreach (GameObject rootGameObject in model.gameObject.scene.GetRootGameObjects())
            {
                AuggioObjectPlaceholderModel[] allPlaceholderModels = rootGameObject.GetComponentsInChildren<AuggioObjectPlaceholderModel>(true);
                foreach (AuggioObjectPlaceholderModel otherPlaceholderModel in allPlaceholderModels)
                {
                    if (string.IsNullOrEmpty(otherPlaceholderModel.PlaceholderId))
                    {
                        continue;
                    }

                    if (otherPlaceholderModel.PlaceholderId.Equals(model.PlaceholderId))
                    {
                        count++;
                        if (count > 1)
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }
        
        private bool IsPlaceholderNameEmpty(AuggioObjectPlaceholderModel model)
        {
            return string.IsNullOrEmpty(model.PlaceholderName);
        }

    }
}
#endif
