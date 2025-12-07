using System.IO;
using UnityEngine;

namespace Auggio.Plugin.SDK.Utils
{
    internal static class AuthFileTokenUtility {

        private static string TOKEN_PATH = Path.Combine("auggioFileToken");

        internal static bool IsFileTokenAvailable() {
            TextAsset textAsset = Resources.Load<TextAsset>(TOKEN_PATH);
            return textAsset != null;
        }
        
        internal static string GetToken() {
            TextAsset textAsset = Resources.Load<TextAsset>(TOKEN_PATH);
            ApplicationToken applicationToken = JsonUtility.FromJson<ApplicationToken>(textAsset.text);
            return applicationToken.Token;
        }
    }
}
