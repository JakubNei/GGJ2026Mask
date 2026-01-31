using UnityEngine;
using UnityEngine.Tilemaps;

public class HoleCover : MonoBehaviour
{
    [SerializeField] AudioClip activateSound;

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

        if (activateSound && AudioManager.i)
            AudioManager.i.PlaySfx(activateSound);
    }
}
