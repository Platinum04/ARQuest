using System.Collections;
using System.Collections.Generic;
using Auggio.Collections.Model;
using UnityEngine;

namespace Auggio.Collections.Utils
{
    internal static class ReferenceHelper
    {
        private static List<string> EMPTY = new List<string>();
        private static List<string> GetReferenceValue(CollectionRef reference, Dictionary<string, Collection> collectionDictionary) {
            if (collectionDictionary.TryGetValue(reference.ReferencedId, out Collection collection)) {
                CollectionRow row = collection.GetRowById(reference.ReferencedRowId);
                if (row == null) {
                    Debug.LogWarning("You are trying to access row id that does not exists. rowId: " + reference.ReferencedRowId);
                    return EMPTY;
                }
                List<string> valueByName = row.GetValueByFieldName(reference.ValueName);
                return valueByName;
            }
            else {
                Debug.LogWarning("You are trying to access collection id that does not exist.");
            }
            return EMPTY;
        }
        
        internal static List<string> ProcessFieldValue(Dictionary<string, Collection> collectionDictionary, string rawValue) {
            CollectionRef collectionRef = JsonUtility.FromJson<CollectionRef>(rawValue);
            if (collectionRef != null) {
                return GetReferenceValue(collectionRef, collectionDictionary);
            }
            else {
                Debug.LogError("Field value reference bool is set to true but the rawValue could not be parsed to reference.");
            }

            return EMPTY;
        }
    }
}
