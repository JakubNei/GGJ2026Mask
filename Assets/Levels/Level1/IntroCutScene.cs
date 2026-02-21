using UnityEngine;
using System.Collections;

public class IntroCutScene : MonoBehaviour
{

    [SerializeField] UnityEngine.UI.Image[] introImages;
    [SerializeField] float fadeDuration = 0.5f;

    public static IntroCutScene Instance => FindFirstObjectByType<IntroCutScene>(FindObjectsInactive.Include);

    void Start()
    {
        StartCoroutine(PlayIntro());
    }

    IEnumerator PlayIntro()
    {

        for (int i = 0; i < introImages.Length; i++)
        {
            if (i > 0)
            {
                var image = introImages[i];
                float elapsedTime = 0f;
                Color c = image.color;
                c.a = 0f;
                image.color = c;
                image.gameObject.SetActive(true);

                yield return null;

                while (elapsedTime < fadeDuration && !PlayerController.IsInteractInputKeyDown())
                {
                    elapsedTime += Time.deltaTime;
                    c.a = Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration);
                    image.color = c;
                    yield return null;
                }

                c.a = 1f;
                image.color = c;

                introImages[i - 1].gameObject.SetActive(false);
            }

            // Wait while audio is playing
            AudioSource audioSource = introImages[i].GetComponent<AudioSource>();
            while (audioSource != null && audioSource.isPlaying)
            {
                if (PlayerController.IsInteractInputKeyDown())
                {
                    break;
                }
                yield return null;
            }
        }

        // Allow character movement
        AllowCharacterMovement();


        // Fade out the last image
        {
            var image = introImages[introImages.Length - 1];

            float elapsedTime = 0f;
            Color c = image.color;
            AudioSource audioSource = image.GetComponent<AudioSource>();
            float initialVolume = audioSource != null ? audioSource.volume : 0f;

            while (elapsedTime < fadeDuration)
            {

                elapsedTime += Time.deltaTime;
                float t = elapsedTime / fadeDuration;
                c.a = Mathf.Lerp(1f, 0f, t);
                image.color = c;

                if (audioSource != null)
                {
                    audioSource.volume = Mathf.Lerp(initialVolume, 0f, t);
                }

                yield return null;
            }

            c.a = 0f;
            image.color = c;

            if (audioSource != null)
            {
                audioSource.volume = 0f;
            }

            image.gameObject.SetActive(false);
        }

        gameObject.SetActive(false);

    }



    void ForbidAllCharaterMovement()
    {
        foreach (var c in FindObjectsByType<Character>(FindObjectsSortMode.None))
        {
            c.temporarilyForbidMovement = true;
        }
        PlayerController.Instance.temporarilyBlockInoput = true;
    }

    void AllowCharacterMovement()
    {
        foreach (var c in FindObjectsByType<Character>(FindObjectsSortMode.None))
        {
            c.temporarilyForbidMovement = false;
        }
        PlayerController.Instance.temporarilyBlockInoput = false;
    }
}   
