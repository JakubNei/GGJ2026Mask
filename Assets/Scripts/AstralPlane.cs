using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class AstralPlane : MonoBehaviour
{
    public static AstralPlane Instance => FindFirstObjectByType<AstralPlane>(FindObjectsInactive.Include);

    // player is required to be on top of this collider to get into astral plane
    public TilemapCollider2D RequiredToSwitchInto;
}
