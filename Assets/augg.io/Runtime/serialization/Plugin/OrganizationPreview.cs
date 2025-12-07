using System;
using UnityEngine;

namespace Auggio.Utils.Serialization.Plugin
{
    [Serializable]
    public class OrganizationPreview
    {
        [SerializeField] private string id;
        [SerializeField] private string name;
        [SerializeField] private bool serviceAccountSet;
        [SerializeField] private OrganizationTierPreview tier;

        public string Id
        {
            get => id;
            set => id = value;
        }

        public string Name
        {
            get => name;
            set => name = value;
        }

        public bool ServiceAccountSet
        {
            get => serviceAccountSet;
            set => serviceAccountSet = value;
        }

        public OrganizationTierPreview Tier
        {
            get => tier;
            set => tier = value;
        }
    }
}
