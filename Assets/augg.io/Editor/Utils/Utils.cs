using System;
using System.IO;
using UnityEngine;

namespace Auggio.Plugin.Editor.Utils
{
    internal class Utils
    {
        
        internal static bool IsValidUUID(string str)
        {
            if (string.IsNullOrEmpty(str))
                return false;

            Guid guidOutput;
            return Guid.TryParse(str, out guidOutput);
        }

       
    }
}
