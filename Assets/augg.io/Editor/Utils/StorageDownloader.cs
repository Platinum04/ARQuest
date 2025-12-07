#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Auggio.Utils.Serialization.Plugin.Experience;
using UnityEngine;

namespace Auggio.Plugin.Editor.Utils
{
    internal class StorageDownloader
    {
    
        internal delegate void OnMeshesDownloaded();
    
        internal delegate void OnMeshesDownloadError(string reason);

        internal delegate void OnProgressUpdated(long currentBytes, long totalBytes);

        private CancellationTokenSource _cancellationTokenSource;
        private string dataFolder;
        private Dictionary<string, long> downloadProgress;

        public StorageDownloader(string dataFolder)
        {
            this.dataFolder = dataFolder;
        }

        internal void CancelTasks()
        {
            _cancellationTokenSource.Cancel();
        }

        internal async Task DownloadMeshes(List<MeshDownloadInfo> meshesToDownload, OnMeshesDownloaded onSuccess, OnMeshesDownloadError onError, OnProgressUpdated onProgressUpdated)
        {
            if (!Directory.Exists(dataFolder))
            {
                Directory.CreateDirectory(dataFolder);
            }

            long totalBytes = meshesToDownload.Sum(info => info.SizeInBytes);
            
            _cancellationTokenSource = new CancellationTokenSource();
            using HttpClient httpClient = new HttpClient();
        
            downloadProgress = meshesToDownload.ToDictionary(info => info.MeshInfo.GetLocalPath(), _ => 0L);

            List<Task> downloadTasks = new List<Task>();
            foreach (MeshDownloadInfo item in meshesToDownload)
            {
                downloadTasks.Add(DownloadMeshForAnchor(httpClient, item, totalBytes, onProgressUpdated, onError));
            }

            await Task.WhenAll(downloadTasks);
            
            onSuccess?.Invoke();
        }
    
        private async Task DownloadMeshForAnchor(HttpClient httpClient, MeshDownloadInfo meshDownloadInfo, long totalBytes, OnProgressUpdated onProgressUpdated, OnMeshesDownloadError onError)
        {
            string savePath = Path.Combine(dataFolder, meshDownloadInfo.MeshInfo.GetLocalPath());
            string directoryPath = Path.Combine(dataFolder, meshDownloadInfo.MeshInfo.GetFolderPath());
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
        
            HttpResponseMessage httpResponseMessage =
                await httpClient.GetAsync(meshDownloadInfo.URL, _cancellationTokenSource.Token);

            if (!httpResponseMessage.IsSuccessStatusCode)
            {
                Debug.LogError(httpResponseMessage.ReasonPhrase);
                onError?.Invoke(httpResponseMessage.ReasonPhrase);
            }

            using (Stream contentStream = await httpResponseMessage.Content.ReadAsStreamAsync())
            {
                using (FileStream fileStream = new FileStream(savePath, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    byte[] buffer = new byte[1048576];
                    int bytesRead;
                    while ((bytesRead = contentStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        await fileStream.WriteAsync(buffer, 0, bytesRead, _cancellationTokenSource.Token);
                        downloadProgress[meshDownloadInfo.MeshInfo.GetLocalPath()] += bytesRead;
                        onProgressUpdated?.Invoke(downloadProgress.Values.Sum(), totalBytes);
                    }
                }
            }
        } 
   
    }
}
#endif
