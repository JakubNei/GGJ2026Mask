using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class OverDog : MonoBehaviour
{
    [SerializeField] Animator cuteDogAnimator;
    [SerializeField] GameObject dogSprite;
    [Serializable]
    public class DogEvilStateData
    {
        public DogEvilState state;
        public Sprite sprite;
        public float UIHolderOffset; // offset Y
        public float UIHolderOffsetX;
    }

    [SerializeField] GameObject UIHolder;
    [SerializeField] List<DogEvilStateData> dogEvilStates;

    [SerializeField] PlayerController playerController;

    [Serializable]
    public class SpeechBubbleData
    {
        public DogSpeechBubble speechBubble;
        public Sprite sprite;
        public GameObject gameObject;
    }
    [SerializeField] List<SpeechBubbleData> speechBubbleData;

    // instance
    public static OverDog i { get; private set; }

    [Header("Movement")]
    [SerializeField] List<Vector2> movementPattern;
    [SerializeField] float timeBetweenPattern;

    NPCState state;
    float idleTimer = 0f;
    int currentPattern = 0;
    Character character;

    public QuestsState QuestsState
    {
        get => questsState;
        set 
        {
            timeSpentInCurrentQuestState = 0;
            if (hintShownForCurrentQuestState)
                ChangeSpeechBubble(DogSpeechBubble.Hide);
            hintShownForCurrentQuestState = false;
            questsState = value;
        }
    }
    QuestsState questsState;
    float timeSpentInCurrentQuestState;

    bool hintShownForCurrentQuestState;


    void Awake()
    {
        character = GetComponent<Character>();
        if (!playerController)
            playerController = FindObjectOfType<PlayerController>();
        i = this;
    }

    private void Update()
    {
        timeSpentInCurrentQuestState += Time.deltaTime;

        if (!hintShownForCurrentQuestState && playerController/* && !playerController.isInteractingWithGuard*/)
        {
            if (timeSpentInCurrentQuestState > 40 + UnityEngine.Random.Range(0, 20))
            {
                hintShownForCurrentQuestState = true;

                if (QuestsState == QuestsState.CantMakeVillagersLaughLetsTryDog)
                {
                    GameController.Instance.ShowDialogThenFreeRoam("Jester: Maybe I can make the dog laugh!");
                }
                else if (GetHintForQuestState().HasValue)
                {
                    ChangeSpeechBubble(GetHintForQuestState().Value);
                    GameController.Instance.ShowDialogThenFreeRoam("Jester: The dog might show me what to do.");
                }
            }
        }
    }

    DogSpeechBubble? GetHintForQuestState()
    {
        if (QuestsState == QuestsState.QuestBanana_GaveBanana)
        {
            return DogSpeechBubble.BananaPlusVillager;
        }
        else if (QuestsState == QuestsState.QuestPoisonCake_GavePoison_WaitingForPeopleToDie)
        {
            return DogSpeechBubble.PoisonPlusCake;
        }
        else if (QuestsState == QuestsState.QuestScareGrandma_GaveScissors)
        {
            return DogSpeechBubble.ScissorsPlusSheet;
        }
        else if (QuestsState == QuestsState.QuestScareGrandma_WaitingForGrandmaScare)
        {
            return DogSpeechBubble.GhostCostumePlusGrandma;
        }
        else if (QuestsState == QuestsState.QuestBurningHouse)
        {
            return DogSpeechBubble.TorchPlusCake;
        }
        else if (QuestsState == QuestsState.QuestBurningHouse_TorchTaken)
        {
            return DogSpeechBubble.TorchPlusCake;
        }
        else if (QuestsState == QuestsState.QuestBurningHouse_Torch_Lit)
        {
            return DogSpeechBubble.TorchPlusHouse;
        }
        return null;
    }

    // void DetermineIfShouldPutSpeechBubbleHigher(GameObject bubble)
    // {
    //     if (QuestsState == QuestsState.QuestScareGrandma_Finished)
    //     {
    //         bubble.transform.position = new Vector3(transform.position.x, transform.position.y + 0.7f, transform.position.z);
    //     }
    //     else if (QuestsState == QuestsState.QuestPoisonCake_GavePoison_WaitingForPeopleToDie)
    //     {
    //         bubble.transform.position = new Vector3(transform.position.x, transform.position.y + 0.7f, transform.position.z);
    //     }
    //     else if (QuestsState == QuestsState.QuestPoisonCake_Finished_PeopleDied)
    //     {
    //         bubble.transform.position = new Vector3(transform.position.x, transform.position.y + 0.7f, transform.position.z);
    //     }
    //     else if (QuestsState == QuestsState.QuestBurningHouse)
    //     {
    //         bubble.transform.position = new Vector3(transform.position.x, transform.position.y + 1.2f, transform.position.z);
    //     }
    //     else if (QuestsState == QuestsState.QuestBurningHouse_TorchTaken)
    //     {
    //         bubble.transform.position = new Vector3(transform.position.x, transform.position.y + 1.2f, transform.position.z);
    //     }
    //     else if (QuestsState == QuestsState.QuestBurningHouse_Torch_Lit)
    //     {
    //         bubble.transform.position = new Vector3(transform.position.x, transform.position.y + 1.2f, transform.position.z);
    //     }
    //     else if (QuestsState == QuestsState.QuestBurningHouse_Finished)
    //     {
    //         bubble.transform.position = new Vector3(transform.position.x, transform.position.y + 1.2f, transform.position.z);
    //     }
    // }


    public void SetSprite(DogEvilState state)
    {
        var dogSpriteData = dogEvilStates.FirstOrDefault(d => d.state == state);
        if (dogSpriteData != null)
        {
            UIHolder.transform.localPosition = new Vector3(dogSpriteData.UIHolderOffsetX, dogSpriteData.UIHolderOffset, 0);
            var comp = dogSprite.GetComponent<SpriteRenderer>();
            if (comp)
            {
                comp.sprite = dogSpriteData.sprite;
            }
        }

        cuteDogAnimator.enabled = state == DogEvilState.CuteHappy;
    }

    public void ChangeSpeechBubble(DogSpeechBubble b)
    {
        foreach (var s in speechBubbleData)
        {
            s.gameObject.SetActive(false);
        }

        var d = speechBubbleData.FirstOrDefault(d => d.speechBubble == b);
        if (d != null)
        {
            //DetermineIfShouldPutSpeechBubbleHigher(d.gameObject);
            d.gameObject.SetActive(true);
            //d.gameObject.transform.localPosition = Vector3.zero;
            var spriteRenderer = d.gameObject.GetComponent<SpriteRenderer>();
            if (d.sprite && spriteRenderer)
                spriteRenderer.sprite = d.sprite;
        }
    }

    public bool CanInteract()
    {
        return true;
    }

    public IEnumerator Interact(Transform initiator)
    {
        GameController.Instance.OnPlayerInteractedWithOverDog();
        yield return null;
    }
    
    public void EatPlayer()
    {
        SetSprite(DogEvilState.DemonHappy);
        GetComponentInChildren<DemonDogEatingAnimation>(true)?.EatTarget(PlayerController.Instance.Character.gameObject);
    }

    #region For endgame
    void StartMovement()
    {
        ChangeSpeechBubble(DogSpeechBubble.Hide);

        if (character.IsAbleToMove && state == NPCState.Idle)
        {
            idleTimer += Time.deltaTime;
            if (idleTimer > timeBetweenPattern)
            {
                idleTimer = 0f;
                if (movementPattern.Count > 0)
                {
                    StartCoroutine(Walk());
                }
            }
        }

        character.HandleUpdate();
    }
    IEnumerator Walk()
    {
        state = NPCState.Walking;

        var oldPos = transform.position;

        yield return character.Move(movementPattern[currentPattern]);

        if (transform.position != oldPos)
            currentPattern = (currentPattern + 1) % movementPattern.Count;

        state = NPCState.Idle;
    }


    #endregion
}
public enum QuestsState
{
    GameStart,

    QuestMakeVillagersLaugh,
    QuestMakeVillagersLaugh_Finished,
    CantMakeVillagersLaughLetsTryDog,

    QuestBanana_GaveBanana,
    QuestBanana_Finished_SeenPeopleFallNoLongerInterested,

    QuestScareGrandma_GaveScissors,
    QuestScareGrandma_WaitingForGrandmaScare,
    QuestScareGrandma_Finished,

    QuestPoisonCake_GavePoison_WaitingForPeopleToDie,
    QuestPoisonCake_Finished_PeopleDied,

    QuestBurningHouse,
    QuestBurningHouse_TorchTaken,
    QuestBurningHouse_Torch_Lit,
    QuestBurningHouse_Finished,

    EndGame_HousesBurned,
    EndGame_YouAreFunAfterAll,
    EndGame_ComeLetMeRewardYou,
}

// add new to bottom, used in setting
public enum DogEvilState
{
    CuteSad,
    CuteHappy,
    BigSad,
    BigHappy,
    DemonSad,
    DemonHappy,
}

// add new to bottom, used in setting
public enum DogSpeechBubble
{
    Hide,
    Happy,
    Sad,
    ExclamationMark,
    BananaPlusVillager,
    PoisonPlusCake,
    TorchPlusCake,
    TorchPlusHouse,
    ScissorsPlusSheet,
    GhostCostumePlusGrandma,
}