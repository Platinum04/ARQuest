using System;
using System.IO;
using UnityEngine;

namespace Auggio.Utils.Serialization.Plugin.Experience
{
    [Serializable]
    public class MeshInfo
    {

        [SerializeField] private string organizationId;
        [SerializeField] private string locationId;
        [SerializeField] private string anchorId;
        [SerializeField] private string meshHash;

        public MeshInfo()
        {
        }

        public MeshInfo(string organizationId, string locationId, string anchorId, string meshHash)
        {
            this.organizationId = organizationId;
            this.locationId = locationId;
            this.anchorId = anchorId;
            this.meshHash = meshHash;
        }

        public string OrganizationId => organizationId;

        public string LocationId => locationId;

        public string AnchorId => anchorId;

        public string MeshHash => meshHash;

        public string GetLocalPath()
        {
            return organizationId + "/" + locationId + "/" + anchorId + "/" + meshHash;
        }

        public string GetFolderPath()
        {
            return Path.Combine(organizationId, locationId, anchorId);
        }
        
    }
}
