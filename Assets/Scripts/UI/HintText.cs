using UnityEngine;

public class HintText : MonoBehaviour
{
    public static HintText Instance { get; private set; }

    int FrramesToHide = 0;

    // Cached components
    private TMPro.TextMeshProUGUI textComponent;
    private Canvas canvas;
    private RectTransform rectTransform;

    private void Awake()
    {
        Instance = this;

        // Cache components
        textComponent = GetComponentInChildren<TMPro.TextMeshProUGUI>();
        canvas = GetComponentInParent<Canvas>();
        rectTransform = GetComponent<RectTransform>();

        textComponent.text = "";
    }

    public static void ShowHint(string text, Vector2 worldPosition)
    {
        Instance?.ShowHintImpl(text, worldPosition);
    }

    void Update()
    {
        if (FrramesToHide > 0)
        {
            FrramesToHide--;
            if (FrramesToHide == 0)
            {
                textComponent.text = "";
            }
        }
    }

    void ShowHintImpl(string text, Vector3 worldPosition)
    {
        textComponent.text = text;

        // Convert world position to screen position
        Vector2 screenPosition = Camera.main.WorldToScreenPoint(worldPosition);

        // Convert screen position to canvas position
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            screenPosition,
            canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : Camera.main,
            out Vector2 canvasPosition
        );

        // Set the position
        rectTransform.anchoredPosition = canvasPosition;

        FrramesToHide = 2;
    }

}
