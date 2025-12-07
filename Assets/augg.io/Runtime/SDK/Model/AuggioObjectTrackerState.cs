using Auggio.Plugin.SDK.Model;
using UnityEngine;

namespace Auggio.Runtime.SDK.Model
{
    internal class AuggioObjectTrackerState {

        private AuggioObjectTracker tracker;
        private ResolveState state = ResolveState.INITIALIZED;

        public AuggioObjectTracker Tracker {
            get => tracker;
            set => tracker = value;
        }

        public ResolveState State {
            get => state;
            set => state = value;
        }


        public enum ResolveState {
            INITIALIZED,
            FETCHING,
            RESOLVING,
            RESOLVED,
            ERROR
        }
    }
}
