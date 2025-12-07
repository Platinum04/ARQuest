namespace Auggio.Plugin.SDK.Model.Ids {
    public class ExperienceId
    {
        private string id;

        public string ID => id;

        public ExperienceId(string id) {
            this.id = id;
        }

        protected bool Equals(ExperienceId other) {
            return id == other.id;
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ExperienceId) obj);
        }

        public override int GetHashCode() {
            return (id != null ? id.GetHashCode() : 0);
        }
        
        public static implicit operator ExperienceId(string value) {
            return new ExperienceId(value);
        }
        
        public static implicit operator string(ExperienceId value) {
            return value.id;
        }
    }
}
