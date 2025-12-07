using System.IO;
using UnityEngine;

namespace Auggio.Utils.Serialization.Model
{
    [System.Serializable]
    public class SerializedColor : ISerializableObject
    {
        [SerializeField] private float r;
        [SerializeField] private float g;
        [SerializeField] private float b;
        [SerializeField] private float a;

        public SerializedColor()
        {
        }

        public SerializedColor(Color color)
        {
            r = color.r;
            g = color.g;
            b = color.b;
            a = color.a;
        }

        public float R
        {
            get => r;
            set => r = value;
        }

        public float G
        {
            get => g;
            set => g = value;
        }

        public float B
        {
            get => b;
            set => b = value;
        }

        public float A
        {
            get => a;
            set => a = value;
        }

        public Color Deserialize()
        {
            return new Color(r, g, b, a);
        }

        // Serialize the object to a binary stream
        public void Serialize(BinaryWriter writer)
        {
            writer.Write(r);
            writer.Write(g);
            writer.Write(b);
            writer.Write(a);
        }

        // Deserialize the object from a binary stream
        public void Deserialize(BinaryReader reader)
        {
            r = reader.ReadSingle();
            g = reader.ReadSingle();
            b = reader.ReadSingle();
            a = reader.ReadSingle();
        }
    }
}
