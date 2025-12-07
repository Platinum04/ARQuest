#if UNITY_EDITOR
using Auggio.Plugin.Editor;
using Auggio.Plugin.Editor.Screen.Controller;

namespace Auggio.Plugin.Screen.Controller
{
    internal class LoginSettingsScreenController : AbstractScreenController
    {
    
        internal LoginSettingsScreenController(EditorWindowController windowController) : base(windowController)
        {
        }
    }
}

#endif
