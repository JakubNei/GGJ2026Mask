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
        // Ignore collision with player
        var playerCollider = PlayerController.Instance.controllingCharacter.GetComponent<Collider2D>();
        var projectileCollider = GetComponent<Collider2D>();
        if (playerCollider && projectileCollider)
        {
            Physics2D.IgnoreCollision(projectileCollider, playerCollider);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Destroy(gameObject);
    }

    public void Throw(Vector2 throwDirection)
    {
        rb.AddForce(throwDirection * throwForce, ForceMode2D.Impulse);
    }
}
