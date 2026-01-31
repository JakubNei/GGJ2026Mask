using UnityEngine;

public class Vase : MonoBehaviour
{
    public GameObject fragmentedVasePrefab;

void Start(){
    Break();
}
    public void Break()
    {
        Instantiate(fragmentedVasePrefab, transform.position, transform.rotation);
        
        Destroy(gameObject);
    }
}