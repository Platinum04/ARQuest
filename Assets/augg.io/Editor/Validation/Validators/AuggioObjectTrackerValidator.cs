#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using Auggio.Plugin.SDK.Model;
using Auggio.Plugin.SDK.Utils;
using UnityEngine;

namespace Auggio.Plugin.Editor.Validation.Validators
{
    internal class AuggioObjectTrackerValidator : IValidator<AuggioObjectTracker>
    {
        private bool IsObjectNameEmpty(AuggioObjectTracker tracker)
        {
            return string.IsNullOrEmpty(tracker.ObjectName);
        }
        
        private bool IsObjectTrackerUnique(AuggioObjectTracker tracker)
        {
            if (string.IsNullOrEmpty(tracker.ObjectId))
            {
                return true;
            }

            int count = 0;
            foreach (GameObject rootGameObject in tracker.gameObject.scene.GetRootGameObjects())
            {
                AuggioObjectTracker[] allTrackers = rootGameObject.GetComponentsInChildren<AuggioObjectTracker>(true);
                foreach (AuggioObjectTracker otherTracker in allTrackers)
                {
                    if (string.IsNullOrEmpty(otherTracker.ObjectId))
                    {
                        continue;
                    }

                    if (otherTracker.ObjectId.Equals(tracker.ObjectId))
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

        public Dictionary<ErrorCode, string> Validate(AuggioObjectTracker value)
        {
            Dictionary<ErrorCode, string> errors = new Dictionary<ErrorCode, string>();
            if(string.IsNullOrEmpty(value.OrganizationId))
            {
                errors.Add(ErrorCode.AUGGIO_TRACKER_MISSING_ORGANIZATION_ID, "augg.io object is missing organization info.");
            }

            if (string.IsNullOrEmpty(value.ExperienceId))
            {
                errors.Add(ErrorCode.AUGGIO_TRACKER_MISSING_EXPERIENCE_ID, "augg.io object is missing experience info.");
            }
            if (!IsObjectTrackerUnique(value))
            {
                errors.Add(ErrorCode.AUGGIO_TRACKER_NOT_UNIQUE, "Duplicate augg.io object ID found.");
            }

            if (IsObjectNameEmpty(value))
            {
                errors.Add(ErrorCode.AUGGIO_TRACKER_NAME_EMPTY, "Object Name cannot be empty");
            }

            if (string.IsNullOrEmpty(value.AnchorId))
            {
                errors.Add(ErrorCode.AUGGIO_TRACKER_MISSING_ANCHOR, "Parent anchor cannot be empty. Object is corrupted.");
            }
            
            return errors;
        }
    }
}
#endif
