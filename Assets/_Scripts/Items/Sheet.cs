using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sheet : MonoBehaviour
{
    [SerializeField] GameObject ghostCostume;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (OverDog.i.QuestsState == QuestsState.QuestScareGrandma_GaveScissors)
        {
            collision.gameObject.GetComponent<Scissors>().ScissorsUsed();
            Destroy(collision.gameObject);
            SpawnGhostCostume();
        }
    }

    void SpawnGhostCostume()
    {
        Destroy(gameObject);
        Instantiate(ghostCostume, transform.position, Quaternion.identity);
    }
}
