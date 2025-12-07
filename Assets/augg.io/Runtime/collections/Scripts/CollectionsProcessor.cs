using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Auggio.Collections.Scripts
{
    internal class CollectionsProcessor
    {

        //TODO make async
        public Dictionary<string, Collection> ProcessReferences(List<Collection> collections) {
            Dictionary<string, Collection> collectionDictionary = new Dictionary<string, Collection>();
            Dictionary<string, Collection> _colectionIdDictionary = new Dictionary<string, Collection>();
            foreach (Collection collection in collections) {
                collection.Initialize();
                collectionDictionary.Add(collection.Name, collection);
                _colectionIdDictionary.Add(collection.ID, collection);
            }

            foreach (KeyValuePair<string, Collection> valuePair in _colectionIdDictionary) {
                Collection collection = valuePair.Value;
                collection.MapReferences(_colectionIdDictionary);
            }
            
            
            return collectionDictionary;
        }
    }
}
