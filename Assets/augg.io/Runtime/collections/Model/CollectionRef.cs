using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Auggio.Collections.Model
{
    [Serializable]
    public class CollectionRef {
        [SerializeField] private string _referencedId;
        [SerializeField] private string _referencedRowId;
        [SerializeField] private string _valueName;
        
        public string ReferencedId {
            get => _referencedId;
            set => _referencedId = value;
        }

        public string ReferencedRowId {
            get => _referencedRowId;
            set => _referencedRowId = value;
        }

        public string ValueName {
            get => _valueName;
            set => _valueName = value;
        }
    }
}
