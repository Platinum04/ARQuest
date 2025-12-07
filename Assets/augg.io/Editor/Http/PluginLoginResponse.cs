using System;
using System.Collections.Generic;
using Auggio.Utils.Serialization.Plugin;
using UnityEngine;

namespace Auggio.Plugin.Editor.Http
{
    [Serializable]
    internal class PluginLoginResponse
    {
        [SerializeField] private List<OrganizationPreview> allOrganizations;
        [SerializeField] private UserPreview user;

        public List<OrganizationPreview> AllOrganizations
        {
            get => allOrganizations;
            set => allOrganizations = value;
        }

        public UserPreview User
        {
            get => user;
            set => user = value;
        }
    }
}
