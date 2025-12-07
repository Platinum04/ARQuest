using UnityEngine;

namespace Auggio.Plugin.SDK.Model
{
    public class VisualizationHierarchy : MonoBehaviour
    {
        [HideInInspector] private string experienceId;
        [Header("Visualization settings")]
        [HideInInspector][SerializeField] private Material material;
        [HideInInspector][SerializeField] private bool visualizeMesh;

        public string ExperienceId
        {
            get => experienceId;
            set => experienceId = value;
        }

        public bool VisualizeMesh
        {
            set => visualizeMesh = value;
        }

        public Material Material
        {
            get => material;
            set => material = value;
        }

    }
}
