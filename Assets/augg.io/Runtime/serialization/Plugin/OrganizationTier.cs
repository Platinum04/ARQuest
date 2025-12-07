using System;
using UnityEngine;

namespace Auggio.Utils.Serialization.Plugin
{
    [Serializable]
    public class OrganizationTierPreview
    {
        [SerializeField] private String id;
        [SerializeField] private String displayName;

        public string Id
        {
            get => id;
            set => id = value;
        }

        public string DisplayName
        {
            get => displayName;
            set => displayName = value;
        }
    }
}
