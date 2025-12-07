#if UNITY_EDITOR
using System.Collections.Generic;
using Auggio.Plugin.Editor.Screen;
using Auggio.Plugin.Editor.Screen.Controller;
using Auggio.Plugin.Screen.Controller;

namespace Auggio.Plugin.Editor
{
    internal class EditorWindowController
    {
        private AuggioEditorPlugin rootWindow;
        private LoginScreen loginScreen;
        private ExperienceScreen experienceScreen;
        private ExperienceDetailScreen experienceDetailScreen;
        private SettingsScreen settingsScreen;
        private LoginSettingsScreen loginSettingsScreen;
        
        private Stack<AbstractEditorScreen> previousScreens;
        private AbstractEditorScreen currentScreen;

        private bool loading;
        private bool focused;
        private string loadingText;
    
        public EditorWindowController(AuggioEditorPlugin rootWindow)
        {
            this.rootWindow = rootWindow;
            previousScreens = new Stack<AbstractEditorScreen>();
            loginScreen = new LoginScreen(rootWindow, new LoginScreenController(this), false, false);
            experienceScreen = new ExperienceScreen(rootWindow, new ExperienceScreenController(this), false, true);
            experienceDetailScreen = new ExperienceDetailScreen(rootWindow, new ExperienceDetailScreenController(this), true, true);
            settingsScreen = new SettingsScreen(rootWindow, new SettingsScreenController(this), true, false);
            loginSettingsScreen =
                new LoginSettingsScreen(rootWindow, new LoginSettingsScreenController(this), true, false);
            SetCurrentScreen(loginScreen);
        }
    
        public void SetCurrentScreen(AbstractEditorScreen screen)
        {
            if (currentScreen != null)
            {
                previousScreens.Push(currentScreen);
            }
            currentScreen = screen;
            currentScreen.OnBecameActive();
        }
        
        public void SetLoading(bool loading, string loadingText = null)
        {
            this.loading = loading;
            this.loadingText = loadingText;
            rootWindow.Repaint();
        }

        public void SetWindowFocused(bool f)
        {
            this.focused = f;
            rootWindow.Repaint();
        }

        public void GoBack()
        {
            currentScreen = previousScreens.Pop();
            currentScreen.OnBecameActive();
        }

        public AbstractEditorScreen CurrentScreen => currentScreen;

        public LoginScreen LoginScreen => loginScreen;

        public SettingsScreen SettingsScreen => settingsScreen;

        public ExperienceScreen ExperienceScreen => experienceScreen;

        public ExperienceDetailScreen ExperienceDetailScreen => experienceDetailScreen;

        public LoginSettingsScreen LoginSettingsScreen => loginSettingsScreen;

        public AuggioEditorPlugin RootWindow => rootWindow;

        public bool Loading => loading;

        public string LoadingText => loadingText;

        public bool Focused => focused;

        public bool HasPreviousScreens()
        {
            return previousScreens != null && previousScreens.Count > 0;
        }

    }
}
#endif
