using System;
using System.Collections;
using System.Collections.Generic;
using Auggio.Collections.Model;
using Auggio.Collections.Utils;
using UnityEngine;

namespace Auggio
{
    
    [Serializable]
    public class CollectionRow 
    {
        [SerializeField] private string id;
        [SerializeField] private string collectionId;
        [SerializeField] private List<CollectionFieldValue> values;

        private Dictionary<string, CollectionFieldValue> valuesDictionary;

        public string ID {
            get => id;
            set => id = value;
        }

        public string CollectionId
        {
            get => collectionId;
            set => collectionId = value;
        }

        public List<CollectionFieldValue> Values {
            get => values;
            set => values = value;
        }

        public List<string> GetValueByFieldName(string name) {
            if (valuesDictionary.TryGetValue(name, out CollectionFieldValue value)) {
                return value.Values;
            }
            return new List<string>();
        }

        public void Initialize() {
            if (values == null || values.Count == 0) return;
            valuesDictionary = new Dictionary<string, CollectionFieldValue>();
            foreach (CollectionFieldValue value in values) {
                valuesDictionary.Add(value.FieldName, value);
            }
        }

        public void MapReferences(Dictionary<string, Collection> collectionDictionary) {
            if (values == null || values.Count == 0) return;
            foreach (KeyValuePair<string, CollectionFieldValue> keyValuePair in valuesDictionary) {
                CollectionFieldValue fieldValue = keyValuePair.Value;
                if (fieldValue.Reference) {
                    List<string> actualValue = new List<string>();
                    for (int i = 0; i < fieldValue.Values.Count; i++) {
                        string referenceValue = fieldValue.Values[i];
                        actualValue.AddRange(ReferenceHelper.ProcessFieldValue(collectionDictionary, referenceValue));
                    }
                    fieldValue.Values = actualValue;
                    fieldValue.Reference = false;
                }
            }
        }

        
    }
}
