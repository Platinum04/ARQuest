using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Auggio.Utils.Serialization.Model
{
    [System.Serializable]
    public class SerializedMesh : ISerializableObject
    {
        [SerializeField] private List<SerializedVector3> vertices;
        [SerializeField] private int[] triangles;
        [SerializeField] private List<SerializedVector3> normals;
        [SerializeField] private List<SerializedColor> colors;

        public List<SerializedVector3> Vertices
        {
            get => vertices;
            set => vertices = value;
        }

        public int[] Triangles
        {
            get => triangles;
            set => triangles = value;
        }

        public List<SerializedVector3> Normals
        {
            get => normals;
            set => normals = value;
        }

        public List<SerializedColor> Colors
        {
            get => colors;
            set => colors = value;
        }

        public void AddVertex(Vector3 vertex)
        {
            if (vertices == null)
            {
                vertices = new List<SerializedVector3>();
            }
            vertices.Add(new SerializedVector3(vertex));
        }

        public void AddNormal(Vector3 normal)
        {
            if (normals == null)
            {
                normals = new List<SerializedVector3>();
            }
            normals.Add(new SerializedVector3(normal));
        }

        public void AddColor(Color color)
        {
            if (colors == null)
            {
                colors = new List<SerializedColor>();
            }
            colors.Add(new SerializedColor(color));
        }

        public Vector3[] GetDeserializedVerticesAsArray(Transform relativeTo)
        {
            if (vertices == null)
            {
                return null;
            }

            Vector3[] deserializedVertices = new Vector3[vertices.Count];
            int i = 0;
            foreach (SerializedVector3 vertex in vertices)
            {
                deserializedVertices[i] = relativeTo.TransformPoint(vertex.Deserialize());
                i++;
            }

            return deserializedVertices;
        }
    
        public Vector3[] GetDeserializedNormalsAsArray(Transform relativeTo)
        {
            if (normals == null)
            {
                return null;
            }

            Vector3[] deserializedNormals = new Vector3[normals.Count];
            int i = 0;
            foreach (SerializedVector3 normal in normals)
            {
                deserializedNormals[i] = relativeTo.TransformDirection(normal.Deserialize());
                i++;
            }

            return deserializedNormals;
        }

        public Color[] GetDeserializedColorAsArray()
        {
            if (colors == null)
            {
                return null;
            }

            Color[] deserializedColors = new Color[colors.Count];
            int i = 0;
            foreach (SerializedColor color in colors)
            {
                deserializedColors[i] = color.Deserialize();
                i++;
            }

            return deserializedColors;
        }

        // Serialize the object to a binary stream
    public void Serialize(BinaryWriter writer)
    {
        // Serialize vertices
        if (vertices != null)
        {
            writer.Write(vertices.Count);
            foreach (SerializedVector3 vertex in vertices)
            {
                vertex.Serialize(writer);
            }
        }
        else
        {
            writer.Write(0);
        }

        if (triangles != null)
        {
            // Serialize triangles
            writer.Write(triangles.Length);
            foreach (int triangle in triangles)
            {
                writer.Write(triangle);
            }
        }
        else
        {
            writer.Write(0);
        }


        if (normals != null)
        {
            // Serialize normals
            writer.Write(normals.Count);
            foreach (SerializedVector3 normal in normals)
            {
                normal.Serialize(writer);
            }
        }
        else
        {
            writer.Write(0);
        }

        if (colors != null)
        {
            // Serialize colors
            writer.Write(colors.Count);
            foreach (SerializedColor color in colors)
            {
                color.Serialize(writer);
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
        // Deserialize vertices
        int vertexCount = reader.ReadInt32();
        vertices = new List<SerializedVector3>(vertexCount);
        for (int i = 0; i < vertexCount; i++)
        {
            SerializedVector3 vertex = new SerializedVector3();
            vertex.Deserialize(reader);
            vertices.Add(vertex);
        }

        // Deserialize triangles
        int triangleCount = reader.ReadInt32();
        triangles = new int[triangleCount];
        for (int i = 0; i < triangleCount; i++)
        {
            triangles[i] = reader.ReadInt32();
        }

        // Deserialize normals
        int normalCount = reader.ReadInt32();
        normals = new List<SerializedVector3>(normalCount);
        for (int i = 0; i < normalCount; i++)
        {
            SerializedVector3 normal = new SerializedVector3();
            normal.Deserialize(reader);
            normals.Add(normal);
        }

        // Deserialize colors
        int colorCount = reader.ReadInt32();
        colors = new List<SerializedColor>(colorCount);
        for (int i = 0; i < colorCount; i++)
        {
            SerializedColor color = new SerializedColor();
            color.Deserialize(reader);
            colors.Add(color);
        }
    }
    }
}
