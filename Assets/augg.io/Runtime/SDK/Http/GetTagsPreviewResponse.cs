using System;
using System.Collections.Generic;
using Auggio.Runtime.serialization.Plugin.Experience;
using UnityEngine;

namespace Auggio.Runtime.SDK.Http
{
    
    [Serializable]
    public class GetTagsPreviewResponse
    {
        
        
        [SerializeField] private List<TagPreview> tags;

        
        public List<TagPreview> Tags
        {
            get => tags;
            set => tags = value;
        }
    }
}
