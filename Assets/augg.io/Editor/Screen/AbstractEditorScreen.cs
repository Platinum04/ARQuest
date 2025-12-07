using UnityEngine.SceneManagement;

#if UNITY_EDITOR
namespace Auggio.Plugin.Editor.Screen
{
    internal abstract class AbstractEditorScreen
    {
        protected AuggioEditorPlugin rootWindow;

        protected AbstractEditorScreen(AuggioEditorPlugin rootWindow)
        {
            this.rootWindow = rootWindow;
            Initialize();
        }

        protected abstract void Initialize();

        internal abstract void Draw();

        internal AuggioEditorPlugin RootWindow => rootWindow;

        internal abstract void OnBecameActive();

        internal abstract void OnFocus();

        internal abstract void OnLostFocus();

        internal abstract void OnSceneOpened(Scene scene);
    }
}
#endif
