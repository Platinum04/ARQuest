using UnityEngine;

namespace Auggio.Plugin.SDK.Model {
    public class AuggioObjectPose {


        private Vector3 position;
        private Vector3 rotation;
        private Vector3 scale;



        public Vector3 Position => position;

        public Vector3 Rotation => rotation;

        public Vector3 Scale => scale;

        public AuggioObjectPose(Vector3 position, Vector3 rotation, Vector3 scale) {
            this.position = position;
            this.rotation = rotation;
            this.scale = scale;
        }
    }
}
