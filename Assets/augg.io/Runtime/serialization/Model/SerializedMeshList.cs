using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Auggio.Utils.Serialization.Model
{
    [System.Serializable]
    public class SerializedMeshList : ISerializableObject
    {
        [SerializeField] private List<SerializedMesh> meshes;

        public List<SerializedMesh> Meshes
        {
            get => meshes;
            set => meshes = value;
        }

        public void AddMesh(SerializedMesh mesh)
        {
            if (meshes == null)
            {
                meshes = new List<SerializedMesh>();
            }
                
            meshes.Add(mesh);
        }

        // Serialize the object to a binary stream
        public void Serialize(BinaryWriter writer)
        {
            if (meshes != null)
            {
                // Serialize the number of meshes in the list
                writer.Write(meshes.Count);

                // Serialize each SerializedMesh in the list
                foreach (SerializedMesh mesh in meshes)
                {
                    mesh.Serialize(writer);
                }
            }
            else
            {
                writer.Write(0);
            }
        }

        // Deserialize the object from a binary stream
        public void Deserialize(BinaryReader reader)
        {
            // Deserialize the number of meshes in the list
            int meshCount = reader.ReadInt32();

            // Deserialize each SerializedMesh in the list
            meshes = new List<SerializedMesh>(meshCount);
            for (int i = 0; i < meshCount; i++)
            {
                SerializedMesh mesh = new SerializedMesh();
                mesh.Deserialize(reader);
                meshes.Add(mesh);
            }
        }
    }
}
