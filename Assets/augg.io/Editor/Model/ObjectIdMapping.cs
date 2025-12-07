#if UNITY_EDITOR
using System;
using UnityEngine;

namespace Auggio.Plugin.Editor.Model
{
    [Serializable]
    internal class ObjectIdMapping
    {
        [SerializeField] private string oldId;
        [SerializeField] private string newId;


        public string OldId
        {
            get => oldId;
            set => oldId = value;
        }

        public string NewId
        {
            get => newId;
            set => newId = value;
        }
    }
}
#endif
