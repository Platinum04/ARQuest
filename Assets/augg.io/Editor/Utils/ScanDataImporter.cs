using Auggio.Utils.Serialization.Model;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

#if UNITY_EDITOR
namespace Auggio.Plugin.Editor.Utils
{
    public class ScanDataImporter
    {
        public void Import(MeshWithAnchor meshWithAnchor, Transform parentTransform, Material material = null)
        {
            foreach (SerializedMesh serializedMesh in meshWithAnchor.Meshes.Meshes)
            {
                Mesh newMesh = new Mesh();
                newMesh.indexFormat = IndexFormat.UInt32;
                newMesh.vertices = serializedMesh.GetDeserializedVerticesAsArray(parentTransform);
                newMesh.triangles = serializedMesh.Triangles;
                newMesh.normals = serializedMesh.GetDeserializedNormalsAsArray(parentTransform);
                newMesh.colors = serializedMesh.GetDeserializedColorAsArray();

                GameObject gameObject = new GameObject("mesh_" + meshWithAnchor.Anchor.AnchorId);
                gameObject.tag = "EditorOnly";
                gameObject.transform.parent = parentTransform;
                gameObject.AddComponent<MeshFilter>().mesh = newMesh;
                gameObject.AddComponent<MeshRenderer>();
                gameObject.GetComponent<MeshRenderer>().material =
                    material != null ? material : new Material(Shader.Find("Universal Render Pipeline/Lit"));
                
                SceneVisibilityManager.instance.DisablePicking(gameObject, true);
            }
        }
    }
}
#endif