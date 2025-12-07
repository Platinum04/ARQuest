using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Auggio.Plugin.Editor.Utils {
    public static class APIKeyManager
    {

        private const string API_KEY_PREFS_KEY = "auggio-api-key";
        
        public static void SetActiveKey(string key) {
            EditorPrefs.SetString(API_KEY_PREFS_KEY, key);
        }
        
        public static bool GetActiveKey(out string key)
        {
            key = EditorPrefs.GetString(API_KEY_PREFS_KEY, string.Empty);
            if (key == string.Empty)
            {
                return false;
            }

            return true;
        }

        public static void RemoveCurrentKey()
        {
            EditorPrefs.DeleteKey(API_KEY_PREFS_KEY);
        }
    }
}
