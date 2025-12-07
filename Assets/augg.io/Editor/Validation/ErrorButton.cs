#if UNITY_EDITOR
using System;

namespace Auggio.Plugin.Editor.Validation
{
    public class ErrorButton
    {
        private string buttonText;
        private Action onClick;

        public ErrorButton(string buttonText, Action onClick)
        {
            this.buttonText = buttonText;
            this.onClick = onClick;
        }

        public string ButtonText
        {
            get => buttonText;
            set => buttonText = value;
        }

        public Action OnClick
        {
            get => onClick;
            set => onClick = value;
        }
    }
}
#endif
