using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class HoleBlocker : SignalReceiver
{
    [SerializeField] bool startEnabled = true;

    Collider2D col;

    void Awake()
    {
        col = GetComponent<Collider2D>();
        col.enabled = startEnabled;
    }

    void Start()
    {
        Debug.Log($"[HoleBlocker] {gameObject.name} - layer: {gameObject.layer} ({LayerMask.LayerToName(gameObject.layer)}), collider: {col?.GetType().Name}, isTrigger: {col?.isTrigger}, enabled: {col?.enabled}");
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log($"[HoleBlocker] OnCollisionEnter2D with {collision.gameObject.name}, layer: {collision.gameObject.layer} ({LayerMask.LayerToName(collision.gameObject.layer)})");
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"[HoleBlocker] OnTriggerEnter2D with {other.gameObject.name}, layer: {other.gameObject.layer} ({LayerMask.LayerToName(other.gameObject.layer)})");
    }

    public override void OnReceiveSignalOn()
    {
        col.enabled = !col.enabled;
    }
}
