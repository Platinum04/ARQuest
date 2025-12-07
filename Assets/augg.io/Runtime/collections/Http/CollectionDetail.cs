using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Auggio.Collections
{
    
    [Serializable]
    public class CollectionDetail
    {

        [SerializeField] private CollectionInfo collection;

        [SerializeField] private List<CollectionRow> rows;

   
        public CollectionInfo Collection
        {
            get => collection;
            set => collection = value;
        }

        public List<CollectionRow> Rows
        {
            get => rows;
            set => rows = value;
        }
        
        public static implicit operator Collection(CollectionDetail collectionDetail) {
            return new Collection()
            {
                ID = collectionDetail.Collection.ID,
                Name = collectionDetail.Collection.Name,
                ApplicationId = collectionDetail.Collection.ApplicationId,
                Rows = collectionDetail.Rows
            };
        }

       
    }
}
