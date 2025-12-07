using System;
using UnityEngine;

namespace Auggio.Plugin.Editor.Http
{
    [Serializable]
    internal class PluginExperiencesRequest
    {
        [SerializeField] private string organizationId;
        [SerializeField] private string workspaceId;

        public string OrganizationId
        {
            get => organizationId;
            set => organizationId = value;
        }

        public string WorkspaceId
        {
            get => workspaceId;
            set => workspaceId = value;
        }
    }
}
