using System.IO;

namespace Auggio.Utils.Serialization.Model
{
    public interface ISerializableObject
    {
        public void Serialize(BinaryWriter writer);

        public void Deserialize(BinaryReader reader);

    }
}
