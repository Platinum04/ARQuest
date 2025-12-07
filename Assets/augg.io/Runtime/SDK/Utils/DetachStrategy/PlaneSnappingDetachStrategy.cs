using System;
using System.Collections;
using System.Collections.Generic;
using Auggio.Plugin.SDK.Model;
using Auggio.Utils.SDK.Utils;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace Auggio.Plugin.SDK.Utils.DetachStrategy
{
    [RequireComponent(typeof(AuggioObjectTracker))]
    [AddComponentMenu("augg.io/Object Detach Strategies/Plane Snapping Strategy")]
    public class PlaneSnappingDetachStrategy : AbstractDetachStrategy
    {
        // Custom enum for expected plane orientation
        public enum PlaneOrientation
        {
            HorizontalUp,
            HorizontalDown,
            Vertical,
            Any
        }
        
        public enum Axis
        {
            X,
            Y,
            Z,
            X_Negative,
            Y_Negative,
            Z_Negative
        }
        [SerializeField] private AuggioPlaneFinder planeFinder;
        [SerializeField] private float lerpDuration = 0.3f;
        [SerializeField] private PlaneOrientation expectedPlaneOrientation;
        [SerializeField] private Axis snapAxis = Axis.Y;

        private Dictionary<string, ARPlane> _arPlanes;
        private Coroutine _lerpCoroutine;
        private bool _enabled;

        public override void Initialize()
        {
            _arPlanes = new Dictionary<string, ARPlane>();
            if (ARComponentsProvider.Instance == null)
            {
                throw new Exception("Missing ARComponentProvider instance");
            }

            if (ARComponentsProvider.Instance.PlaneManager == null)
            {
                throw new Exception("Missing Plane Manager");
            }

            if (ARComponentsProvider.Instance.AnchorManager == null)
            {
                throw new Exception("Missing Anchor Manager");
            }

            if (planeFinder == null)
            {
                throw new Exception("Missing Plane Finder");
            }

            ARComponentsProvider.Instance.PlaneManager.planesChanged += PlanesChanged;
        }

        public override void Disable()
        {
            ARComponentsProvider.Instance.PlaneManager.planesChanged -= PlanesChanged;
            _arPlanes = null;
        }

        private void PlanesChanged(ARPlanesChangedEventArgs obj)
        {
            foreach (ARPlane addedPlane in obj.added)
            {
                if (IsViablePlane(addedPlane))
                {
                    _arPlanes.Add(addedPlane.trackableId.ToString(), addedPlane);
                }
            }

            foreach (ARPlane updatedPlane in obj.updated)
            {
                if (_arPlanes.ContainsKey(updatedPlane.trackableId.ToString()))
                {
                    if (IsViablePlane(updatedPlane))
                    {
                        _arPlanes[updatedPlane.trackableId.ToString()] = updatedPlane;
                    }
                    else
                    {
                        // Handle change in alignment (e.g., remove from list)
                        _arPlanes.Remove(updatedPlane.trackableId.ToString());
                    }
                }
                else
                {
                    // Handle case where plane was not in the list before
                    if (IsViablePlane(updatedPlane))
                    {
                        _arPlanes.Add(updatedPlane.trackableId.ToString(), updatedPlane);
                    }
                }
            }

            foreach (ARPlane removedPlane in obj.removed)
            {
                if (_arPlanes.ContainsKey(removedPlane.trackableId.ToString()))
                {
                    _arPlanes.Remove(removedPlane.trackableId.ToString());
                }
            }
        }

        public override void OnUpdate()
        {
            if (_arPlanes == null)
            {
                return;
            }

            // Iterate through each AR plane in the dictionary
            foreach (ARPlane arPlane in _arPlanes.Values)
            {
                if (IsPlaneIntersectingCollider(arPlane))
                {
                    Debug.Log("Intersection with AR Plane: " + arPlane.trackableId);
                    if (_lerpCoroutine == null)
                    {
                        _lerpCoroutine = StartCoroutine(SmoothlyLerpToPlane(arPlane, () =>
                        {
                            AttachToAnchor(arPlane);
                            Disable();
                        }));
                    }
                }
            }
        }

        private bool IsPlaneIntersectingCollider(ARPlane plane)
        {
            MeshCollider planeMeshCollider = plane.GetComponent<MeshCollider>();

            if (planeMeshCollider == null)
            {
                Debug.LogWarning("ARPlane is missing a MeshCollider component.");
                return false;
            }

            return planeMeshCollider.bounds.Intersects(planeFinder.Collider.bounds);
        }

        // Coroutine to smoothly lerp the game object's transform to the target plane
        private IEnumerator SmoothlyLerpToPlane(ARPlane plane, Action onLerpFinished)
        {
            float elapsedTime = 0f;

            Vector3 initialPosition = transform.position;
            Quaternion initialRotation = transform.rotation;

            Vector3 planeNormal = plane.transform.up;
            Quaternion targetRotation = Quaternion.FromToRotation(initialRotation * GetSnapAxis(), planeNormal) * initialRotation;

            float distanceFromPlane = Vector3.Dot(transform.position - plane.transform.position, planeNormal);
            Vector3 targetPosition = transform.position - distanceFromPlane * planeNormal;
            // Lerp over the duration, using smooth step interpolation
            while (elapsedTime < lerpDuration)
            {
                float t = elapsedTime / lerpDuration;
                t = t * t * (3f - 2f * t); // Smooth step interpolation (ease in ease out)

                transform.position = Vector3.Lerp(initialPosition, targetPosition, t);
                transform.rotation = Quaternion.Slerp(initialRotation, targetRotation, t);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // Set the final position and attach the object to the plane's anchor
            transform.position = targetPosition;
            onLerpFinished?.Invoke();
        }

        private Vector3 GetSnapAxis()
        {
            switch (snapAxis)
            {
                case Axis.X:
                    return Vector3.right;
                case Axis.Y:
                    return Vector3.up;
                case Axis.Z:
                    return Vector3.forward;
                case Axis.X_Negative:
                    return Vector3.left;
                case Axis.Y_Negative:
                    return Vector3.down;
                case Axis.Z_Negative:
                    return Vector3.back;
            }
            throw new Exception("Cannot find vector for given snap axis");
        }

        private void AttachToAnchor(ARPlane plane)
        {
            Pose pose = new Pose(transform.position, transform.rotation);
            ARAnchor anchor = ARComponentsProvider.Instance.AnchorManager.AttachAnchor(plane, pose);

            // Attach the anchor's transform to the GameObject
            transform.SetParent(anchor.transform);

            enabled = false;
        }

        private bool IsViablePlane(ARPlane plane)
        {
            return IsAuggioPlane(plane) && IsExpectedOrientation(plane);
        }

        // Check if the plane's alignment matches the expected orientation
        private bool IsExpectedOrientation(ARPlane plane)
        {
            switch (expectedPlaneOrientation)
            {
                case PlaneOrientation.HorizontalUp:
                    return plane.alignment == PlaneAlignment.HorizontalUp;
                case PlaneOrientation.HorizontalDown:
                    return plane.alignment == PlaneAlignment.HorizontalDown;
                case PlaneOrientation.Vertical:
                    return plane.alignment == PlaneAlignment.Vertical;
                case PlaneOrientation.Any:
                    return true;
                default:
                    return false;
            }
        }

        private bool IsAuggioPlane(ARPlane arPlane)
        {
            return arPlane.GetComponent<AuggioPlane>() != null;
        }

        public AuggioPlaneFinder PlaneFinder
        {
            get => planeFinder;
            set => planeFinder = value;
        }

        public PlaneOrientation ExpectedPlaneOrientation => expectedPlaneOrientation;

        public Axis SnapAxis => snapAxis;
    }
}