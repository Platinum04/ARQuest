using System.IO;
using UnityEngine;

namespace Auggio.Utils.Serialization.Model
{
    [System.Serializable]
    public class SerializedVector3 : ISerializableObject
    {
        [SerializeField] private float x;
        [SerializeField] private float y;
        [SerializeField] private float z;

        public SerializedVector3(Vector3 vector3)
        {
            x = vector3.x;
            y = vector3.y;
            z = vector3.z;
        }

        public SerializedVector3()
        {
        }

        public float X
        {
            get => x;
            set => x = value;
        }

        public float Y
        {
            get => y;
            set => y = value;
        }

        public float Z
        {
            get => z;
            set => z = value;
        }

        public Vector3 Deserialize()
        {
            return new Vector3(x, y, z);
        }

        // Serialize the object to a binary stream
        public void Serialize(BinaryWriter writer)
        {
            writer.Write(x);
            writer.Write(y);
            writer.Write(z);
        }

        // Deserialize the object from a binary stream
        public void Deserialize(BinaryReader reader)
        {
            x = reader.ReadSingle();
            y = reader.ReadSingle();
            z = reader.ReadSingle();
        }
        
        public static implicit operator Vector3(SerializedVector3 rValue) {
            return new Vector3(rValue.x, rValue.y, rValue.z);
        }
    }
}
