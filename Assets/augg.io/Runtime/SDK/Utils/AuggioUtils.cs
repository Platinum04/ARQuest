using System.IO;
using UnityEngine;

namespace Auggio.Plugin.SDK.Utils
{
    public class AuggioUtils
    {

        public const float Epsilon = 0.001f;
        
        public const string OBJECT_NAME_PREFIX = "[Object] ";
        public const string OBJECT_PLACEHOLDER_NAME_PREFIX = "[Object Placeholder] ";
        public const string EXPERIENCE_NAME_PREFIX = "[Experience] ";
        public const string LOCATION_NAME_PREFIX = "[Location] ";
        public const string ANCHOR_NAME_PREFIX = "[Anchor] ";
        
        public static string GetGameObjectName(string objectName)
        {
            return OBJECT_NAME_PREFIX  + objectName;
        }
        
        public static string GetGameObjectPlaceholderName(string placeholderName)
        {
            return OBJECT_PLACEHOLDER_NAME_PREFIX  + placeholderName;
        }

        public static string GetExperienceGameObjectName(string experienceName)
        {
            return EXPERIENCE_NAME_PREFIX  + experienceName;
        }
        
        public static string GetLocationGameObjectName(string locatioName)
        {
            return LOCATION_NAME_PREFIX + locatioName;
        }
        
        public static string GetAnchorGameObjectName(string anchorName)
        {
            return ANCHOR_NAME_PREFIX  + anchorName;
        }

        public static string GetExperienceFilePath(string organizationId, string experienceId, bool relativeToAssetsPath = false)
        {
            return Path.Combine(GetExperienceDirectoryPath(organizationId, relativeToAssetsPath), experienceId);
        }
        
        public static string GetOrganizationDirectoryPath(string organizationId, bool relativeToAssetsPath = false)
        {
            return Path.Combine(relativeToAssetsPath? "Assets" : Application.dataPath, "augg.io", "data", organizationId);
        }
        
        public static string GetExperienceDirectoryPath(string organizationId, bool relativeToAssetsPath = false)
        {
            return Path.Combine(GetOrganizationDirectoryPath(organizationId, relativeToAssetsPath), "experiences");
        }

        public static string GetMeshDataPath(bool relativeToAssetsPath = false)
        {
            return Path.Combine(relativeToAssetsPath? "Assets" : Application.dataPath, "augg.io", "data", "mesh_data");
        }
        
        
    }
}
