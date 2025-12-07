using System;
using System.IO;
using UnityEngine;

namespace Auggio.Utils.Serialization.Model
{
    [Serializable]
    public class MeshWithAnchor : ISerializableObject
    {
        [SerializeField] private SerializedMeshList meshList;
        [SerializeField] private SerializedAnchor anchor;
        [SerializeField] private AnchorType anchorType;

        public MeshWithAnchor(SerializedMeshList meshList, SerializedAnchor anchor)
        {
            this.meshList = meshList;
            this.anchor = anchor;
            this.anchorType = AnchorType.CLOUD_ANCHOR;
        }

        public MeshWithAnchor()
        {
        }

        public SerializedMeshList Meshes
        {
            get => meshList;
            set => meshList = value;
        }

        public SerializedAnchor Anchor
        {
            get => anchor;
            set => anchor = value;
        }

        public AnchorType AnchorType
        {
            get => anchorType;
            set => anchorType = value;
        }

        // Serialize the object to a binary stream
        public void Serialize(BinaryWriter writer)
        {
            // Serialize the mesh list
            meshList?.Serialize(writer);
            // Serialize the anchor
            anchor?.Serialize(writer);
            // Serialize the anchor type (assuming it's an enum)
            writer.Write((int)anchorType);
        }

        // Deserialize the object from a binary stream
        public void Deserialize(BinaryReader reader)
        {
            // Deserialize the mesh list
            meshList = new SerializedMeshList();
            meshList.Deserialize(reader);

            // Deserialize the anchor
            anchor = new SerializedAnchor();
            anchor.Deserialize(reader);

            // Deserialize the anchor type (assuming it's an enum)
            anchorType = (AnchorType)reader.ReadInt32();
        }
    }
}