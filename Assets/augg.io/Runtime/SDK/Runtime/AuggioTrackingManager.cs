using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Authentication;
using Auggio.Runtime.SDK.Utils.AnchorAfterResolveStrategy;
using Auggio.Plugin.SDK.Http;
using Auggio.Plugin.SDK.Model;
using Auggio.Plugin.SDK.Model.Ids;
using Auggio.Plugin.SDK.Utils;
using Auggio.Runtime.SDK.Model;
using Auggio.Runtime.SDK.Tag;
using Auggio.Utils.Serialization;
using Auggio.Utils.Serialization.Plugin;
using Auggio.Utils.Serialization.Plugin.Experience;
using Google.XR.ARCoreExtensions;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace Auggio.Plugin.SDK.Runtime {
    
    /**
     *  Tracking manager used for experience object tracking.
     */
    [AddComponentMenu("augg.io/AuggioTrackingManager")]
    public class AuggioTrackingManager : MonoBehaviour {

        public delegate void OnAuggioObjectResolvedCallback(AuggioObject auggioObject, Experience experience, Transform parent);

        private static readonly float DEFAULT_WARMUP_TIME = 2f;
        
        public static AuggioTrackingManager Instance;

        private delegate void OnExperienceFetched(string response);
        private delegate void OnExperienceFetchError(ErrorResponse response);

        public List<ARCloudAnchor> TrackedAnchors {
            get {
                if (resolver == null || !initialized) return new List<ARCloudAnchor>();
                return resolver.ResolvedCloudAnchors.Values.ToList();
            }
        }

        public OnAuggioObjectResolvedCallback OnAuggioObjectResolved
        {
            get => onAuggioObjectResolved;
            set => onAuggioObjectResolved = value;
        }

        [SerializeField] private ARAnchorManager anchorManager;
        
        [Tooltip("ARFoundation needs some time to warm up before we can resolve Cloud Anchors.")]
        [SerializeField] private float warmUpTime = DEFAULT_WARMUP_TIME;

        [Tooltip("Null strategy means default behaviour after anchor resolve.")]
        [SerializeField] private AbstractAnchorAfterResolveStrategy anchorAfterResolveStrategy;

        //private AuggioObjectTracker[] sceneTrackers;
        private OnAuggioObjectResolvedCallback onAuggioObjectResolved;

        private AuggioObjectTracker[] trackersCreateByPlugin;

        private Dictionary<ExperienceId, List<AuggioObjectTracker>> sceneTrackers = new Dictionary<ExperienceId, List<AuggioObjectTracker>>();

        private Dictionary<ExperienceId, ExperienceFetchState> trackables = new Dictionary<ExperienceId, ExperienceFetchState>();
        
        private Dictionary<ExperienceId, List<AuggioObjectResolveState>> auggioObjects = new Dictionary<ExperienceId, List<AuggioObjectResolveState>>();
        
        

        /**
         * All valid anchors found in the scene
         */
        //private Dictionary<AuggioAnchorId, CloudAnchorId> sceneAnchors = new Dictionary<AuggioAnchorId, CloudAnchorId>();
        

        private Resolver resolver;

        public bool Initialized => initialized;
        private bool initialized = false;

        //private Coroutine initializeWebRequest;
        private Coroutine initializeCoroutine;
        private List<Coroutine> fetchWebRequests = new List<Coroutine>();

        #region unity lifecycle
        private void Awake() {
            Instance = this;
        }

        private void OnDestroy() {
            if (Instance == this) {
                Instance = null;
            }
        }

        private void OnDisable() {
            ClearState();
            DisableAllTrackers(sceneTrackers);
            TagManager.Instance?.DiscardData();
        }

        #endregion

        /**
         *  Initializes the tracking manager. This has to be called AFTER the ARSession is tracking.
         * 
         */
        public void Initialize() {
            if (AuthFileTokenUtility.IsFileTokenAvailable()) {
                if (warmUpTime == 0) {
                    warmUpTime = DEFAULT_WARMUP_TIME;
                    Debug.LogWarning("Warmup time cannot be zero. Defaulting to " + DEFAULT_WARMUP_TIME + "s");
                }
                initializeCoroutine = StartCoroutine(InitializeInternal());
            }
            else {
                string message = "The project does not contain file token used for authentication. Please go to cms.augg.io and download token for your application.";
                LogProgress(message);
                throw new AuthenticationException(message);
            }
        }

        private IEnumerator InitializeInternal() {
            LogProgress("AuggioTrackingManager Initialization Started");
            yield return new WaitForSeconds(warmUpTime);
            if (!enabled) {
                yield break;
            }
            resolver = new Resolver(this, anchorManager, OnAnchorResolved);
#if !UNITY_EDITOR
            if (ARSession.state != ARSessionState.SessionTracking) {
                LogProgress("ARSession is not tracking.");
                Debug.LogError("ARSession is not tracking. Could not initialize AuggioTrackingManager.");
                yield break;
            }
#endif
#if UNITY_EDITOR && !AUGGIO_DEBUG
            LogProgress("AuggioTrackingManager Initialized Editor");
            yield break;
#endif
            ClearState();
            
            HandleSceneTrackers();
            
            trackables = new Dictionary<ExperienceId, ExperienceFetchState>();
            

            initialized = true;
            
            //call resolve for all scene trackers
            ResolveSceneTrackers();
            
        }

        private void ResolveSceneTrackers() {
            foreach (KeyValuePair<ExperienceId,List<AuggioObjectTracker>> experienceTrackedObjects in sceneTrackers) {
                if (experienceTrackedObjects.Value != null) {
                    foreach (AuggioObjectTracker auggioObjectTracker in experienceTrackedObjects.Value) {
                        Resolve(auggioObjectTracker.ObjectId, auggioObjectTracker.ExperienceId);
                    }
                }
            }
        }

        private void HandleSceneTrackers() {
            trackersCreateByPlugin = FindObjectsOfType<AuggioObjectTracker>(true);
            foreach (AuggioObjectTracker auggioObjectTracker in trackersCreateByPlugin) {
                AddSceneTracker(auggioObjectTracker);
            }
            DisableAllTrackers(sceneTrackers);
        }

        private void AddSceneTracker(AuggioObjectTracker tracker) {
            if (SceneTrackerExists(tracker.ObjectId, tracker.ExperienceId)) return;
            if (sceneTrackers.TryGetValue(tracker.ExperienceId, out List<AuggioObjectTracker> trackers)) {
                trackers.Add(tracker);
            }
            else {
                sceneTrackers.Add(tracker.ExperienceId, new () {tracker});
            }
            tracker.Initialize();
        }
        
        public void Resolve(string objectId, string experienceId) {
            if (!initialized) {
                Debug.LogError("Cannot resolve object before the tracking manager is initialized.");
                return;
            }
            //add the object to the toBeResolved list
            //if object is already resolving or resolved it will return false and we do not continue
            if (!AddObjectToTheQueue(objectId, experienceId)) return;
            
            //check if experience data fetched
            if (trackables.TryGetValue(experienceId, out ExperienceFetchState state)) {
                if (state.State == ExperienceFetchState.FetchState.FETCHED) {
                    RefreshTrackingScene();
                }
            }
            else {
                FetchObjectData(objectId, experienceId);
            }
        }

        private void FetchObjectData(string objectId, string experienceId) {
            ExperienceFetchState experienceFetchState = new ExperienceFetchState();
                
            trackables.Add(experienceId, experienceFetchState);
            
            FetchExperienceData(experienceId, (response) => {
                if (TagManager.Instance != null)
                {
                    TagManager.Instance.FetchTagData(experienceId, (tags) =>
                    {
                        OnExperienceDataFetched(response);
                    });
                }
                else
                {
                    OnExperienceDataFetched(response);
                }
            }, (res) => {
                FetchErrorMessage(res);
                experienceFetchState.State = ExperienceFetchState.FetchState.ERROR;
            });
        }

        private void OnExperienceDataFetched(string response)
        {
            HandleExperienceFetched(response);
            RefreshTrackingScene();
        }
        
        private void HandleExperienceFetched(string response) {
            GetFullExperiencesResponse parsedResponse = JsonUtility.FromJson<GetFullExperiencesResponse>(response);
            foreach (Experience experience in parsedResponse.Experiences) {
                //update fetch status
                //update scene trackers created through plugin flow
                List<AuggioObjectTracker> experienceTrackers = GetSceneTrackers(new ExperienceId(experience.ID));

                foreach (AuggioObjectTracker tracker in experienceTrackers)
                {
                    UpdateSceneTrackerState(tracker, experience);
                }
                
                if (trackables.TryGetValue(experience.ID, out ExperienceFetchState state)) {
                    state.State = ExperienceFetchState.FetchState.FETCHED;
                    state.Experience = experience;
                }
                else {
                    Debug.LogError("There is no experience fetch state for experience id " + experience.ID);
                }
            }
        }
        
        private void UpdateSceneTrackerState(AuggioObjectTracker tracker, Experience experience)
        {
            try
            {
                AuggioObject auggioObject = experience.FindObjectByObjectId(tracker.ObjectId);
                tracker.AnchorId = auggioObject.AssignedAnchor;
                tracker.ObjectName = auggioObject.Name;
            }
            catch (ArgumentException e)
            {
                Debug.LogWarning("The augg.io object with id " + tracker.ObjectId + " does not exist in fetched experience.");
            }
        }

        private void FetchExperienceData(string experienceId, OnExperienceFetched onFetchCompleted, OnExperienceFetchError onExperienceFetchError) {
            LogProgress("Experience id" + experienceId + " data fetch started.");
            string fileToken = AuthFileTokenUtility.GetToken();
            GetFullExperiencesByIdRequest request = new GetFullExperiencesByIdRequest(new () {experienceId});
            Coroutine fetchCoroutine = null;
            fetchCoroutine = StartCoroutine(WebRequestUtility.WebRequestSDK(HttpMethod.Post, "/sdk/experiences", JsonUtility.ToJson(request), fileToken, (response) => {
                LogProgress("Experience data fetch success.");
                onFetchCompleted?.Invoke(response);
                fetchWebRequests.Remove(fetchCoroutine);
            }, (s) => {
                ErrorResponse errorResponse = ErrorResponse.Get(s);
                LogProgress("Experience data fetch error code: " + errorResponse.ErrorCode + " message: " + errorResponse.Message);
                if (trackables.TryGetValue(experienceId, out ExperienceFetchState state)) {
                    state.State = ExperienceFetchState.FetchState.ERROR;
                }
                onExperienceFetchError?.Invoke(errorResponse);
            }));
            
            fetchWebRequests.Add(fetchCoroutine);
            
            if (trackables.TryGetValue(experienceId, out ExperienceFetchState state)) {
                state.State = ExperienceFetchState.FetchState.FETCHING;
            }
        }

        private bool AddObjectToTheQueue(string objectId, string experienceId) {
            AuggioObjectResolveState auggioObjectResolveState = new AuggioObjectResolveState(experienceId, objectId);
            if (auggioObjects.TryGetValue(experienceId, out List<AuggioObjectResolveState> resolveStates)) {
                if (resolveStates.Contains(auggioObjectResolveState)) {
                    Debug.LogWarning("Auggio Object id " + objectId + " is already resolving or resolved.");
                    return false;
                }
                else
                {
                    resolveStates.Add(auggioObjectResolveState);
                }
            }
            else {
                auggioObjects.Add(experienceId, new () {auggioObjectResolveState});
            }

            return true;
        }

        private void FetchErrorMessage(ErrorResponse errorResponse) {
            if (errorResponse.ErrorCode == ErrorPopupCode.AG010) {
                throw new NotSupportedException("This version of the SDK is no longer supported. Please update to a new version.");
            }
            Debug.LogError("Could not fetch experience data from the server. This can be either because you are not connected to the internet or you have some inconsistencies in the scene.");
        }

        private void RefreshTrackingScene() {
            LogProgress("Refreshing Tracking Scene");
            foreach (KeyValuePair<ExperienceId,List<AuggioObjectResolveState>> toBeResolvedExperience in auggioObjects) {
                
                if(!IsExperienceFetched(toBeResolvedExperience.Key)) continue;
                if(!IgnoreTags() && !IsExperienceFetched(toBeResolvedExperience.Key)) continue;
                //TODO are tags for experience fetched
                foreach (AuggioObjectResolveState auggioObjectResolveState in toBeResolvedExperience.Value) {
                    //check if object is not being resolved
                    if(auggioObjectResolveState.State != AuggioObjectResolveState.ResolveState.INITIALIZED) continue;
                    
                    if (GetAuggioObject(auggioObjectResolveState.ObjectId, toBeResolvedExperience.Key, out AuggioObject auggioObject, out ExperienceFetchState fetchState))
                    { 
                        LogProgress("Resolving auggio object " + auggioObject.Name);
                        ResolveInternal(auggioObjectResolveState.ObjectId, toBeResolvedExperience.Key, auggioObject, fetchState.Experience);
                        auggioObjectResolveState.State = AuggioObjectResolveState.ResolveState.RESOLVING;
                    }
                    else
                    {
                        Debug.LogError("Auggio object " + auggioObjectResolveState.ObjectId + " could not be found in experience: " + toBeResolvedExperience.Key);
                    }
                }
                
            }
            
        }
        
        private bool IgnoreTags()
        {
            return TagManager.Instance == null;
        }

        private bool IsExperienceFetched(string experienceId)
        {
            if (trackables.TryGetValue(experienceId, out ExperienceFetchState experienceFetchState))
            {
                return experienceFetchState.State == ExperienceFetchState.FetchState.FETCHED;
            }

            return false;
        }

        private bool GetAuggioObject(string objectId, string experienceId, out AuggioObject auggioObject, out ExperienceFetchState state)
        {
            auggioObject = null;
            state = null;
            if (trackables.TryGetValue(experienceId, out ExperienceFetchState experienceFetchState))
            {
                if (experienceFetchState.State != ExperienceFetchState.FetchState.FETCHED) return false;
                state = experienceFetchState;
                auggioObject = experienceFetchState.Experience.Objects.FirstOrDefault(ao => ao.AuggioId == objectId);
                return auggioObject != null;
            }
            
            return false;
        }

        private void ResolveInternal(string objectId, string experienceId, AuggioObject auggioObject, Experience experience) {
            //check if scene trackable exist if not create and add to scene trackers
            if (!SceneTrackerExists(objectId, experienceId)) {
                GameObject trackerObject = new GameObject(AuggioUtils.GetGameObjectName(auggioObject.Name));
                
                AuggioObjectTracker auggioObjectTracker = trackerObject.AddComponent<AuggioObjectTracker>();

                //Bind data
                auggioObjectTracker.ExperienceId = experienceId;
                auggioObjectTracker.OrganizationId = experience.OrganizationId;
                auggioObjectTracker.AnchorId = auggioObject.AssignedAnchor;
                auggioObjectTracker.ObjectId = auggioObject.AuggioId;
                auggioObjectTracker.gameObject.name = AuggioUtils.GetGameObjectName(auggioObject.Name);
                auggioObjectTracker.ObjectName = auggioObject.Name;
                auggioObjectTracker.CreateByCodeDrivenResolving = true;
                
                auggioObjectTracker.transform.localScale = Vector3.one;
                
                auggioObjectTracker.gameObject.SetActive(false);
                
                AddSceneTracker(auggioObjectTracker);
            }

            SingleAnchor anchor = experience.AssignedLocations.SelectMany(l => l.SingleAnchorList).FirstOrDefault(an => an.AuggioId == auggioObject.AssignedAnchor);
            if (anchor != null)
            {
                //resolve anchor
                resolver.Resolve(anchor);
            }
            else
            {
                Debug.LogWarning("Auggio object " + auggioObject.Name + " is corrupted. Will not resolve.");
            }
        }

        private bool SceneTrackerExists(string objectId, string experienceId) {
            if (sceneTrackers.TryGetValue(experienceId, out List<AuggioObjectTracker> trackers)) {
                return trackers.Any(t => t.ObjectId == objectId);
            }

            return false;
        }
        
        private void OnAnchorResolved(AuggioAnchorId auggioAnchorId, ARCloudAnchor anchor) {
            //find objects assigned to the given anchor and update
            List<AuggioObjectTracker> trackers = GetSceneTrackers(auggioAnchorId);
            
            if (trackers != null) {
                foreach (AuggioObjectTracker tracker in trackers) {
                    //activate the scene tracker associated (they should have been created previously)
                    tracker.HandleTrackerActivation(auggioAnchorId, anchor, GetObjectPose(tracker.ExperienceId, tracker.ObjectId));
                    MarkAuggioObjectAsResolved(tracker.ExperienceId, tracker.ObjectId, tracker);
                }
            }
            
            anchorAfterResolveStrategy?.Process(auggioAnchorId, anchor);
        }

        private void MarkAuggioObjectAsResolved(string experienceId, string objectId, AuggioObjectTracker tracker) {
            if (auggioObjects.TryGetValue(experienceId, out List<AuggioObjectResolveState> objects)) {
                AuggioObjectResolveState auggioObjectResolveState = objects.First(ao => ao.ObjectId == objectId);
                auggioObjectResolveState.State = AuggioObjectResolveState.ResolveState.RESOLVED;
                if (GetAuggioObject(objectId, experienceId, out AuggioObject auggioObject, out ExperienceFetchState fetchState))
                {
                    Debug.Log("AuggioObject resolved " + auggioObject.Name);
                    OnAuggioObjectResolved?.Invoke(auggioObject, fetchState.Experience, tracker.transform);
                }
            }
        }

        private List<AuggioObjectTracker> GetSceneTrackers(AuggioAnchorId auggioAnchorId) {
            return sceneTrackers.SelectMany(kv => kv.Value) // Flatten all tracker lists into one sequence
                .Where(tracker => tracker.AnchorId == auggioAnchorId) // Filter by anchorId
                .ToList();
        }

        private List<AuggioObjectTracker> GetSceneTrackers(ExperienceId experienceId)
        {
            return sceneTrackers.SelectMany(kv => kv.Value)
                .Where(tracker => tracker.ExperienceId == experienceId)
                .ToList();
        }
        
        private AuggioObjectPose GetObjectPose(string experienceId, string objectId) {
            trackables.TryGetValue(experienceId, out ExperienceFetchState experienceFetchState);
            AuggioObject auggioObject = experienceFetchState.Experience.FindObjectByObjectId(objectId);
            AuggioObjectPose pose = new AuggioObjectPose(auggioObject.Position, auggioObject.Rotation, auggioObject.Scale);
            return pose;
        }

        public void DiscardTrackingData() {
            if (!initialized) {
                string message = "Tracking manager has not been initialized.";
                LogProgress(message);
                return;
            }
            //stop all fetch web requests
            StopAllCoroutines();
            StartCoroutine(ResetAnchorManager());
            TagManager.Instance?.DiscardData();
        }
        
        private void LogProgress(string log) {
            Debug.Log(log);
            if (DebugUI.Instance != null) {
                DebugUI.Instance.Log(log);
            }
        }

        private IEnumerator ResetAnchorManager() {
            anchorManager.enabled = false;
            ClearState();
            yield return new WaitForSeconds(3f);
            anchorManager.enabled = true;
        }

  
        
        #region private methods

        private void ClearTrackingState() {
            DisableAllTrackers(sceneTrackers);
            resolver?.Clear();
            
            

            if (initializeCoroutine != null) {
                StopCoroutine(initializeCoroutine);
            }
        }
        private void ClearState() {
            initialized = false;
            ClearTrackingState();
            trackables.Clear();
            sceneTrackers.Clear();
            auggioObjects.Clear();

            AuggioObjectTracker[] trackersFoundInScene = FindObjectsOfType<AuggioObjectTracker>(true);
            //destroy trackers created by code driven
            foreach (AuggioObjectTracker objectTracker in trackersFoundInScene) {
                if (objectTracker.CreateByCodeDrivenResolving) {
                    Destroy(objectTracker.gameObject);
                }
            }
        }

        private void DisableAllTrackers(Dictionary<ExperienceId, List<AuggioObjectTracker>> trackedObjects) {
            if (trackedObjects == null) return;
            foreach (KeyValuePair<ExperienceId,List<AuggioObjectTracker>> experienceTrackedObjects in trackedObjects) {
                if (experienceTrackedObjects.Value != null) {
                    foreach (AuggioObjectTracker auggioObjectTracker in experienceTrackedObjects.Value) {
                        if(auggioObjectTracker == null) continue;
                        auggioObjectTracker.DisableTracking();
                    }
                }
            }
        }
        
        
        
        #endregion
      

    }
}
