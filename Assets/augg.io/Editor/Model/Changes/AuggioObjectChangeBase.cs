#if UNITY_EDITOR
using Auggio.Utils.Serialization.Plugin;
using Auggio.Utils.Serialization.Plugin.Experience;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Auggio.Plugin.Editor.Model.Changes
{
    internal abstract class AuggionObjectChangeBase
    {
     
        protected string objectName;
        protected string experienceId;
        protected string objectId;
        protected GameObject sceneGameObject;

        protected AuggionObjectChangeBase(string experienceId, string objectId, string objectName, GameObject sceneGameObject)
        {
            this.experienceId = experienceId;
            this.objectId = objectId;
            this.objectName = objectName;
            this.sceneGameObject = sceneGameObject;
        }
        
        internal abstract string OldValueToString();
        
        internal abstract string NewValueToString();

        internal abstract string UIStringName();

        internal void DiscardChange(Scene scene, Experience experience, AuggioObject oldData)
        {
            DiscardChangeInternal(scene, experience, oldData);
            EditorSceneManager.MarkSceneDirty(scene);
        }

        protected abstract void DiscardChangeInternal(Scene scene, Experience experience, AuggioObject oldData);

        internal abstract void ApplyToExperience(Experience cachedExperience);
        
        internal string ObjectName
        {
            get => objectName;
            set => objectName = value;
        }

        internal string ExperienceId
        {
            get => experienceId;
            set => experienceId = value;
        }

        internal string ObjectId
        {
            get => objectId;
            set => objectId = value;
        }

        internal GameObject SceneGameObject
        {
            get => sceneGameObject;
            set => sceneGameObject = value;
        }
    }
}
#endif
