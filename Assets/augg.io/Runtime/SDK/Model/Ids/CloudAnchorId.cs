using UnityEngine;

namespace Auggio.Plugin.SDK.Model.Ids {
    public class CloudAnchorId
    {
        private string id;

        public string ID => id;

        public CloudAnchorId(string id) {
            this.id = id;
        }

        protected bool Equals(CloudAnchorId other) {
            return id == other.id;
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((CloudAnchorId) obj);
        }

        public override int GetHashCode() {
            return (id != null ? id.GetHashCode() : 0);
        }
        
        public static implicit operator CloudAnchorId(string value) {
            return new CloudAnchorId(value);
        }
        
        public static implicit operator string(CloudAnchorId value) {
            return value.id;
        }
    }
}
