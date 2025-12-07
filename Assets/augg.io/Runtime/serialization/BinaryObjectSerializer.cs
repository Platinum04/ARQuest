using System.IO;
using Auggio.Utils.Serialization.Model;
using JetBrains.Annotations;

namespace Auggio.Utils.Serialization
{
    public class BinaryObjectSerializer<T> where T : ISerializableObject
    {
        public byte[] Serialize(T obj)
        {
            using MemoryStream memoryStream = new MemoryStream();
            using BinaryWriter writer = new BinaryWriter(memoryStream);
            obj.Serialize(writer);
            return memoryStream.ToArray();
        }

        [CanBeNull]
        public void Deserialize(byte[] array, T intoObject)
        {
            using MemoryStream memoryStream = new MemoryStream(array);
            using BinaryReader reader = new BinaryReader(memoryStream);
            intoObject.Deserialize(reader);
        }
    }
}
