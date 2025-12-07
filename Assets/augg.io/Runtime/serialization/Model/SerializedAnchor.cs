using System;
using System.IO;
using UnityEngine;

namespace Auggio.Utils.Serialization.Model
{
    [Serializable]
    public class SerializedAnchor : ISerializableObject
    {
        [SerializeField] private string anchorId;
        [SerializeField] private string cloudAnchorId; //TODO to ensure backwards compatibility with previous editor plugin
        [SerializeField] private SerializedVector3 position;
        [SerializeField] private SerializedVector3 rotation;

        public SerializedVector3 Position
        {
            get => position;
            set => position = value;
        }

        public SerializedVector3 Rotation
        {
            get => rotation;
            set => rotation = value;
        }

        public string AnchorId
        {
            get => anchorId;
            set => anchorId = value;
        }

        public string CloudAnchorId
        {
            get => cloudAnchorId;
            set => cloudAnchorId = value;
        }

        // Serialize the object to a binary stream
        public void Serialize(BinaryWriter writer)
        {
            writer.Write(anchorId);
            position?.Serialize(writer);
            rotation?.Serialize(writer);
        }

        // Deserialize the object from a binary stream
        public void Deserialize(BinaryReader reader)
        {
            anchorId = reader.ReadString();
            position = new SerializedVector3();
            position.Deserialize(reader);
            rotation = new SerializedVector3();
            rotation.Deserialize(reader);
        }
    }
}
