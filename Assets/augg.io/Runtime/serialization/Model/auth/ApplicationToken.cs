using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Auggio {
    [Serializable]
    public class ApplicationToken {

        [SerializeField] private string applicationId;
        [SerializeField] private string token;

        public string ApplicationId => applicationId;

        public string Token => token;
    }
}
