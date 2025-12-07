using System;
using System.Collections.Generic;
using UnityEngine;

namespace Auggio.Collections.Http
{
    [Serializable]
    public class GetCollectionsResponse {

        [SerializeField] private List<CollectionDetail> collections;

        public List<CollectionDetail> Collections => collections;

        public GetCollectionsResponse(List<CollectionDetail> collections) {
            this.collections = collections;
        }
    }
}
