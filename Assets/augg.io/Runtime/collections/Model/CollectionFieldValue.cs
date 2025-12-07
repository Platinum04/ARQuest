using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Auggio
{
    
    [Serializable]
    public class CollectionFieldValue
    {
        [SerializeField] private bool reference;
        [SerializeField] private string fieldId;
        [SerializeField] private string fieldName;
        [SerializeField] private List<string> values;

        public bool Reference {
            get => reference;
            set => reference = value;
        }

        public string FieldName {
            get => fieldName;
            set => fieldName = value;
        }

        public string FieldId {
            get => fieldId;
            set => fieldId = value;
        }

        public List<string> Values {
            get => values;
            set => values = value;
        }
    }
}
