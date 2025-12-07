using System;
using Auggio.Utils.Serialization.Model;
using UnityEngine;

namespace Auggio.Utils.Serialization.Plugin.Experience
{
    [Serializable]
    public class SingleAnchor 
    {
        [SerializeField] private string auggioId;
        [SerializeField] private string name;
        [SerializeField] private SerializedVector3 relativeOriginPosition;
        [SerializeField] private SerializedVector3 relativeOriginRotation;
        [SerializeField] private string meshHash;
        [SerializeField] private string googleAnchorId;
        [SerializeField] private bool reviewed;
        [SerializeField] private bool corrupted;
        [SerializeField] private long expireTime;

        public string GoogleAnchorId {
            get => googleAnchorId;
            set => googleAnchorId = value;
        }

        public string AuggioId
        {
            get => auggioId;
            set => auggioId = value;
        }

        public string Name
        {
            get => name;
            set => name = value;
        }

        public SerializedVector3 RelativeOriginPosition
        {
            get => relativeOriginPosition;
            set => relativeOriginPosition = value;
        }

        public SerializedVector3 RelativeOriginRotation
        {
            get => relativeOriginRotation;
            set => relativeOriginRotation = value;
        }

        public string MeshHash
        {
            get => meshHash;
            set => meshHash = value;
        }

        public bool Reviewed
        {
            get => reviewed;
            set => reviewed = value;
        }

        public bool Corrupted
        {
            get => corrupted;
            set => corrupted = value;
        }

        public long ExpireTime
        {
            get => expireTime;
            set => expireTime = value;
        }
    }
}
