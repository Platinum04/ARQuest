using UnityEngine;
#if AUGGIO_SDK
using Auggio.SDK;
#elif AUGGIO
using Auggio;
#elif CZ_AUGG_IO_SDK
using cz.augg.io.sdk;
#endif

public class SetAuggioAPIKey : MonoBehaviour
{
    [Header("ARCore Cloud Anchor API Key")]
    [SerializeField] private string apiKey = "AIzaSyBqp-ZHAT20KWUuyUKU09iSGA2LPxAYgLw";

    private void Awake()
    {
        // Try to set the API key if AuggioSettings exists in this SDK
        var settingsType = System.Type.GetType("cz.augg.io.sdk.AuggioSettings, cz.augg.io.sdk")
                         ?? System.Type.GetType("Auggio.AuggioSettings, cz.augg.io.sdk")
                         ?? System.Type.GetType("AuggioSettings, cz.augg.io.sdk");

        if (settingsType != null)
        {
            var instanceProp = settingsType.GetProperty("Instance");
            var setKeyMethod = settingsType.GetMethod("SetARCoreAPIKey");

            if (instanceProp != null && setKeyMethod != null)
            {
                var instance = instanceProp.GetValue(null);
                setKeyMethod.Invoke(instance, new object[] { apiKey });
                Debug.Log("[Augg.io] ARCore API Key set successfully at runtime.");
                return;
            }
        }

        Debug.LogWarning("[Augg.io] Could not find AuggioSettings class — check SDK namespace.");
    }
}
