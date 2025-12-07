#if UNITY_EDITOR
using System.Linq;
using Auggio.Plugin.Editor.Screen.Controller;
using Auggio.Plugin.Editor.Utils;
using Auggio.Plugin.Editor.Model.Changes;
using Auggio.Plugin.Editor.Model.Changes.Impl;
using Auggio.Utils.Serialization.Plugin.Experience;
using UnityEditor;
using UnityEngine;

namespace Auggio.Plugin.Editor.Screen
{
    internal class ExperienceDetailScreen : EditorScreen<ExperienceDetailScreenController>
    {
     
        private Vector2 _screenScrollPosition;
        private Vector2 _anchorsListScrollPosition;
        private Vector2 _changesListScrollPosition;
        
        internal ExperienceDetailScreen(AuggioEditorPlugin rootWindow, ExperienceDetailScreenController screenController, bool drawBackButton, bool useFocusLogic) : base(rootWindow, screenController, drawBackButton, useFocusLogic)
        {
        }

        protected override void Initialize()
        {
        }

        internal override void DrawOnLoadingScreen()
        {
            if (screenController.DownloadingMeshes)
            {
                EditorGUI.ProgressBar(new Rect(rootWindow.position.width/2 - 200,rootWindow.position.height/2 + 32, 400, 32),
                    screenController.MeshDownloadProgress, (int) (screenController.MeshDownloadProgress*100)+"%");
                
                if(GUI.Button(new Rect(rootWindow.position.width/2 - 100,rootWindow.position.height/2 + 96, 200, 32), "Cancel", AuggioEditorStyles.Instance.PrimaryButton))
                {
                    screenController.CancelDownloadMeshes();
                }
                //TODO cancel button
            }
        }

        internal override void DrawScreen()
        {
            DrawExperienceDetail();
        }

        private void DrawExperienceDetail()
        {
            GUILayout.BeginHorizontal();
           
            
            GUILayout.Label(
                screenController.IsInActiveScene?
                    new GUIContent(screenController.Experience.Name, EditorGUIUtility.IconContent("d_Scene").image) :
                    new GUIContent(screenController.Experience.Name), 
                AuggioEditorStyles.Instance.H1
            );
            GUILayout.FlexibleSpace();
            
            if (screenController.IsInActiveScene)
            {
                GUIContent validateButtonContent = EditorGUIUtility.IconContent("d_Scene");
                validateButtonContent.text = "Validate scene";
                if (GUILayout.Button(validateButtonContent, AuggioEditorStyles.Instance.PrimaryButton))
                {
                    screenController.DisplayValidationWindow();;
                }
            }
            
            if (screenController.LocalChanges != null && screenController.LocalChanges.Count > 0)
            {
                GUIContent uploadButtonContent = EditorGUIUtility.IconContent("Update-Available");
                uploadButtonContent.text = "Upload local changes to server";
                if (GUILayout.Button(uploadButtonContent, AuggioEditorStyles.Instance.PrimaryButton))
                {
                    screenController.UploadLocalChanges();
                }
            }

            GUIContent sceneButtonContent = EditorGUIUtility.IconContent("Download-Available");
            sceneButtonContent.text = screenController.IsInActiveScene ? "Update scene from server" : "Import scene from server";
            if(GUILayout.Button(sceneButtonContent,AuggioEditorStyles.Instance.PrimaryButton))
            {
                screenController.UpdateScene();
            }
            
            if (screenController.IsInActiveScene)
            {
                GUIContent serverChangesAvailableContent = EditorGUIUtility.IconContent(screenController.HasServerChanges? "Error" : "Installed");
                serverChangesAvailableContent.tooltip = screenController.HasServerChanges? "Server Changes Available" : "No new server changes";
                EditorGUILayout.LabelField(serverChangesAvailableContent, AuggioEditorStyles.Instance.PrimaryButton,
                    GUILayout.Width(32), GUILayout.Height(32));
            }

            GUILayout.EndHorizontal();
            
            GUIUtils.DrawLineSeparator();

            _screenScrollPosition = GUILayout.BeginScrollView(_screenScrollPosition, GUILayout.ExpandHeight(true));
            
            DrawLocalChangesTable();
            GUIUtils.DrawLineSeparator();
            DrawAnchorsTable();
            GUIUtils.DrawLineSeparator();
            DrawActions();
            
            GUILayout.EndScrollView();
        }

        private void DrawActions()
        {
            GUILayout.Label("Actions", AuggioEditorStyles.Instance.H2);
            if (screenController.LocalFilesPresent)
            {
                GUIContent deleteFilesButtonContent = new GUIContent(EditorGUIUtility.IconContent("Error"));
                deleteFilesButtonContent.text = "Delete experience files from project";
                if (GUILayout.Button(deleteFilesButtonContent, AuggioEditorStyles.Instance.PrimaryButton))
                {
                    screenController.DeleteDataFromProjectStructure();
                }

            }

            if (screenController.MeshesPresent)
            {
                GUIContent deleteAllMeshesButton = new GUIContent(EditorGUIUtility.IconContent("Error"));
                deleteAllMeshesButton.text = "Delete all meshes";
                if (GUILayout.Button(deleteAllMeshesButton, AuggioEditorStyles.Instance.PrimaryButton))
                {
                    screenController.DeleteAllMeshes();
                }
            }

        }

        private void DrawLocalChangesTable()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Local Scene Changes", AuggioEditorStyles.Instance.H2);
            GUILayout.FlexibleSpace();
            
            GUIContent refreshButtonContent = EditorGUIUtility.IconContent("TreeEditor.Refresh");
            refreshButtonContent.text = "Force Refresh";
            if (GUILayout.Button(refreshButtonContent, AuggioEditorStyles.Instance.PrimaryButton))
            {
                screenController.RefreshLocalChanges();
            }

            GUILayout.EndHorizontal();

            DrawChangesHeader();

            if (screenController.LocalChanges != null && screenController.LocalChanges.Count > 0)
            {
                _changesListScrollPosition = GUILayout.BeginScrollView(_changesListScrollPosition,
                    GUILayout.ExpandHeight(false), GUILayout.MinHeight(200),
                    GUILayout.MaxHeight(screenController.MAXChangesScrollViewHeight));
                foreach (AuggionObjectChangeBase change in screenController.LocalChanges)
                {
                    DrawChangeDataRow(change);
                }

                GUILayout.EndScrollView();
            }
            else
            {
                GUILayout.Label("No local changes found. Try to click on 'Force Refresh' button.", AuggioEditorStyles.WithMargin(
                    AuggioEditorStyles.WithTextAlignment(
                                AuggioEditorStyles.Instance.H3,
                        TextAnchor.MiddleCenter)
                , new RectOffset(0,0,16,16)));
            }
        }

        private void DrawAnchorsTable()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Anchors", AuggioEditorStyles.Instance.H2);
            GUILayout.FlexibleSpace();

            if (screenController.Experience.AssignedLocations.Count > 0 && screenController.Experience.AssignedLocations
                .SelectMany(location => location.SingleAnchorList).ToList().Count > 0)
            {
                
                GUIContent downloadButtonContent = EditorGUIUtility.IconContent("Download-Available");
                downloadButtonContent.text = "Download meshes";
                if (GUILayout.Button(downloadButtonContent, AuggioEditorStyles.Instance.PrimaryButton))
                {
                    screenController.DownloadMeshes();
                }
            }
            GUILayout.EndHorizontal();
            DrawAnchorDataHeader();
            _anchorsListScrollPosition = GUILayout.BeginScrollView(_anchorsListScrollPosition, GUILayout.ExpandHeight(false),
                GUILayout.MinHeight(100), GUILayout.MaxHeight(screenController.MAXAnchorsScrollViewHeight));
            foreach (Location location in screenController.Experience.AssignedLocations)
            {
                foreach (SingleAnchor singleAnchor in location.SingleAnchorList)
                {
                    DrawAnchorDataRow(singleAnchor, location);
                }
            }

            GUILayout.EndScrollView();
        }

        private void DrawAnchorDataHeader()
        {
            GUILayout.Space(1);
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal(AuggioEditorStyles.Instance.TableHeaderStyle);
            GUILayout.Label("Anchor Name", AuggioEditorStyles.WithTextAlignment(AuggioEditorStyles.Instance.BoldText, TextAnchor.MiddleCenter), GUILayout.Width(rootWindow.position.width*0.28f), GUILayout.Height(48));
            GUILayout.Label("Location", AuggioEditorStyles.WithTextAlignment(AuggioEditorStyles.Instance.BoldText, TextAnchor.MiddleCenter),GUILayout.Width(rootWindow.position.width*0.28f), GUILayout.Height(48));
            GUILayout.Label("Download status", AuggioEditorStyles.WithTextAlignment(AuggioEditorStyles.Instance.BoldText, TextAnchor.MiddleCenter),GUILayout.Width(rootWindow.position.width*0.28f), GUILayout.Height(48));
            GUILayout.Label("Actions", AuggioEditorStyles.WithTextAlignment(AuggioEditorStyles.Instance.BoldText, TextAnchor.MiddleCenter),GUILayout.Width(rootWindow.position.width*0.12f), GUILayout.Height(48));
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            GUILayout.Space(1);
        }

        private void DrawAnchorDataRow(SingleAnchor anchor, Location location)
        {
            GUILayout.Space(1);
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal(AuggioEditorStyles.Instance.TableRowStyle);
            GUILayout.Label(anchor.Name, AuggioEditorStyles.WithTextAlignment(AuggioEditorStyles.Instance.NormalText, TextAnchor.MiddleCenter), GUILayout.Width(rootWindow.position.width*0.28f), GUILayout.Height(48));
            GUILayout.Label(location.Name, AuggioEditorStyles.WithTextAlignment(AuggioEditorStyles.Instance.NormalText, TextAnchor.MiddleCenter), GUILayout.Width(rootWindow.position.width*0.28f), GUILayout.Height(48));
            bool isDownloaded = screenController.IsDownloaded(anchor, location);
            GUIContent downloadStatus = EditorGUIUtility.IconContent(isDownloaded ? "Installed" : "Error");
            downloadStatus.tooltip = isDownloaded ? "Downloaded" : "Missing";
            
            GUILayout.Label(
                downloadStatus,
                AuggioEditorStyles.WithTextColor(AuggioEditorStyles.WithTextAlignment(AuggioEditorStyles.Instance.NormalText, TextAnchor.MiddleCenter), isDownloaded? AuggioEditorStyles.Instance.Green: AuggioEditorStyles.Instance.Red),
                GUILayout.Width(rootWindow.position.width*0.28f), GUILayout.Height(48));

            GUILayout.BeginHorizontal(GUILayout.Width(rootWindow.position.width*0.12f), GUILayout.Height(48));
            if (isDownloaded)
            {
                if (GUILayout.Button("Delete",
                    AuggioEditorStyles.WithMargin(AuggioEditorStyles.Instance.PrimaryButton,
                        new RectOffset(2, 2, 8, 8)), GUILayout.Height(48)))
                {
                    screenController.DeleteMeshForAnchor(location, anchor);
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            GUILayout.Space(1);
        }

        private void DrawChangesHeader()
        {
            //TODO refactor styles?
            GUILayout.Space(1);
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal(AuggioEditorStyles.Instance.TableHeaderStyle);
            GUILayout.Label("Object ID", AuggioEditorStyles.WithTextAlignment(AuggioEditorStyles.Instance.BoldText, TextAnchor.MiddleCenter), GUILayout.Width(rootWindow.position.width*0.18f), GUILayout.Height(48));
            GUILayout.Label("Change", AuggioEditorStyles.WithTextAlignment(AuggioEditorStyles.Instance.BoldText, TextAnchor.MiddleCenter),GUILayout.Width(rootWindow.position.width*0.18f), GUILayout.Height(48));
            GUILayout.Label("Old Value", AuggioEditorStyles.WithTextAlignment(AuggioEditorStyles.Instance.BoldText, TextAnchor.MiddleCenter),GUILayout.Width(rootWindow.position.width*0.18f), GUILayout.Height(48));
            GUILayout.Label("New Value", AuggioEditorStyles.WithTextAlignment(AuggioEditorStyles.Instance.BoldText, TextAnchor.MiddleCenter),GUILayout.Width(rootWindow.position.width*0.18f), GUILayout.Height(48));
            GUILayout.Label("Actions", AuggioEditorStyles.WithTextAlignment(AuggioEditorStyles.Instance.BoldText, TextAnchor.MiddleCenter),GUILayout.Width(rootWindow.position.width*0.18f), GUILayout.Height(48));
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            GUILayout.Space(1);
        }
        
        private void DrawChangeDataRow(AuggionObjectChangeBase change)
        {
            //TODO refactor styles?
            GUILayout.Space(1);
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal(AuggioEditorStyles.Instance.TableRowStyle);
            GUILayout.Label(change.ObjectName, AuggioEditorStyles.WithTextAlignment(AuggioEditorStyles.Instance.NormalText, TextAnchor.MiddleCenter), GUILayout.Width(rootWindow.position.width*0.225f), GUILayout.Height(48));
            GUILayout.Label(change.UIStringName(), AuggioEditorStyles.WithTextAlignment(AuggioEditorStyles.Instance.NormalText, TextAnchor.MiddleCenter), GUILayout.Width(rootWindow.position.width*0.18f), GUILayout.Height(48));

            GUILayout.Label(change.OldValueToString(), AuggioEditorStyles.WithTextAlignment(AuggioEditorStyles.Instance.NormalText, TextAnchor.MiddleCenter), GUILayout.Width(rootWindow.position.width*0.18f), GUILayout.Height(48));
            GUILayout.Label(change.NewValueToString(), GUILayout.Width(rootWindow.position.width*0.18f), GUILayout.Height(48));
            GUILayout.BeginHorizontal(GUILayout.Width(rootWindow.position.width*0.18f), GUILayout.Height(48));

            if (change is not ObjectDeleted && change is not PlaceholderDeleted)
            {
                if (GUILayout.Button("Select in Scene",
                    AuggioEditorStyles.WithMargin(AuggioEditorStyles.Instance.PrimaryButton, new RectOffset(2, 2, 8, 8)),
                    GUILayout.Height(48)))
                {
                    screenController.SelectAuggioObjectInScene(change);
                }
            }

            if (GUILayout.Button("Discard", AuggioEditorStyles.WithMargin(AuggioEditorStyles.Instance.PrimaryButton, new RectOffset(2,2,8,8)),  GUILayout.Height(48)))
            {
                screenController.DiscardChange(change);
            }
            GUILayout.EndHorizontal();
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            GUILayout.Space(1);
        }



    }
}
#endif
