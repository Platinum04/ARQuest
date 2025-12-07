using System;
using System.Collections.Generic;
using UnityEngine;

namespace Auggio.Utils.Serialization.Plugin.Experience
{
    [Serializable]
    public class Location
    {
        [SerializeField] private String id;
        [SerializeField] private String organizationId;
        [SerializeField] private string name;
        [SerializeField] private List<SingleAnchor> singleAnchorList;
        [SerializeField] private bool corrupted;

        public string ID
        {
            get => id;
            set => id = value;
        }

        public string OrganizationId
        {
            get => organizationId;
            set => organizationId = value;
        }

        public string Name
        {
            get => name;
            set => name = value;
        }

        public List<SingleAnchor> SingleAnchorList
        {
            get => singleAnchorList;
            set => singleAnchorList = value;
        }

        public bool Corrupted
        {
            get => corrupted;
            set => corrupted = value;
        }
    }
}
