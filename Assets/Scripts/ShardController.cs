using UnityEngine;
using System.Collections;

public class ShardController : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;

    public void Initialize(Vector2 direction, float speed, float lifetime)
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();

        // Set the initial velocity
        rb.linearVelocity = direction * speed;

        // Start the fading process
        StartCoroutine(FadeAndDestroy(lifetime));
    }

    private IEnumerator FadeAndDestroy(float lifetime)
    {
        float elapsed = 0f;
        Color startColor = spriteRenderer.color;

        while (elapsed < lifetime)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / lifetime);
            spriteRenderer.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
            yield return null;
        }

        Destroy(gameObject);
    }
}