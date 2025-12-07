using UnityEngine;
using UnityEngine.UI;

namespace Auggio.Plugin.SDK
{
    public class DebugMessage : MonoBehaviour {
        [SerializeField] private Text text;

        public void Bind(string message) {
            text.text = message;
        }
    }
}
