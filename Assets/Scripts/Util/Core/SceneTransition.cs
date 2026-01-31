using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneTransition : MonoBehaviour
{
    public static SceneTransition Instance { get; private set; }

    [Header("Settings")]
    [SerializeField] float fadeDuration = 0.5f;

    [Header("Fade Canvas")]
    [SerializeField] Image fadeImage;

    Canvas fadeCanvas;
    bool isTransitioning;

    void Awake()
    {
        Instance = this;

        // Create fade canvas if not assigned
        if (fadeImage == null)
            CreateFadeCanvas();
        else
            fadeCanvas = fadeImage.GetComponentInParent<Canvas>();
    }

    void CreateFadeCanvas()
    {
        // Create canvas
        var canvasGO = new GameObject("FadeCanvas");
        canvasGO.transform.SetParent(transform);

        fadeCanvas = canvasGO.AddComponent<Canvas>();
        fadeCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        fadeCanvas.sortingOrder = 9999; // Always on top

        canvasGO.AddComponent<CanvasScaler>();
        canvasGO.AddComponent<GraphicRaycaster>();

        // Create fade image
        var imageGO = new GameObject("FadeImage");
        imageGO.transform.SetParent(canvasGO.transform);

        fadeImage = imageGO.AddComponent<Image>();
        fadeImage.color = new Color(0, 0, 0, 0); // Start transparent

        // Stretch to fill
        var rt = fadeImage.rectTransform;
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
    }

    /// <summary>
    /// Load a scene with smooth fade transition
    /// </summary>
    public void LoadScene(string sceneName)
    {
        if (isTransitioning) return;
        StartCoroutine(TransitionToScene(sceneName));
    }

    /// <summary>
    /// Load a scene by build index with smooth fade transition
    /// </summary>
    public void LoadScene(int sceneIndex)
    {
        if (isTransitioning) return;
        StartCoroutine(TransitionToSceneByIndex(sceneIndex));
    }

    IEnumerator TransitionToScene(string sceneName)
    {
        isTransitioning = true;

        // Fade to black
        yield return FadeOut();

        // Load scene
        yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);

        // Fade in
        yield return FadeIn();

        isTransitioning = false;
    }

    IEnumerator TransitionToSceneByIndex(int sceneIndex)
    {
        isTransitioning = true;

        // Fade to black
        yield return FadeOut();

        // Load scene
        yield return SceneManager.LoadSceneAsync(sceneIndex, LoadSceneMode.Single);

        // Fade in
        yield return FadeIn();

        isTransitioning = false;
    }

    IEnumerator FadeOut()
    {
        // Fade ALL audio via AudioListener (affects everything including ambience, boulder, etc.)
        DOTween.To(() => AudioListener.volume, x => AudioListener.volume = x, 0f, fadeDuration);

        // Fade screen to black
        fadeImage.raycastTarget = true; // Block input during transition
        yield return fadeImage.DOFade(1f, fadeDuration).SetUpdate(true).WaitForCompletion();
    }

    IEnumerator FadeIn()
    {
        // Small delay after load for scene to initialize
        yield return null;

        // Fade ALL audio back in
        DOTween.To(() => AudioListener.volume, x => AudioListener.volume = x, 1f, fadeDuration);

        // Fade screen from black
        yield return fadeImage.DOFade(0f, fadeDuration).SetUpdate(true).WaitForCompletion();
        fadeImage.raycastTarget = false; // Re-enable input
    }

    /// <summary>
    /// Fade screen to black without loading a scene (useful for custom transitions)
    /// </summary>
    public Coroutine FadeToBlack()
    {
        return StartCoroutine(FadeOut());
    }

    /// <summary>
    /// Fade screen from black (useful for custom transitions)
    /// </summary>
    public Coroutine FadeFromBlack()
    {
        return StartCoroutine(FadeIn());
    }
}
