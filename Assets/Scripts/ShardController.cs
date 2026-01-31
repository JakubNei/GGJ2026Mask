using UnityEngine;
using System.Collections;

public class ShardController : MonoBehaviour
{
    public float explosionForce = 50f;
    public float lifetime = 2f;
    public float fadeSpeed = 1f;

    void Start()
    {
        foreach (Transform shard in transform)
        {
            Rigidbody2D rb = shard.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
// Calculate direction
            Vector2 forceDirection = (shard.position - transform.position).normalized;

            // FALLBACK: If direction is zero (shards are on top of parent), pick a random one
            if (forceDirection == Vector2.zero)
            {
                forceDirection = new Vector2(Random.Range(-1f, 1f), Random.Range(0.1f, 1f)).normalized;
            }

            // Add an upward bias (makes it look like an explosion, not just a slide)
            forceDirection += Vector2.up * 0.5f;

            // Use ForceMode2D.Impulse for an instant "kick"
            rb.AddForce(forceDirection.normalized * explosionForce, ForceMode2D.Impulse);
            rb.AddTorque(Random.Range(-50f, 50f));
            }
        }

        StartCoroutine(FadeAndDestroy());
    }

    private IEnumerator FadeAndDestroy()
    {
        yield return new WaitForSeconds(lifetime);

        SpriteRenderer[] renderers = GetComponentsInChildren<SpriteRenderer>();
        float alpha = 1f;

        while (alpha > 0)
        {
            alpha -= Time.deltaTime * fadeSpeed;
            foreach (var sr in renderers)
            {
                if (sr != null)
                {
                    Color c = sr.color;
                    sr.color = new Color(c.r, c.g, c.b, alpha);
                }
            }
            yield return null;
        }

        Destroy(gameObject);
    }
}