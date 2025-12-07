using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Auggio
{
    
    [Serializable]
    public class Collection
    {
        [SerializeField] private string id;
        [SerializeField] private string name;
        [SerializeField] private string applicationId;
        [SerializeField] private List<CollectionRow> rows;
        private Dictionary<string, CollectionRow> rowsDictionary;

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

        public List<CollectionRow> Rows {
            get => rows;
            set => rows = value;
        }

        public CollectionRow GetRowById(string rowId) {
            if (rowsDictionary.TryGetValue(rowId, out CollectionRow row)) {
                return row;
            }
            return null;
        }

        public void Initialize() {
            if (rows == null || rows.Count == 0) return;
            rowsDictionary = new Dictionary<string, CollectionRow>();
            foreach (CollectionRow collectionRow in rows) {
                collectionRow.Initialize();
                rowsDictionary.Add(collectionRow.ID, collectionRow);
            }
        }

        public void MapReferences(Dictionary<string, Collection> collectionDictionary) {
            if (rows == null || rows.Count == 0) return;
            foreach (CollectionRow row in rows) {
                row.MapReferences(collectionDictionary);
            }
        }
    }
}
