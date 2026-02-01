using UnityEngine;
using System.Collections;

public class IntroCutScene : MonoBehaviour
{

    [SerializeField] UnityEngine.UI.Image[] introImages;
    [SerializeField] float fadeDuration = 1f;
    [SerializeField] float displayDuration = 10f;

    void Start()
    {
        ForbidAllCharaterMovement();
        StartCoroutine(PlayIntroCutscene());
    }

    IEnumerator PlayIntroCutscene()
    {
        // Initialize all images to transparent
        foreach (var image in introImages)
        {
            Color c = image.color;
            c.a = 0f;
            image.color = c;
            image.gameObject.SetActive(false);
        }

        // Cycle through each image
        for (int i = 0; i < introImages.Length; i++)
        {
            // Fade in the current image
            yield return StartCoroutine(FadeInImage(introImages[i]));

            // Wait for timer or skip input
            float elapsedTime = 0f;
            while (elapsedTime < displayDuration)
            {
                if (PlayerController.IsInteractInputKeyDown())
                {
                    break;
                }
                elapsedTime += Time.deltaTime;
                yield return null;
            }
        }

        // Hide all images
        foreach (var image in introImages)
        {
            image.gameObject.SetActive(false);
        }

        // Allow character movement
        AllowCharacterMovement();
    }

    IEnumerator FadeInImage(UnityEngine.UI.Image image)
    {
        float elapsedTime = 0f;
        Color c = image.color;
        
        while (elapsedTime < fadeDuration)
        {
            if (PlayerController.IsInteractInputKeyDown())
            {
                // Skip fade and set to full opacity immediately
                c.a = 1f;
                image.color = c;
                yield break;
            }
            
            elapsedTime += Time.deltaTime;
            c.a = Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration);
            image.color = c;
            yield return null;
        }

        c.a = 1f;
        image.color = c;
    }
 
    void ForbidAllCharaterMovement()
    {

    }

    void AllowCharacterMovement()
    {
        
    }
}   
