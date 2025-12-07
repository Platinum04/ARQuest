using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Auggio
{
    
    [Serializable]
    public class CollectionInfo
    {
        [SerializeField] private string id;
        [SerializeField] private string name;
        [SerializeField] private string applicationId;

        public string ID {
            get => id;
            set => id = value;
        }

        public string Name {
            get => name;
            set => name = value;
        }

        public string ApplicationId {
            get => applicationId;
            set => applicationId = value;
        }

    }
}