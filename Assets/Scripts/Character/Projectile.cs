using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;

public class Projectile : MonoBehaviour
{
    float throwForce = 1f;
    Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if(PlayerController.Instance.controllingCharacter.gameObject != collision.gameObject) 
        {
            Destroy(gameObject);
        }
    }

    public void Throw(Vector2 throwDirection)
    {
        rb.AddForce(throwDirection * throwForce, ForceMode2D.Impulse);
    }
}
