#if UNITY_EDITOR
using System.IO;
using System.Text;
using Auggio.Plugin.SDK.Utils;
using Auggio.Utils.Serialization.Plugin.Experience;
using UnityEditor;
using UnityEngine;

namespace Auggio.Plugin.Editor.Utils
{
    internal class ExperienceFileCache : AbstractFileCache<Experience>
    {

        private string _experienceId;
        private string _organizationId;
        
        public ExperienceFileCache(string organizationId, string experienceId) : 
            base(AuggioUtils.GetExperienceDirectoryPath(organizationId), experienceId)
        {
            _experienceId = experienceId;
            _organizationId = organizationId;
        }

        internal override void Save(Experience experience)
        {
            string json = JsonUtility.ToJson(experience);
            byte[] bytes = Encoding.UTF8.GetBytes(json);
            File.WriteAllBytes(Path.Combine(directoryPath, fileName), bytes);
            AssetDatabase.Refresh();
        }

        internal override Experience Load()
        {
            byte[] bytes = File.ReadAllBytes(Path.Combine(directoryPath, fileName));
            string json = Encoding.UTF8.GetString(bytes);
            return JsonUtility.FromJson<Experience>(json);
        }

        internal override void Delete()
        {
            string path = Path.Combine(directoryPath, fileName);
            string pathMeta = path + ".meta";
            if (File.Exists(path))
            {
                File.Delete(path);
            }

            if (File.Exists(pathMeta))
            {
                File.Delete(pathMeta);
            }
        }
    }
}
#endif
