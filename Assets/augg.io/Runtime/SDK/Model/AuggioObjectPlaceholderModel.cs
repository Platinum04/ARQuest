using Auggio.Plugin.SDK.Model;
using Auggio.Plugin.SDK.Utils;
using UnityEngine;

namespace Auggio.Plugin.SDK.Model
{
    [AddComponentMenu("augg.io/Objects/Auggio Object Placeholder")]
    public class AuggioObjectPlaceholderModel : MonoBehaviour
    {
        [HideInInspector] [SerializeField] private string experienceId;
        [HideInInspector] [SerializeField] private string placeholderId;
        [HideInInspector] [SerializeField] private AuggioObjectTracker auggioObjectTracker;
        [HideInInspector] [SerializeField] private string modelId;
        [HideInInspector] [SerializeField] private Mesh mesh;

        [HideInInspector][SerializeField] private string placeholderName;

        public string ExperienceId
        {
            get => experienceId;
            set => experienceId = value;
        }

        public AuggioObjectTracker AuggioObjectTracker
        {
            get => auggioObjectTracker;
            set => auggioObjectTracker = value;
        }

        public string PlaceholderName
        {
            get => placeholderName;
            set => placeholderName = value;
        }

        public string PlaceholderId
        {
            get => placeholderId;
            set => placeholderId = value;
        }

#if UNITY_EDITOR
        public string ModelId
        {
            get => modelId;
            set
            {
                if (!string.IsNullOrEmpty(value) && PrimitiveTypeHelper.IsPrimitiveModelId(value))
                {
                    GameObject go =
                        GameObject.CreatePrimitive(PrimitiveTypeHelper.GetPrimitiveTypeByModelId(value));
                    mesh = go.GetComponent<MeshFilter>().sharedMesh;
                    DestroyImmediate(go);
                }

                modelId = value;
            }
        }
        
        // This method is called whenever the object is validated, including after duplication
        private void OnValidate()
        {
            if (placeholderName == null)
            {
                return;
            }
            gameObject.name = AuggioUtils.GetGameObjectPlaceholderName(placeholderName);
        }
#endif
        public Mesh Mesh
        {
            get => mesh;
            set => mesh = value;
        }
    }
}
