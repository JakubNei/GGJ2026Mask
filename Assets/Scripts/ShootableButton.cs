using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ShootableButton : MonoBehaviour
{
    [SerializeField] SignalReceiver[] targets;
    [SerializeField] AudioClip activateSound;

    void Start()
    {
        var collider = GetComponent<Collider2D>();
        Debug.Log($"[ShootableButton] Started on {gameObject.name}, layer: {gameObject.layer} ({LayerMask.LayerToName(gameObject.layer)}), collider: {collider?.GetType().Name}, isTrigger: {collider?.isTrigger}");
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log($"[ShootableButton] OnCollisionEnter2D with {collision.gameObject.name}");
        TryActivate(collision.gameObject);
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        Debug.Log($"[ShootableButton] OnTriggerEnter2D with {collider.gameObject.name}");
        TryActivate(collider.gameObject);
    }

    void TryActivate(GameObject other)
    {
        var projectile = other.GetComponent<Projectile>();
        Debug.Log($"[ShootableButton] Projectile component: {(projectile != null ? "FOUND" : "NOT FOUND")}");

        if (projectile != null)
        {
            Activate();
        }
    }

    void Activate()
    {
        Debug.Log($"[ShootableButton] Activating! Targets count: {targets?.Length ?? 0}");

        if (activateSound && AudioManager.i)
            AudioManager.i.PlaySfx(activateSound);

        targets.ReceiveSignal();
    }
}
