using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scissors : ItemBase
{
    public void ScissorsUsed()
    {
        OverDog.i.QuestsState = QuestsState.QuestScareGrandma_WaitingForGrandmaScare;
        AudioManager.i.PlaySfx(AudioId.ScissorsCutting);
    }
}
