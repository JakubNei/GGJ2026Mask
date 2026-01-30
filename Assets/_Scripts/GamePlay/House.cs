using System.Collections;
using UnityEngine;

public class House : MonoBehaviour
{
    public GameObject firePrefab;

    [SerializeField] NPCFireController npc1;
    [SerializeField] NPCFireController npc2;


    public IEnumerator SpreadFire()
    {
        AudioManager.i.PlaySfx(AudioId.HouseBurning);

        if (firePrefab != null)
            Instantiate(firePrefab, transform.position + new Vector3(0, -2f, 0), Quaternion.identity);

        var newNpc = Instantiate(npc1, transform.position + new Vector3(0, -2.5f, 0), Quaternion.identity);
        yield return newNpc.WalkOverAndFall();
        yield return new WaitForSeconds(1.5f);
        var newNpc2 = Instantiate(npc2, transform.position + new Vector3(0, -2.5f, 0), Quaternion.identity);
        yield return newNpc2.WalkOverAndFall();
        GameController.Instance.OnHouseBurned();

        if (GameController.Instance.houseBurned >= 3)
            GameController.Instance.OnHouseLit();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (OverDog.i.QuestsState == QuestsState.QuestBurningHouse_Torch_Lit)
            StartCoroutine(SpreadFire());
    }
}
