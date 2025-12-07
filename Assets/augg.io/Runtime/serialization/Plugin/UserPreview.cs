using System;
using UnityEngine;

namespace Auggio.Utils.Serialization.Plugin
{
    [Serializable]
    public class UserPreview
    {
        [SerializeField] private String email;

        public string Email
        {
            get => email;
            set => email = value;
        }
    }
}
