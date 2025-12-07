using UnityEngine.SceneManagement;

#if UNITY_EDITOR
namespace Auggio.Plugin.Editor.Screen.Controller
{
    internal abstract class AbstractScreenController
    {
        protected EditorWindowController windowController;

        protected AbstractScreenController(EditorWindowController windowController)
        {
            this.windowController = windowController;
        }

        internal EditorWindowController WindowController => windowController;

        internal virtual void OnScreenBecomeActive()
        {
            
        }

        internal virtual void OnScreenFocus()
        {
            
        }

        internal virtual void OnScreenLostFocus()
        {
            
        }
        
        internal virtual void OnSceneOpened(Scene scene)
        {
            
        }
    }
}

#endif
