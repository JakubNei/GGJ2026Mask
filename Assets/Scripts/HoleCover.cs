using UnityEngine;
using UnityEngine.Tilemaps;

public class HoleCover : SignalReceiver
{
    [SerializeField] AudioClip activateSound;
    [SerializeField] Collider2D[] holeColliders; // Colliders that block when hole is exposed

    TilemapRenderer tilemapRenderer;
    TilemapCollider2D tilemapCollider;
    Tilemap tilemap;
    bool triggered = false;

    void Awake()
    {
        tilemapRenderer = GetComponent<TilemapRenderer>();
        tilemapCollider = GetComponent<TilemapCollider2D>();
        tilemap = GetComponent<Tilemap>();

        Debug.Log($"[HoleCover] Awake on {name}: TilemapRenderer={tilemapRenderer != null}, TilemapCollider={tilemapCollider != null}, Tilemap={tilemap != null}");

        if (tilemap != null)
        {
            var bounds = tilemap.cellBounds;
            int tileCount = 0;
            foreach (var pos in bounds.allPositionsWithin)
            {
                if (tilemap.HasTile(pos)) tileCount++;
            }
            Debug.Log($"[HoleCover] Tilemap bounds: {bounds}, tile count: {tileCount}");
        }

        // Hide but keep GameObject active so Tilemap stays initialized
        if (tilemapRenderer != null)
            tilemapRenderer.enabled = false;
        if (tilemapCollider != null)
            tilemapCollider.enabled = false;

        // Hole colliders start ENABLED (hole is exposed at start)
        SetHoleCollidersEnabled(true);
    }

    void SetHoleCollidersEnabled(bool enabled)
    {
        if (holeColliders == null) return;
        foreach (var col in holeColliders)
        {
            if (col != null)
                col.enabled = enabled;
        }
    }

    void Update()
    {
        if (IsReceivingSignal)
        {
            if (!triggered)
            {
                Trigger();
                triggered = true;
            }
        }
        else
        {
            if (triggered)
            {
                Trigger();
                triggered = false;
            }
        }
    }

    public void Trigger()
    {
        // Get components if Awake didn't run (object was inactive)
        if (tilemapRenderer == null)
            tilemapRenderer = GetComponent<TilemapRenderer>();
        if (tilemapCollider == null)
            tilemapCollider = GetComponent<TilemapCollider2D>();

        // First activate the GameObject if needed
        if (!gameObject.activeSelf)
            gameObject.SetActive(true);

        // Toggle
        bool show = tilemapRenderer == null || !tilemapRenderer.enabled;

        if (tilemapRenderer != null)
            tilemapRenderer.enabled = show;
        if (tilemapCollider != null)
            tilemapCollider.enabled = show;

        // Hole colliders toggle INVERSELY - disabled when cover is shown
        SetHoleCollidersEnabled(!show);

        if (activateSound && AudioManager.i)
            AudioManager.i.PlaySfx(activateSound);
    }
}
