using System.Collections.Generic;
using Auggio.Utils.Serialization.Model;
using UnityEngine;

namespace Auggio.Plugin.SDK.Utils
{
    public class PrimitiveTypeHelper
    {

        public const string NO_PLACEHOLDER = "<None>";
        
        public const string CUBE_ID = "auggio_cube_primitive";
        public const string SPHERE_ID = "auggio_sphere_primitive";
        public const string CAPSULE_ID = "auggio_capsule_primitive";
        public const string CYLINDER_ID = "auggio_cylinder_primitive";

        public static PrimitiveType GetPrimitiveTypeByModelId(string modelId)
        {
            switch (modelId)
            {
                case CUBE_ID:
                    return PrimitiveType.Cube;
                case SPHERE_ID:
                    return PrimitiveType.Sphere;
                case CAPSULE_ID:
                    return PrimitiveType.Capsule;
                case CYLINDER_ID:
                    return PrimitiveType.Cylinder;
                default:
                    return PrimitiveType.Cube;
            }
        }

        public static bool IsPrimitiveModelId(string modelId)
        {
            return modelId.Equals(CUBE_ID) ||
                   modelId.Equals(SPHERE_ID) ||
                   modelId.Equals(CAPSULE_ID) ||
                   modelId.Equals(CYLINDER_ID);
        }

        public static List<string> GetIds()
        {
            return new List<string> {CUBE_ID, SPHERE_ID, CAPSULE_ID, CYLINDER_ID};
        }
        
    }
}
