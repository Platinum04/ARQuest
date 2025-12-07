namespace Auggio.Plugin.SDK.Model.Ids {
    public class AuggioAnchorId {
        private string id;

        public string ID => id;

        public AuggioAnchorId(string id) {
            this.id = id;
        }

        protected bool Equals(AuggioAnchorId other) {
            return id == other.id;
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((AuggioAnchorId) obj);
        }

        public override int GetHashCode() {
            return (id != null ? id.GetHashCode() : 0);
        }
        
        public static implicit operator AuggioAnchorId(string value) {
            return new AuggioAnchorId(value);
        }
        
        public static implicit operator string(AuggioAnchorId value) {
            return value.id;
        }
    }
}
