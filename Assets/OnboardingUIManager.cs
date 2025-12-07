using UnityEngine;
using System.Collections;

public class OnboardingManager : MonoBehaviour
{
    [Header("Onboarding UI")]
    [SerializeField] private GameObject onboardingUI;   // your onboarding Image (Canvas/Image)
    [SerializeField] private float displayTime = 4f;     // how long to show the onboarding UI

    [Header("Main AR Content Root")]
    [SerializeField] private GameObject mainARContent;  // parent of all AR elements

    private CanvasGroup canvasGroup;

    void Start()
    {
        // Ensure AR content is off initially
        if (mainARContent != null)
            mainARContent.SetActive(false);

        // Ensure onboarding UI is on
        if (onboardingUI != null)
        {
            onboardingUI.SetActive(true);
            canvasGroup = onboardingUI.GetComponent<CanvasGroup>();
        }

        // Start the transition
        StartCoroutine(HandleOnboarding());
    }

    IEnumerator HandleOnboarding()
    {
        // Optional fade-in (if you have CanvasGroup)
        if (canvasGroup != null)
        {
            for (float t = 0; t < 1f; t += Time.deltaTime)
            {
                canvasGroup.alpha = Mathf.Lerp(0f, 1f, t);
                yield return null;
            }
        }

        // Wait for the display time
        yield return new WaitForSeconds(displayTime);

        // Optional fade-out
        if (canvasGroup != null)
        {
            for (float t = 0; t < 1f; t += Time.deltaTime)
            {
                canvasGroup.alpha = Mathf.Lerp(1f, 0f, t);
                yield return null;
            }
        }

        // Hide onboarding and enable AR content
        if (onboardingUI != null)
            onboardingUI.SetActive(false);

        if (mainARContent != null)
            mainARContent.SetActive(true);
    }
}
