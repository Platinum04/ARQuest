using System.IO;
using UnityEngine;

namespace Auggio.Plugin.Editor.Utils
{
    internal abstract class AbstractFileCache<T>
    {
        protected string directoryPath;
        protected string fileName;

        protected AbstractFileCache(string directoryPath, string fileName)
        {
            this.directoryPath = directoryPath;
            this.fileName = fileName;

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
        }

        internal abstract void Save(T obj);
        internal abstract T Load();

        internal abstract void Delete();
    }
}
