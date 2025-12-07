using System;
using System.Collections.Generic;
using System.Net.Http;
using Auggio.Plugin.SDK.Http;
using Auggio.Plugin.SDK.Model.Ids;
using Auggio.Plugin.SDK.Utils;
using Auggio.Runtime.SDK.Http;
using Auggio.Runtime.SDK.Model;
using Auggio.Runtime.serialization.Plugin.Experience;
using Auggio.Utils.Serialization;
using UnityEngine;

namespace Auggio.Runtime.SDK.Tag
{
    public class TagManager : MonoBehaviour
    {
        public static TagManager Instance { get; private set; }
        public delegate void OnTagFetchedCallback(List<TagPreview> tag);

        public delegate void OnTagFetchErrorCallback(ErrorResponse error);
        
        //ongoing request downloading the tag data
        private List<Coroutine> ongoingRequests = new List<Coroutine>(); 
        
        //downloaded tags indexed with tag id for efficient retrieval
        private Dictionary<string, TagPreview> tags = new Dictionary<string, TagPreview>();
        
        //ids of experience that has their tags downloaded
        private List<ExperienceId> fetchedExperienceIds = new List<ExperienceId>();
        
        //ids of ongoing experience fetches
        private List<ExperienceId> fetchingExperienceIds = new List<ExperienceId>();
        
        //list of fetched tags
        private List<TagPreview> fetchedTags = new List<TagPreview>();

        private void Awake()
        {
            Instance = this;
        }

        private void OnDestroy()
        {
            Instance = null;
        }

        public void DiscardData()
        {
            tags.Clear();
            StopAllCoroutines();
            ongoingRequests.Clear();
            fetchedExperienceIds.Clear();
            fetchingExperienceIds.Clear();
            fetchedTags.Clear();
        }
        
        public void FetchTagData(string experienceId, OnTagFetchedCallback onTagFetched, OnTagFetchErrorCallback onError = null)
        {
            if (fetchingExperienceIds.Contains(experienceId))
            {
                LogProgress("Tags for experience is already fetching " + experienceId);
                return;
            }
            if (fetchedExperienceIds.Contains(experienceId))
            {
                LogProgress("Tags data already fetched experience id: " + experienceId);
                return;
            }
            
            fetchingExperienceIds.Add(experienceId);
            
            LogProgress("Tags data for experience " + experienceId + " fetch started.");
            string fileToken = AuthFileTokenUtility.GetToken();
            GetTagsForExperienceRequest request = new GetTagsForExperienceRequest(experienceId);
            Coroutine requestCoroutine = null;
            requestCoroutine = StartCoroutine(WebRequestUtility.WebRequestSDK(HttpMethod.Post, "/sdk/tags", JsonUtility.ToJson(request), fileToken, (response) => {
                LogProgress("Tags data fetch success. " + experienceId);
                GetTagsPreviewResponse responseObject = JsonUtility.FromJson<GetTagsPreviewResponse>(response);
                fetchedTags.AddRange(responseObject.Tags);
                foreach (TagPreview tagPreview in responseObject.Tags)
                {
                    tags.TryAdd(tagPreview.ID, tagPreview);
                }
                fetchedExperienceIds.Add(experienceId);
                
                fetchingExperienceIds.Remove(experienceId);
                
                onTagFetched?.Invoke(responseObject.Tags);
                ongoingRequests.Remove(requestCoroutine);
            }, (s) => {
                ErrorResponse errorResponse = ErrorResponse.Get(s);
                LogProgress("Tags data fetch error code: " + errorResponse.ErrorCode + " message: " + errorResponse.Message);
                onError?.Invoke(errorResponse);
            }));
            
            ongoingRequests.Add(requestCoroutine);
        }
        
        private void LogProgress(string log) {
            Debug.Log(log);
            if (DebugUI.Instance != null) {
                DebugUI.Instance.Log(log);
            }
        }
        
        public bool IsExperienceFetched(string experienceId)
        {
            return fetchedExperienceIds.Contains(experienceId);
        }
        
        public bool GetTag(string tagId, out TagPreview tag)
        {
            if (tags.TryGetValue(tagId, out tag))
            {
                return true;
            }

            Debug.LogWarning("No tag found it was probably not fetched yet.");
            return false;
        }
        
        
        


    }
}
