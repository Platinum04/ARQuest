using System;
using System.Collections.Generic;
using Auggio.Utils.Serialization.Plugin.Experience;
using UnityEngine;

namespace Auggio.Plugin.Editor.Http
{
    [Serializable]
    internal class MeshDownloadResponse
    {
        [SerializeField] private List<MeshDownloadInfo> downloadInfo;

        public List<MeshDownloadInfo> DownloadInfo
        {
            get => downloadInfo;
            set => downloadInfo = value;
        }
    }
}
