using System;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ShootableButton : MonoBehaviour
{
    [SerializeField] SignalReceiver[] targets;
    [SerializeField] AudioClip activateSound;
    [SerializeField] GameObject configOnVisuals;
    [SerializeField] GameObject configOffVisuals;
    public bool turnedOn = false;

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

        if (projectile != null && lastActivatedProjectile != projectile)
        {
            lastActivatedProjectile = projectile;
            Activate();
        }
    }

    Projectile lastActivatedProjectile;

    void Activate()
    {
        Debug.Log($"[ShootableButton] Activating! Targets count: {targets?.Length ?? 0}");

        if (activateSound && AudioManager.i)
            AudioManager.i.PlaySfx(activateSound);

        Turn(!turnedOn);
    }

    void Turn(bool on)
    {
        if (turnedOn == on)
            return;
        turnedOn = on;
        if (on)
        {
            configOnVisuals.SetActive(true);
            configOffVisuals.SetActive(false);
            targets.ReceiveSignalOn();
        }
        else
        {
            configOnVisuals.SetActive(false);
            configOffVisuals.SetActive(true);
            targets.ReceiveSignalOff();
        }
    }
}
