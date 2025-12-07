using System;
using System.Collections.Generic;
using augg.io.Serialization.Plugin.Workspace;
using Auggio.Utils.Serialization.Plugin.Experience;
using UnityEngine;

namespace Auggio.Plugin.Editor.Http
{
    [Serializable]
    internal class PluginWorkspacesResponse
    {
        [SerializeField] private List<WorkspacePreview> workspaces;

        public List<WorkspacePreview> Workspaces
        {
            get => workspaces;
            set => workspaces = value;
        }
    }
}
