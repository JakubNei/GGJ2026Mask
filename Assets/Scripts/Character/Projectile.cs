using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;
using System.Collections;

public class Projectile : MonoBehaviour
{
    [SerializeField] float throwForce = 10f;
    [SerializeField] bool rotate = false;

    [Header("Audio")]
    [SerializeField] AudioClip flyingSound;
    [SerializeField] float soundVolume = 1f;
    [SerializeField] float fadeOutDuration = 0.15f;

    Rigidbody2D rb;
    AudioSource audioSource;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        // Setup audio source
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = flyingSound;
        audioSource.volume = soundVolume;
        audioSource.loop = true;
        audioSource.playOnAwake = false;
    }

    void Start()
    {
        var projectileCollider = GetComponent<Collider2D>();
        Debug.Log($"[Projectile] Started, layer: {gameObject.layer} ({LayerMask.LayerToName(gameObject.layer)}), collider: {projectileCollider?.GetType().Name}, isTrigger: {projectileCollider?.isTrigger}");

        // Log collision matrix info
        int myLayer = gameObject.layer;
        for (int i = 0; i < 32; i++)
        {
            string layerName = LayerMask.LayerToName(i);
            if (!string.IsNullOrEmpty(layerName))
            {
                bool ignores = Physics2D.GetIgnoreLayerCollision(myLayer, i);
                if (ignores)
                    Debug.Log($"[Projectile] Layer {myLayer} ({LayerMask.LayerToName(myLayer)}) IGNORES layer {i} ({layerName})");
            }
        }

        // Ignore collision with player
        var playerCollider = PlayerController.Instance.controllingCharacter.GetComponent<Collider2D>();
        if (playerCollider && projectileCollider)
        {
            Physics2D.IgnoreCollision(projectileCollider, playerCollider);
        }
    }

    void Update()
    {
        if(rotate)
        {
            Vector3 currentRotation = transform.eulerAngles;
            currentRotation.z += 1000 * Time.deltaTime;
            transform.rotation = Quaternion.Euler(currentRotation);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log($"[Projectile] OnCollisionEnter2D with {collision.gameObject.name}, layer: {collision.gameObject.layer} ({LayerMask.LayerToName(collision.gameObject.layer)})");
        DestroyWithFadeOut();
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        Debug.Log($"[Projectile] OnTriggerEnter2D with {collider.gameObject.name}, layer: {collider.gameObject.layer} ({LayerMask.LayerToName(collider.gameObject.layer)})");
    }

    public void Throw(Vector2 throwDirection)
    {
        rb.AddForce(throwDirection * throwForce, ForceMode2D.Impulse);
        if (flyingSound != null)
            audioSource.Play();
    }

    void DestroyWithFadeOut()
    {
        StartCoroutine(FadeOutAndDestroy());
    }

    IEnumerator FadeOutAndDestroy()
    {
        float startVolume = audioSource.volume;
        float elapsed = 0f;

        while (elapsed < fadeOutDuration)
        {
            elapsed += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, 0f, elapsed / fadeOutDuration);
            yield return null;
        }

        Destroy(gameObject);
    }
}
