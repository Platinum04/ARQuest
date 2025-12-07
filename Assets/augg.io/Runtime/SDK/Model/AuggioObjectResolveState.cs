using System;

namespace Auggio.Runtime.SDK.Model
{
    internal class AuggioObjectResolveState {
        
        private string experienceId;
        private string objectId;
        private ResolveState state = ResolveState.INITIALIZED; 

        public AuggioObjectResolveState(string experienceId, string objectId) {
            this.experienceId = experienceId;
            this.objectId = objectId;
        }

        internal string ExperienceId {
            get => experienceId;
            set => experienceId = value;
        }

        internal string ObjectId {
            get => objectId;
            set => objectId = value;
        }

        internal ResolveState State {
            get => state;
            set => state = value;
        }

        protected bool Equals(AuggioObjectResolveState other) {
            return experienceId == other.experienceId && objectId == other.objectId;
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((AuggioObjectResolveState) obj);
        }

        public override int GetHashCode() {
            return HashCode.Combine(experienceId, objectId);
        }


        public enum ResolveState {
            INITIALIZED,
            RESOLVING,
            RESOLVED,
            ERROR
        }
    }
}
