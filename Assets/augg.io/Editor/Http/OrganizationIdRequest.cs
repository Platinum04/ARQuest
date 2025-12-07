using System;
using UnityEngine;

namespace Auggio.Plugin.Editor.Http
{
    [Serializable]
    internal class OrganizationIdRequest
    {
        [SerializeField] private string organizationId;

        public string OrganizationId
        {
            get => organizationId;
            set => organizationId = value;
        }
    }
}
