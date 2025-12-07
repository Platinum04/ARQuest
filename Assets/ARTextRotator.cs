using UnityEngine;
using TMPro;  // Make sure TextMeshPro is imported
using System.Collections;

public class ARTextRotator : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textDisplay; // assign in Inspector
    [SerializeField] private float updateInterval = 3f;   // seconds between updates

    // List of messages to rotate through
    [TextArea]
    public string[] messages = new string[]
    {
        "?? Welcome to Ilorin Innovation Hub!",
        "?? Did you know? The hub empowers over 100 young innovators every year.",
        "?? Tap to explore the AR quiz and earn a badge!",
        "?? Fun fact: This space was designed for collaboration and creativity.",
        "?? AR is changing the way we experience the world — explore and learn!"
    };

    private int currentIndex = 0;

    void Start()
    {
        if (textDisplay == null)
        {
            Debug.LogError("?? TextMeshProUGUI reference not assigned!");
            return;
        }

        // Start looping text updates
        StartCoroutine(UpdateTextLoop());
    }

    IEnumerator UpdateTextLoop()
    {
        while (true)
        {
            textDisplay.text = messages[currentIndex];
            currentIndex = (currentIndex + 1) % messages.Length; // loop back to start
            yield return new WaitForSeconds(updateInterval);
        }
    }
}
