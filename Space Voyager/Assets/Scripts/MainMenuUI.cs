using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class MainMenuUI : MonoBehaviour
{
    [Header("Menu Elements")]
    [SerializeField] private RectTransform mainTitle;
    [SerializeField] private RectTransform buttonsPanel;
    [SerializeField] private Button playButton;
    [SerializeField] private Button optionsButton;
    [SerializeField] private Button creditsButton;
    [SerializeField] private Button quitButton;
    
    [Header("Visual Effects")]
    [SerializeField] private ParticleSystem starfieldEffect;
    [SerializeField] private Image backgroundGradient;
    [SerializeField] private float pulseSpeed = 1.5f;
    [SerializeField] private float buttonHoverScale = 1.2f;

    private void Start()
    {
        // Initial setup
        mainTitle.localScale = Vector3.zero;
        buttonsPanel.anchoredPosition = new Vector2(-1000, 0);
        
        // Setup button hover effects
        SetupButtonAnimations(playButton);
        SetupButtonAnimations(optionsButton);
        SetupButtonAnimations(creditsButton);
        SetupButtonAnimations(quitButton);

        // Start animations
        StartCoroutine(AnimateMenuElements());
    }

    private System.Collections.IEnumerator AnimateMenuElements()
    {
        float elapsed = 0;
        float duration = 1f;

        // Animate title scale
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / duration;
            mainTitle.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, progress);
            yield return null;
        }

        // Animate buttons panel
        elapsed = 0;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / duration;
            buttonsPanel.anchoredPosition = Vector2.Lerp(
                new Vector2(-1000, 0),
                Vector2.zero,
                progress
            );
            yield return null;
        }
    }

    private void SetupButtonAnimations(Button button)
    {
        if (button == null) return;

        var eventTrigger = button.gameObject.GetComponent<EventTrigger>() ?? 
                          button.gameObject.AddComponent<EventTrigger>();

        // Hover enter
        var enterEntry = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
        enterEntry.callback.AddListener((data) => {
            button.transform.localScale = Vector3.one * buttonHoverScale;
        });
        eventTrigger.triggers.Add(enterEntry);

        // Hover exit
        var exitEntry = new EventTrigger.Entry { eventID = EventTriggerType.PointerExit };
        exitEntry.callback.AddListener((data) => {
            button.transform.localScale = Vector3.one;
        });
        eventTrigger.triggers.Add(exitEntry);

        // Click feedback
        button.onClick.AddListener(() => {
            StartCoroutine(ButtonClickAnimation(button.transform));
        });
    }

    private System.Collections.IEnumerator ButtonClickAnimation(Transform buttonTransform)
    {
        Vector3 originalScale = buttonTransform.localScale;
        Vector3 targetScale = originalScale * 0.9f;
        
        float elapsed = 0;
        float duration = 0.1f;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / duration;
            buttonTransform.localScale = Vector3.Lerp(originalScale, targetScale, progress);
            yield return null;
        }
        
        elapsed = 0;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / duration;
            buttonTransform.localScale = Vector3.Lerp(targetScale, originalScale, progress);
            yield return null;
        }
    }

    private void Update()
    {
        // Animate background color
        if (backgroundGradient != null)
        {
            float t = (Mathf.Sin(Time.time * pulseSpeed) + 1) * 0.5f;
            Color startColor = new Color(0.1f, 0.2f, 0.4f, 1f);
            Color endColor = new Color(0.2f, 0.3f, 0.5f, 1f);
            backgroundGradient.color = Color.Lerp(startColor, endColor, t);
        }
    }
}
