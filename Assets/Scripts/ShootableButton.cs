using System;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ShootableButton : MonoBehaviour, IInteractable
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
            Turn(true);
        }
    }

    Projectile lastActivatedProjectile;

    void Toggle()
    {
        Turn(!turnedOn);
    }

    void Turn(bool on)
    {
        if (turnedOn == on)
            return;

        if (activateSound && AudioManager.i)
            AudioManager.i.PlaySfx(activateSound);

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

    public bool CanPickUp()
    {
        return false;
    }

    public int lastFrameInteracted = 0;
    public void UpdateWhileInteracting()
    {
        if (lastFrameInteracted < Time.frameCount - 2)
        {
            Toggle();
        }
        
        lastFrameInteracted = Time.frameCount;
    }

    public bool CanInteract()
    {
        return true;
    }
}
