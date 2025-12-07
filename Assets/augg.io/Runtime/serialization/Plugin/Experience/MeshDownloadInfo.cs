using System;
using UnityEngine;

namespace Auggio.Utils.Serialization.Plugin.Experience
{
    [Serializable]
    public class MeshDownloadInfo
    {
        [SerializeField] private MeshInfo meshInfo;
        [SerializeField] private string url;
        [SerializeField] private long sizeInBytes;

        public MeshInfo MeshInfo
        {
            get => meshInfo;
            set => meshInfo = value;
        }

        public string URL
        {
            get => url;
            set => url = value;
        }

        public long SizeInBytes
        {
            get => sizeInBytes;
            set => sizeInBytes = value;
        }
    }
}
