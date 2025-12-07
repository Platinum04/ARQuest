using System;
using System.Collections.Generic;
using Auggio.Utils.Serialization.Plugin.Experience;
using UnityEngine;

namespace Auggio.Plugin.Editor.Http
{
    [Serializable]
    internal class MeshDownloadRequest
    {
        [SerializeField] private List<MeshInfo> meshInfos;

        public List<MeshInfo> MeshInfos
        {
            get => meshInfos;
            set => meshInfos = value;
        }
    }
}
