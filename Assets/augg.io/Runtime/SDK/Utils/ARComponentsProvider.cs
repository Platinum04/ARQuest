
using System;
using Unity.XR.CoreUtils;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace Auggio.Utils.SDK.Utils
{
    
    [AddComponentMenu("augg.io/ARComponentsProvider")]
    public class ARComponentsProvider : MonoBehaviour
    {
#if UNITY_EDITOR


        [CustomEditor(typeof(ARComponentsProvider))]
        public class ARComponentsProviderEditor : UnityEditor.Editor
        {
            public override void OnInspectorGUI()
            {
                serializedObject.Update();
                base.OnInspectorGUI();
                ARComponentsProvider arComponentsProvider = (ARComponentsProvider) target;

                if (GUILayout.Button("Try to automatically fill component references"))
                {
                    TryToFillReferences(arComponentsProvider);
                }
            }

            public static void TryToFillReferences(ARComponentsProvider arComponentsProvider)
            {
                XROrigin[] xrOrigins =
                    FindObjectsByType<XROrigin>(FindObjectsInactive.Include, FindObjectsSortMode.None);
                if (xrOrigins.Length == 0)
                {
                    throw new Exception("XR Origin is missing in current scene");
                }

                if (xrOrigins.Length > 1)
                {
                    throw new Exception("Cannot automatically fill reference. Multiple XROrigins found in scene.");
                }
                arComponentsProvider.origin = xrOrigins[0];
                arComponentsProvider.arCamera = arComponentsProvider.Origin.GetComponentInChildren<Camera>();
                arComponentsProvider.anchorManager =
                    arComponentsProvider.Origin.GetComponent<ARAnchorManager>();
                arComponentsProvider.planeManager = arComponentsProvider.Origin.GetComponent<ARPlaneManager>();

                EditorUtility.SetDirty(arComponentsProvider);
            }
        }
#endif

        [Header("Mandatory components")]
        [SerializeField] private XROrigin origin;
        [SerializeField] private Camera arCamera;
        [SerializeField] private ARAnchorManager anchorManager;
        
        [Header("Optional components")]
        [SerializeField] private ARPlaneManager planeManager;

        private static ARComponentsProvider _instance;

        public static ARComponentsProvider Instance => _instance;

        public XROrigin Origin => origin;

        public Camera ARCamera => arCamera;

        public ARAnchorManager AnchorManager => anchorManager;

        public ARPlaneManager PlaneManager => planeManager;

        private void Awake()
        {
            _instance = this;
        }

        private void OnDestroy()
        {
            if (_instance != null)
            {
                Destroy(_instance);
                _instance = null;
            }
        }
    }
}