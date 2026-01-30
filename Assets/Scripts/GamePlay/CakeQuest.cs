using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CakeQuest : MonoBehaviour
{
    [SerializeField] private List<Character> npcs;
    public void StartQuest()
    {
        gameObject.SetActive(true);
    }

    public static void StartQuestt()
    {
        FindObjectOfType<CakeQuest>(true).StartQuest();
    }

    public void QuestDone() {
        npcs[0].SetFallAfterTime(4);
        npcs[1].SetFallAfterTime(8);
        npcs[2].SetFallAfterTime(12);
    }
}
