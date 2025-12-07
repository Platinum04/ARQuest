#if UNITY_EDITOR
using Auggio.Utils.Serialization.Plugin;
using Auggio.Utils.Serialization.Plugin.Experience;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Auggio.Plugin.Editor.Model.Changes
{
    internal abstract class AuggioObjectChange<C, T> : AuggionObjectChangeBase where C : MonoBehaviour
    {
        protected C sceneComponent;
        protected T oldValue;
        protected T newValue;

        internal AuggioObjectChange(string experienceId, string objectId, C sceneComponent, string objectName, T oldValue, T newValue) : base(experienceId, objectId, objectName, sceneComponent != null? sceneComponent.gameObject : null)
        {
            this.sceneComponent = sceneComponent;
            this.oldValue = oldValue;
            this.newValue = newValue;
        }

        protected override void DiscardChangeInternal(Scene scene, Experience experience, AuggioObject oldData)
        {
            if (sceneComponent != null)
            {
                //object is present in scene
                DiscardChange(sceneComponent, scene, experience, oldData);
                if (sceneComponent != null)
                {
                    EditorUtility.SetDirty(sceneComponent.gameObject);
                }
            }
            else
            {
                //object is not present in scene - e.g remove change
                C t = DiscardChange(null, scene, experience, oldData);
                if (t != null)
                {
                    EditorUtility.SetDirty(t.gameObject);
                }
            }
        }

        protected abstract C DiscardChange(C component, Scene scene, Experience experience,
            AuggioObject oldData);
        
        internal C SceneComponent
        {
            get => sceneComponent;
            set => sceneComponent = value;
        }

        internal T OldValue
        {
            get => oldValue;
            set => oldValue = value;
        }

        internal T NewValue
        {
            get => newValue;
            set => newValue = value;
        }

    }
}
#endif
