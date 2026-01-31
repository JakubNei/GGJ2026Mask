using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;

public class Projectile : MonoBehaviour
{
    [SerializeField] float throwForce = 10f;
    Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
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

    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log($"[Projectile] OnCollisionEnter2D with {collision.gameObject.name}, layer: {collision.gameObject.layer} ({LayerMask.LayerToName(collision.gameObject.layer)})");
        Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        Debug.Log($"[Projectile] OnTriggerEnter2D with {collider.gameObject.name}, layer: {collider.gameObject.layer} ({LayerMask.LayerToName(collider.gameObject.layer)})");
    }

    public void Throw(Vector2 throwDirection)
    {
        rb.AddForce(throwDirection * throwForce, ForceMode2D.Impulse);
    }
}
