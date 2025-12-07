using System;
using UnityEngine;

namespace augg.io.Serialization.Plugin.Workspace {
    
    [Serializable]
    public class WorkspacePreview
    {
        [SerializeField] private string id;
        [SerializeField] private string name;
        [SerializeField] private bool isDebug;
        
        public string ID {
            get => id;
            set => id = value;
        }

        public string Name {
            get => name;
            set => name = value;
        }

        public bool IsDebug {
            get => isDebug;
            set => isDebug = value;
        }
    }
}
