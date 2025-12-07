using System;
using Auggio.Utils.Serialization.Plugin;
using UnityEngine;

namespace Auggio.Plugin.Editor.Http
{
    [Serializable]
    internal class SelectOrganizationResponse
    {
        [SerializeField] private OrganizationRole organizationRole;

        public OrganizationRole OrganizationRole
        {
            get => organizationRole;
            set => organizationRole = value;
        }
    }
}
