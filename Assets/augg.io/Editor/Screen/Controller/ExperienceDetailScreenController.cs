#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using Auggio.Plugin.Editor.Http;
using Auggio.Plugin.Editor.Utils;
using Auggio.Plugin.Editor.Model;
using Auggio.Plugin.Editor.Model.Changes;
using Auggio.Plugin.Editor.SDK;
using Auggio.Plugin.SDK.Model;
using Auggio.Plugin.SDK.Utils;
using Auggio.Utils.Serialization.Plugin.Experience;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using WebRequestUtility = Auggio.Plugin.Editor.Utils.WebRequestUtility;

namespace Auggio.Plugin.Editor.Screen.Controller
{
    internal class ExperienceDetailScreenController : AbstractScreenController
    {
        private string meshFolder;
        private Experience windowExperience;

        private float meshDownloadProgress;
        private bool downloadingMeshes;

        private StorageDownloader storageDownloader;
        private EditorCoroutine downloadInfoCoroutine;

        private List<AuggionObjectChangeBase> localChanges;

        private float maxAnchorsScrollViewHeight;
        private float maxChangesScrollViewHeight;

        private bool isInActiveScene;
        private bool localFilesPresent;
        private bool meshesPresent;
        private bool hasServerChanges;

        internal ExperienceDetailScreenController(EditorWindowController windowController) : base(windowController)
        {
            meshFolder = AuggioUtils.GetMeshDataPath();
        }

        internal Experience Experience
        {
            get => windowExperience;
            set => windowExperience = value;
        }

        internal float MeshDownloadProgress => meshDownloadProgress;

        internal bool DownloadingMeshes => downloadingMeshes;

        internal List<AuggionObjectChangeBase> LocalChanges => localChanges;

        internal float MAXAnchorsScrollViewHeight => maxAnchorsScrollViewHeight;

        internal float MAXChangesScrollViewHeight => maxChangesScrollViewHeight;

        internal bool IsInActiveScene => isInActiveScene;

        internal bool LocalFilesPresent => localFilesPresent;

        internal bool MeshesPresent => meshesPresent;

        internal bool HasServerChanges => hasServerChanges;


        internal override void OnScreenBecomeActive()
        {
            maxAnchorsScrollViewHeight = GetAnchorScrollViewHeight();
            RefreshScreenData();
        }

        internal override void OnScreenFocus()
        {
            RefreshScreenData();
        }

        internal override void OnSceneOpened(Scene scene)
        {
            RefreshScreenData();
        }

        private void RefreshScreenData()
        {
            RefreshLocalFilesPresent();
            RefreshMeshesPresent();
            RefreshIsInActiveScene();
            if (isInActiveScene)
            {
                RefreshServerChangesStatus(() =>
                {
                    RefreshLocalFilesPresent();
                    RefreshLocalChanges();
                });
            }
            else
            {
                RefreshLocalChanges();
            }
           
        }

        private void RefreshServerChangesStatus(Action callback)
        {
            ExperienceFileCache experienceFileCache =
                new ExperienceFileCache(windowExperience.OrganizationId, windowExperience.ID);
            try
            {
                Experience cachedExperience = experienceFileCache.Load();
                GetServerChanges(cachedExperience, callback);
            }
            catch (FileNotFoundException)
            {
                WebRequestUtility.WebRequestPlugin(windowController.RootWindow, HttpMethod.Get,
                    "/plugin/experience/" + windowExperience.ID, null, windowController.RootWindow.AuggioAPIKey,
                    (experienceJson) =>
                    {
                        Experience serverExperience = JsonUtility.FromJson<Experience>(experienceJson);
                        experienceFileCache.Save(serverExperience);
                        GetServerChanges(serverExperience, callback);
                    }, OnError);
            }
        }

        private void GetServerChanges(Experience cachedExperience, Action callback)
        {
            WebRequestUtility.WebRequestPlugin(windowController.RootWindow, HttpMethod.Get,
                "/plugin/experience/changes", JsonUtility.ToJson(cachedExperience),
                windowController.RootWindow.AuggioAPIKey, (response) =>
                {
                    OnServerChangesStatusRefreshed(response);
                    callback?.Invoke();
                }, OnError);
        }

        private void OnServerChangesStatusRefreshed(string response)
        {
            Boolean.TryParse(response, out hasServerChanges);
        }

        private void RefreshIsInActiveScene()
        {
            isInActiveScene =
                (AuggioSceneManager.GetExperienceFromSceneById(SceneManager.GetActiveScene(), Experience.ID) != null);
        }

        private void RefreshLocalFilesPresent()
        {
            ExperienceFileCache experienceFileCache =
                new ExperienceFileCache(windowExperience.OrganizationId, windowExperience.ID);
            try
            {
                Experience experience = experienceFileCache.Load();
                localFilesPresent = true;
            }
            catch (FileNotFoundException)
            {
                localFilesPresent = false;
            }
        }

        private void RefreshMeshesPresent()
        {
            if (Experience.AssignedLocations == null)
            {
                meshesPresent = false;
                return;
            }

            meshesPresent = Experience.AssignedLocations.Any(location =>
            {
                if (location.SingleAnchorList == null)
                {
                    return false;
                }

                string path = Path.Combine(AuggioUtils.GetMeshDataPath(true), location.OrganizationId,
                    location.ID);
                return location.SingleAnchorList.Any(anchor =>
                {
                    string anchorPath = Path.Combine(path, anchor.AuggioId, anchor.MeshHash);
                    return File.Exists(anchorPath);
                });
            });
        }

        private void ReFetchExperience(Action<string> callback)
        {
            windowController.SetLoading(true);
            windowController.RootWindow.Errors.Clear();
            WebRequestUtility.WebRequestPlugin(windowController.RootWindow, HttpMethod.Get,
                "/plugin/experience/" + windowExperience.ID, null, windowController.RootWindow.AuggioAPIKey, callback,
                OnError);
        }

        private float GetAnchorScrollViewHeight()
        {
            float totalHeight = 0;
            foreach (Location location in windowExperience.AssignedLocations)
            {
                foreach (SingleAnchor singleAnchor in location.SingleAnchorList)
                {
                    totalHeight += AuggioEditorStyles.Instance.TableRowStyle.fixedHeight + 4;
                }
            }

            return totalHeight;
        }

        private float GetLocalChangesScrollViewHeight()
        {
            float totalHeight = 0;
            if (localChanges == null)
            {
                return 0f;
            }

            foreach (AuggionObjectChangeBase localChange in localChanges)
            {
                totalHeight += AuggioEditorStyles.Instance.TableRowStyle.fixedHeight + 4;
            }

            return totalHeight;
        }

        internal bool IsDownloaded(SingleAnchor anchor, Location location)
        {
            return File.Exists(Path.Combine(meshFolder, location.OrganizationId, location.ID, anchor.AuggioId,
                anchor.MeshHash));
        }

        internal void DownloadMeshes(Action callback = null)
        {
            windowController.RootWindow.Errors.Clear();
            downloadingMeshes = true;
            meshDownloadProgress = 0;
            windowController.SetLoading(true, "Downloading meshes...");
            //TODO filter already downloaded meshes?
            List<MeshInfo> meshesToDownload = new List<MeshInfo>();
            foreach (Location location in windowExperience.AssignedLocations)
            {
                foreach (SingleAnchor singleAnchor in location.SingleAnchorList)
                {
                    meshesToDownload.Add(new MeshInfo(location.OrganizationId, location.ID, singleAnchor.AuggioId,
                        singleAnchor.MeshHash));
                }
            }

            MeshDownloadRequest meshDownloadRequest = new MeshDownloadRequest();
            meshDownloadRequest.MeshInfos = meshesToDownload;
            string body = JsonUtility.ToJson(meshDownloadRequest);
            downloadInfoCoroutine = WebRequestUtility.WebRequestPlugin(windowController.RootWindow, HttpMethod.Get,
                "/plugin/mesh/downloadInfos", body, windowController.RootWindow.AuggioAPIKey,
                 (response) => { OnDownloadInfoFetched(response, callback); }, OnError);
        }

        internal void UpdateScene()
        {
            bool proceed = true;
            if (localChanges != null && localChanges.Count > 0)
            {
                proceed = EditorUtility.DisplayDialog("Overwrite local changes?",
                    "Updating scene from server will discard all your local changes. Do you wish to continue? ",
                    "Overwrite local changes", "Cancel");
            }

            if (proceed)
            {
                ReFetchExperience((response =>
                {
                    Experience backendExperience = JsonUtility.FromJson<Experience>(response);
                    windowController.ExperienceDetailScreen.ScreenController.Experience = backendExperience;

                    bool downloadMeshes = false;
                    if (!AllMeshesDownloaded(backendExperience))
                    {
                        downloadMeshes = EditorUtility.DisplayDialog("Download meshes?",
                            "It looks like you are missing one or more mesh scans for anchors present in the experience. Would you like to download them?",
                            "Download", "Continue without downloading");
                    }

                    if (downloadMeshes)
                    {
                        DownloadMeshes(() =>
                        {
                            UpdateExperienceInScene(backendExperience);
                            RefreshScreenData();
                        });
                    }
                    else
                    {
                        UpdateExperienceInScene(backendExperience);
                        RefreshScreenData();
                    }
                }));
            }
        }

        internal void DisplayValidationWindow()
        {
            ValidateSceneWindow.ShowWindow(Experience);
        }

        private void UpdateExperienceInScene(Experience experience)
        {
            AuggioSceneManager.ProcessExperience(SceneManager.GetActiveScene(), experience,
                windowController.RootWindow.DefaultImportMaterial,
                windowController.RootWindow.VisualizeMeshOnImport);
            RefreshLocalChanges();
            windowController.SetLoading(false);
        }

        private bool AllMeshesDownloaded(Experience experience)
        {
            foreach (Location location in experience.AssignedLocations)
            {
                foreach (SingleAnchor anchor in location.SingleAnchorList)
                {
                    if (!IsDownloaded(anchor, location))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        internal void CancelDownloadMeshes()
        {
            if (downloadInfoCoroutine != null)
            {
                EditorCoroutineUtility.StopCoroutine(downloadInfoCoroutine);
                downloadInfoCoroutine = null;
            }

            if (storageDownloader != null)
            {
                storageDownloader.CancelTasks();
            }

            downloadingMeshes = false;
            windowController.RootWindow.Errors.Clear();
            windowController.SetLoading(false);
            Debug.Log("Downloading of meshes was cancelled.");
        }

        internal void SelectAuggioObjectInScene(AuggionObjectChangeBase change)
        {
            EditorGUIUtility.PingObject(change.SceneGameObject);
            Selection.activeGameObject = change.SceneGameObject;
        }

        private async void OnDownloadInfoFetched(string response, Action callback)
        {
            downloadInfoCoroutine = null;
            MeshDownloadResponse downloadResponse = JsonUtility.FromJson<MeshDownloadResponse>(response);

            storageDownloader =
                new StorageDownloader(meshFolder);

            await storageDownloader.DownloadMeshes(downloadResponse.DownloadInfo, () => { OnMeshesDownloaded(callback); },
                OnError, OnMeshDownloadProgressUpdate);
        }

        private void OnMeshDownloadProgressUpdate(long currentbytes, long totalbytes)
        {
            meshDownloadProgress = ((currentbytes / (float)totalbytes));
            windowController.RootWindow.Repaint();
        }

        private void OnMeshesDownloaded(Action callback)
        {
            Debug.Log("[augg.io] Meshes downloaded");
            windowController.SetLoading(false);
            downloadingMeshes = false;
            callback?.Invoke();
            AssetDatabase.Refresh();
            RefreshMeshesPresent();
        }

        private void OnError(string error)
        {
            downloadInfoCoroutine = null;
            windowController.RootWindow.Errors.Add(error);
            windowController.SetLoading(false);
            downloadingMeshes = false;
        }

        internal void RefreshLocalChanges()
        {
            windowController.RootWindow.Errors.Clear();
            windowController.SetLoading(true, "Finding local changes");
            try
            {
                localChanges =
                    AuggioSceneChangesManager.GetLocalChanges(SceneManager.GetActiveScene(), windowExperience);
            }
            catch (Exception e)
            {
                string error =
                    "Cannot refresh local changes. Hierarchy may be corrupted, we recommend to update scene from server. See console for more details.";
                AuggioEditorPlugin.ShowErrorDialog(error);
                windowController.RootWindow.Errors.Add(error);
                Debug.LogError(e);
                Debug.LogError(error);
            }

            maxChangesScrollViewHeight = GetLocalChangesScrollViewHeight();
            windowController.SetLoading(false);
        }

        internal void DiscardChange(AuggionObjectChangeBase change)
        {
            AuggioSceneChangesManager.DiscardChange(SceneManager.GetActiveScene(), windowExperience, change);
            RefreshLocalChanges();
        }

        internal void UploadLocalChanges()
        {
            if (AuggioSceneManager.GetGameObjectsWithErrors(windowExperience).Count > 0)
            {
                if (EditorUtility.DisplayDialog("Experience in scene has unresolved errors",
                        "Uploading experience with errors may result into invalid state. Please fix the errors before uploading. ",
                        "Display errors", "Ok"))
                {
                    DisplayValidationWindow();
                }

                ;
                return;
            }

            bool proceed = true;
            if (hasServerChanges)
            {
                proceed = EditorUtility.DisplayDialog("Overwrite server changes?",
                    "By uploading local changes, you will overwrite changes on server. Do you wish to proceed? ",
                    "Overwrite server changes", "Cancel");
            }

            if (!proceed)
            {
                return;
            }

            windowController.SetLoading(true, "Uploading changes to backend");
            if (localChanges == null || localChanges.Count == 0)
            {
                return;
            }

            ExperienceFileCache experienceFileCache =
                new ExperienceFileCache(windowExperience.OrganizationId, windowExperience.ID);
            Experience cachedExperience = experienceFileCache.Load();

            foreach (AuggionObjectChangeBase changeBase in localChanges)
            {
                changeBase.ApplyToExperience(cachedExperience);
            }

            List<string> newObjects = cachedExperience.Objects.FindAll(o => o.IsNewObject)
                .Select(newObject => newObject.AuggioId).ToList();
            List<string> newPlaceholders = cachedExperience.Objects.SelectMany(o => o.PlaceholderModels).ToList()
                .FindAll(placeholder => placeholder.IsNewPlaceholder).Select(placeholder => placeholder.ID).ToList();

            EditExperienceRequest request = new EditExperienceRequest(cachedExperience, newObjects, newPlaceholders);
            WebRequestUtility.WebRequestPlugin(windowController.RootWindow, HttpMethod.Post,
                "/plugin/experience/upload",
                JsonUtility.ToJson(request), windowController.RootWindow.AuggioAPIKey,
                (response) => { OnLocalChangesUploaded(response, newObjects, newPlaceholders); },
                (error) => OnLocalChangesUploadError(error, newObjects, newPlaceholders));
        }

        private void OnLocalChangesUploaded(string response, List<string> newObjects, List<string> newPlaceholders)
        {
            UploadExperienceChangesResponse changesResponse =
                JsonUtility.FromJson<UploadExperienceChangesResponse>(response);

            foreach (string oldId in newObjects)
            {
                AuggioObjectTracker tracker = AuggioSceneManager.GetAuggioObjectTrackerFromScene(
                    SceneManager.GetActiveScene(),
                    changesResponse.Experience.ID, oldId);
                if (tracker == null)
                {
                    Debug.LogError("Cannot find AuggioObjectTracker for ID = " + oldId);
                    continue;
                }

                string newId = null;
                foreach (ObjectIdMapping objectIdMapping in changesResponse.IDMapping)
                {
                    if (objectIdMapping.OldId.Equals(oldId))
                    {
                        newId = objectIdMapping.NewId;
                        break;
                    }
                }

                if (string.IsNullOrEmpty(newId))
                {
                    throw new ArgumentException("Missing new ID for new object");
                }

                tracker.ObjectId = newId;
            }

            foreach (string oldId in newPlaceholders)
            {
                AuggioObjectPlaceholderModel model =
                    AuggioSceneManager.GetAuggioPlaceholderModelFromScene(SceneManager.GetActiveScene(),
                        changesResponse.Experience.ID, oldId);
                if (model == null)
                {
                    Debug.LogError("Cannot find AuggioObjectPlaceholderModel for ID = " + oldId);
                    continue;
                }

                string newId = null;
                foreach (ObjectIdMapping objectIdMapping in changesResponse.PlaceholderIdMapping)
                {
                    if (objectIdMapping.OldId.Equals(oldId))
                    {
                        newId = objectIdMapping.NewId;
                        break;
                    }
                }

                if (string.IsNullOrEmpty(newId))
                {
                    throw new ArgumentException("Missing new ID for new object");
                }

                model.PlaceholderId = newId;
            }

            ExperienceFileCache experienceFileCache = new ExperienceFileCache(changesResponse.Experience.OrganizationId,
                changesResponse.Experience.ID);
            experienceFileCache.Save(changesResponse.Experience);
            windowExperience = changesResponse.Experience;
            RefreshLocalChanges();
            windowController.SetLoading(false);
            Debug.Log("[augg.io] Local changes were successfully uploaded");
        }

        private void OnLocalChangesUploadError(string error, List<string> newObjects, List<string> newPlaceholders)
        {
            OnError(error);
        }

        public void DeleteDataFromProjectStructure()
        {
            bool proceed = EditorUtility.DisplayDialog("Delete experience data from project?",
                "If experience is in active scene data will be automatically re-downloaded once you open Experience detail in augg.io Editor Window. If you wish to delete files permanently please make sure the experience is also deleted from all of your scenes. Do you wish to proceed? ",
                "Delete experience data", "Cancel");
            if (!proceed)
            {
                return;
            }

            ExperienceFileCache experienceFileCache = new ExperienceFileCache(Experience.OrganizationId, Experience.ID);
            experienceFileCache.Delete();

            string experienceDirectoryPath = AuggioUtils.GetExperienceDirectoryPath(Experience.OrganizationId);
            if (Directory.Exists(experienceDirectoryPath) && IsUnityFolderEffectivelyEmpty(experienceDirectoryPath))
            {
                Debug.Log("[augg.io] Deleting empty experiences directory");
                Directory.Delete(experienceDirectoryPath, true);
                File.Delete(experienceDirectoryPath + ".meta");
            }

            string organizationDirectoryPath = AuggioUtils.GetOrganizationDirectoryPath(Experience.OrganizationId);
            if (Directory.Exists(organizationDirectoryPath) && IsUnityFolderEffectivelyEmpty(organizationDirectoryPath))
            {
                Debug.Log("[augg.io] Deleting empty organization directory");
                Directory.Delete(organizationDirectoryPath, true);
                File.Delete(organizationDirectoryPath + ".meta");
            }

            AssetDatabase.Refresh();
            Debug.Log("[augg.io] Successfully deleted experience data from project");
            RefreshLocalFilesPresent();
        }

        public void DeleteAllMeshes()
        {
            bool proceed = EditorUtility.DisplayDialog("Delete all meshes from project?",
                "If you delete meshes you won't be able to see them in the scene view. Please note meshes can be shared between experiences, so it may affect other experiences. Do you wish to proceed? ",
                "Delete all meshes", "Cancel");
            if (!proceed)
            {
                return;
            }

            //delete meshes from scene if there are any
            Scene activeScene = SceneManager.GetActiveScene();
            AuggioExperience experienceTransform =
                AuggioSceneManager.GetExperienceFromSceneById(activeScene, Experience.ID);
            if (experienceTransform != null)
            {
                VisualizationHierarchy visualizationHierarchy =
                    experienceTransform.GetComponentInChildren<VisualizationHierarchy>(true);
                if (visualizationHierarchy != null)
                {
                    visualizationHierarchy.VisualizeMesh = false;
                    foreach (AuggioLocation location in visualizationHierarchy.transform
                                 .GetComponentsInChildren<AuggioLocation>(true))
                    {
                        AuggioLocationEditor.HideLocationMeshes(location);
                    }

                    EditorUtility.SetDirty(visualizationHierarchy);
                    EditorSceneManager.MarkSceneDirty(activeScene);
                }
            }

            foreach (Location location in Experience.AssignedLocations)
            {
                string organizationDirectoryPath = Path.Combine(meshFolder, location.OrganizationId);
                string locationDirectoryPath = Path.Combine(organizationDirectoryPath, location.ID);
                foreach (SingleAnchor anchor in location.SingleAnchorList)
                {
                    string anchorDirectoryPath = Path.Combine(locationDirectoryPath, anchor.AuggioId);
                    string path = Path.Combine(anchorDirectoryPath, anchor.MeshHash);
                    string pathMeta = Path.Combine(anchorDirectoryPath, anchor.MeshHash + ".meta");
                    if (File.Exists(path))
                    {
                        File.Delete(path);
                    }

                    if (File.Exists(pathMeta))
                    {
                        File.Delete(pathMeta);
                    }

                    if (Directory.Exists(anchorDirectoryPath))
                    {
                        Debug.Log("[augg.io] Deleting empty anchor folder " + path);
                        Directory.Delete(anchorDirectoryPath, true);
                        File.Delete(anchorDirectoryPath + ".meta");
                    }
                }

                if (Directory.Exists(locationDirectoryPath) && IsUnityFolderEffectivelyEmpty(locationDirectoryPath))
                {
                    Debug.Log("[augg.io] Deleting empty location folder " + locationDirectoryPath);
                    Directory.Delete(locationDirectoryPath, true);
                    File.Delete(locationDirectoryPath + ".meta");
                }

                if (Directory.Exists(organizationDirectoryPath) &&
                    IsUnityFolderEffectivelyEmpty(organizationDirectoryPath))
                {
                    Debug.Log("[augg.io] Deleting empty organization folder " + organizationDirectoryPath);
                    Directory.Delete(organizationDirectoryPath, true);
                    File.Delete(organizationDirectoryPath + ".meta");
                }
            }

            RefreshMeshesPresent();
            AssetDatabase.Refresh();
            Debug.Log("[augg.io] Meshes successfully deleted from project");
        }

        public void DeleteMeshForAnchor(Location location, SingleAnchor anchor)
        {
            bool proceed = EditorUtility.DisplayDialog("Delete meshes for " + anchor.Name + " anchor?",
                "If you delete meshes you won't be able to see them in the scene view. Please note meshes can be shared between experiences, so it may affect other experiences. Do you wish to proceed? ",
                "Delete meshes for this anchor", "Cancel");
            if (!proceed)
            {
                return;
            }

            string path = Path.Combine(meshFolder, location.OrganizationId, location.ID, anchor.AuggioId,
                anchor.MeshHash);
            if (File.Exists(path))
            {
                File.Delete(path);
            }

            AssetDatabase.Refresh();
        }

        private bool IsUnityFolderEffectivelyEmpty(string path)
        {
            return !Directory.EnumerateFileSystemEntries(path)
                .Any(entry =>
                {
                    string fileName = Path.GetFileName(entry);
                    string extension = Path.GetExtension(entry);

                    // Skip hidden files and meta files
                    return !fileName.StartsWith(".") && extension != ".meta";
                });
        }
    }
}

#endif