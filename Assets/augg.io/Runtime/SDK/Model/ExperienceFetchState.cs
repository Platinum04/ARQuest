using System.Collections.Generic;
using Auggio.Plugin.SDK.Model;
using Auggio.Plugin.SDK.Model.Ids;
using Auggio.Utils.Serialization.Plugin.Experience;
using UnityEngine;

namespace Auggio.Runtime.SDK.Model
{
    internal class ExperienceFetchState {
        
        private Dictionary<AuggioObjectId, AuggioObjectTracker> trackers = new Dictionary<AuggioObjectId, AuggioObjectTracker>();
        private Experience experience;
        private FetchState state = FetchState.INITIALIZED;

        public Dictionary<AuggioObjectId, AuggioObjectTracker> Trackers {
            get => trackers;
            set => trackers = value;
        }

        public FetchState State {
            get => state;
            set => state = value;
        }

        public Experience Experience {
            get => experience;
            set => experience = value;
        }

        public enum FetchState {
            INITIALIZED,
            FETCHING,
            FETCHED,
            ERROR
        }
    }
}
