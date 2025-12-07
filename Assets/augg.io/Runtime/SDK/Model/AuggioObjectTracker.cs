using System;
using System.Collections.Generic;
using Auggio.Plugin.SDK.Runtime;
using Auggio.Plugin.SDK.Utils;
using Auggio.Plugin.SDK.Utils.DetachStrategy;
using Google.XR.ARCoreExtensions;
using UnityEngine;

namespace Auggio.Plugin.SDK.Model
{
    [AddComponentMenu("augg.io/Objects/Auggio Object Tracker")]
    public class AuggioObjectTracker : MonoBehaviour
    {
        
        public delegate void OnTrackingStartedCallback();
        public OnTrackingStartedCallback OnTrackingStarted;

        [HideInInspector] [SerializeField] private string organizationId;
        [HideInInspector] [SerializeField] private string experienceId;
        [HideInInspector] [SerializeField] private string anchorId;
        [HideInInspector] [SerializeField] private string objectId;
        [HideInInspector] [SerializeField] private List<AuggioObjectPlaceholderModel> models;
        [HideInInspector] [SerializeField] private string objectName;
        [HideInInspector] [SerializeField] private bool createByCodeDrivenResolving;
        
        [SerializeField] private bool visualizeMesh = true; 
        
        
        private Transform initialParent;
        private bool initialized = false;
        private bool tracked = false;
        private AbstractDetachStrategy detachStrategy;
        

        #region accessors

        public bool CreateByCodeDrivenResolving {
            get => createByCodeDrivenResolving;
            set => createByCodeDrivenResolving = value;
        }

        public string ExperienceId
        {
            get => experienceId;
            set => experienceId = value;
        }

        public string OrganizationId
        {
            get => organizationId;
            set => organizationId = value;
        }

        public string AnchorId
        {
            get => anchorId;
            set => anchorId = value;
        }

        public string ObjectId
        {
            get => objectId;
            set => objectId = value;
        }

        public string ObjectName
        {
            get => objectName;
            set =>objectName = value;
        }

        public bool VisualizeMesh => visualizeMesh;

        public List<AuggioObjectPlaceholderModel> Models
        {
            get
            {
                return models;
            }
            set => models = value;
        }

        public bool Tracked => tracked;

        #endregion

        internal void HandleTrackerActivation(string trackedAnchorId, ARCloudAnchor anchor, AuggioObjectPose pose) {
            if (tracked) return;
            if (!initialized) {
                Initialize();
            }
            if (!gameObject.activeSelf) {
                if (trackedAnchorId.Equals(anchorId)) {
                    tracked = true;
                    detachStrategy?.Initialize();
                    gameObject.SetActive(true);
                    transform.parent = anchor.transform;
                    transform.localPosition = pose.Position;
                    transform.localRotation = Quaternion.Euler(pose.Rotation);
                    transform.localScale = pose.Scale;
                    OnTrackingStarted?.Invoke();
                }
                
            }
        }

        private void Update() {
            if (tracked) {
                detachStrategy?.OnUpdate();
            }
        }

        internal void Initialize() {
            initialParent = transform.parent;
            initialized = true;
            detachStrategy = GetComponent<AbstractDetachStrategy>();
        }

        internal void DisableTracking() {
            gameObject?.SetActive(false);
            detachStrategy?.Disable();
            tracked = false;
            if (initialParent != null) {
                transform.parent = initialParent;
            }
        }
        
        private void Awake() {
            AuggioTrackingManager auggioTrackingManager = AuggioTrackingManager.Instance;
            if (auggioTrackingManager == null || !auggioTrackingManager.Initialized) {
                gameObject.SetActive(false);
            }
        }
        
        #if UNITY_EDITOR
        private void OnValidate()
        {
            if (string.IsNullOrEmpty(objectName))
            {
                return;
            }
            gameObject.name = AuggioUtils.GetGameObjectName(objectName);
        }

#endif
    }
}