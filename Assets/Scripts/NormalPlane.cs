using UnityEngine;
using UnityEngine.Tilemaps;

public class NormalPlane : MonoBehaviour
{
    public static NormalPlane Instance => FindFirstObjectByType<NormalPlane>(FindObjectsInactive.Include);

    // player is required to be on top of this collider to get back into normal plane
    public TilemapCollider2D RequiredToSwitchInto;

}
