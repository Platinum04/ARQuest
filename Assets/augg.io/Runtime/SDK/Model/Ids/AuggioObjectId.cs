namespace Auggio.Plugin.SDK.Model.Ids {
    public class AuggioObjectId
    {
        private string id;

        public string ID => id;

        public AuggioObjectId(string id) {
            this.id = id;
        }

        protected bool Equals(AuggioObjectId other) {
            return id == other.id;
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((AuggioObjectId) obj);
        }

        public override int GetHashCode() {
            return (id != null ? id.GetHashCode() : 0);
        }
        
        public static implicit operator AuggioObjectId(string value) {
            return new AuggioObjectId(value);
        }
        
        public static implicit operator string(AuggioObjectId value) {
            return value.id;
        }
    }
}
