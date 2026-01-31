using Unity.VisualScripting;
using UnityEngine;

public class AstralPlane : MonoBehaviour
{
    public static AstralPlane Instance => FindFirstObjectByType<AstralPlane>(FindObjectsInactive.Include);

}
