using UnityEngine;

public class NormalPlane : MonoBehaviour
{
    public static NormalPlane Instance => FindFirstObjectByType<NormalPlane>(FindObjectsInactive.Include);


}
