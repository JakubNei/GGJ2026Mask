using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
    EvilDogSideview,
    FreeRoam,
    Menu,
    Paused,
    Knocked,
}

public class GameController : MonoBehaviour
{
    [SerializeField] PlayerController playerController;
    [SerializeField] Camera worldCamera;
    [SerializeField] GameObject prefabItemBanana;
    [SerializeField] GameObject prefabItemPoison;
    [SerializeField] GameObject prefabItemScissors;
    [SerializeField] GameObject prefabItemTorch;
    [SerializeField] GameObject evilDogSideviewUI;
    [SerializeField] OverDog overdog;

    [Header("Quests")]
    [SerializeField] GameObject quest2;
    [SerializeField] GameObject quest3;
    [SerializeField] GameObject quest4;

    [SerializeField] List<AudioClip> happySounds;

    public GameState gameState = GameState.FreeRoam;
    GameState prevState;

    public int NumNPCSPoisoned = 0;
    public int houseBurned = 0;
    public int numBadJokesToldToVillagers = 0;

    MenuController menuController;

    public static GameController Instance { get; private set; }

    [System.Serializable] // for debug
    public class DialogPool
    {
        List<string> currentPool = new();
        List<string> defaultPool = new();
        public void Add(string line)
        {
            defaultPool.Add(line);
        }
        public string TakeRandomLine()
        {
            if (currentPool.Count == 0 || currentPool.Count < defaultPool.Count * 0.2)
                RefillPool();

            int index = UnityEngine.Random.Range(0, currentPool.Count);
            return TakeJokeAtIndex(index);
        }

        void RefillPool()
        {
            foreach (var d in defaultPool)
            {
                if (!currentPool.Contains(d))
                    currentPool.Add(d);
            }
        }

        public int[] GetRandomUniqueJokesIndexes(int numJokes)
        {
            if (currentPool.Count < numJokes)
                RefillPool();
            
            if (numJokes > currentPool.Count)
                numJokes = currentPool.Count;

            int[] uniqueJokeIndexes = new int[numJokes];
            for (int i = 0; i < uniqueJokeIndexes.Length; i++)
            {
                bool isDuplicate = false;
                do
                {
                    uniqueJokeIndexes[i] = UnityEngine.Random.Range(0, currentPool.Count);
                    isDuplicate = false;
                    for (int j = 0; j < i; j++)
                    {
                        if (uniqueJokeIndexes[i] == uniqueJokeIndexes[j])
                        {
                            isDuplicate = true;
                            break;
                        }
                    }
                } while (isDuplicate);
            }
            return uniqueJokeIndexes;
        }

        public string PeekJokeAtIndex(int index)
        {
            if (index >= currentPool.Count)
                return "How does the ocean say hi? It waves."; // error
            return currentPool[index];
        }

        public string TakeJokeAtIndex(int index)    
        {
            if (index >= currentPool.Count)
                return "How does the ocean say hi? It waves."; // error
            var line = currentPool[index];
            currentPool.RemoveAt(index);
            return line;
        }
    }


    public DialogPool badJokesJesterPool = new();
    public DialogPool badJokesVillagerResponsesPool = new();
    public DialogPool villagerDoesNotWantToTalkToYouPool = new();
    public DialogPool dogQuestInProgressDialogsPool = new();



    private void Awake()
    {
        Instance = this;

        DOTween.Init();

        overdog = FindObjectOfType<OverDog>(true);

        menuController = FindObjectOfType<MenuController>(true);

        badJokesJesterPool.Add("My friends say I'm stingy, but I'm not buying it.");
        badJokesJesterPool.Add("I fell through the glass doors of a French bakery and now I'm in a world of pain.");
        badJokesJesterPool.Add("I bought my friend an elephant for his room. When he said, \"Thanks\" I told him not to mention it.");
        badJokesJesterPool.Add("Vampires aren’t real. Unless you count Dracula.");
        badJokesJesterPool.Add("My boss has threatened to sack the employee with the worst posture. I have a hunch, it might be me...");
        badJokesJesterPool.Add("I joined a Carpenter's class the other day. We haven't made anything yet. We've only just begun.");
        badJokesJesterPool.Add("I can't believe they fired me from the clock factory! After all the extra hours I put in...");
        badJokesJesterPool.Add("How does a moon cut his hair? Eclipse it");
        badJokesJesterPool.Add("Why are snails slow? Because they’re carrying a house on their back.");
        badJokesJesterPool.Add("How does the ocean say hi? It waves!");
        badJokesJesterPool.Add("Name the kind of tree you can hold in your hand? A palm tree!");
        badJokesJesterPool.Add("What did the left eye say to the right eye? Between us, something smells!");
        badJokesJesterPool.Add("What did the lava say to his girlfriend? “I lava you!”");
        badJokesJesterPool.Add("Sandy’s mum has four kids; North, West, East.What is the name of the fourth child? Sandy, obviously!");
        badJokesJesterPool.Add("What do you call a magic dog? A labracadabrador.");
        badJokesJesterPool.Add("What do cows say when they hear a bad joke? “I am not amoosed.”");
        badJokesJesterPool.Add("Why do French people eat snails? They don’t like fast food.");
        badJokesJesterPool.Add("Why don’t the circus lions eat the clowns? Because they taste funny!");
        badJokesJesterPool.Add("Why couldn’t the leopard play hide-and-seek? Because he was always spotted.");

        badJokesVillagerResponsesPool.Add("Leave me alone.");
        badJokesVillagerResponsesPool.Add("My daughter makes better jokes than that, and all she can say is 'gaga'.");
        badJokesVillagerResponsesPool.Add("What? Please go do something useful.");
        badJokesVillagerResponsesPool.Add("That must be the worst joke I've ever heard.");
        badJokesVillagerResponsesPool.Add("Why are you bothering me?");  
        badJokesVillagerResponsesPool.Add("I feel like something just died inside me.");
        badJokesVillagerResponsesPool.Add("Am I supposed to laugh now?");
        badJokesVillagerResponsesPool.Add("That must be the worst joke I've ever heard.");
        badJokesVillagerResponsesPool.Add("I was wrong! This is the worst joke I've ever heard!");
        badJokesVillagerResponsesPool.Add("Stop bothering me.");
        badJokesVillagerResponsesPool.Add("Your jokes are terrible. You are a joke!");
        badJokesVillagerResponsesPool.Add("No. Just... no.");
        badJokesVillagerResponsesPool.Add("What are you talking about? Oh wait, that was a joke?");
        badJokesVillagerResponsesPool.Add("Go away please. You're making me uncomfortable.");
        badJokesVillagerResponsesPool.Add("I'd pretend to laugh, but I'm afraid that would encourage you.");
        badJokesVillagerResponsesPool.Add("Every time you tell a joke, God kills a kitten.");

        villagerDoesNotWantToTalkToYouPool.Add("No. Just... no.");
        villagerDoesNotWantToTalkToYouPool.Add("Leave me alone.");
        villagerDoesNotWantToTalkToYouPool.Add("What? Please go do something useful.");
        villagerDoesNotWantToTalkToYouPool.Add("Please go away.");
        villagerDoesNotWantToTalkToYouPool.Add("Stop bothering me.");

        dogQuestInProgressDialogsPool.Add("Do your job!");
        dogQuestInProgressDialogsPool.Add("You have things to do.");
        dogQuestInProgressDialogsPool.Add("Make me laugh!");
        dogQuestInProgressDialogsPool.Add("Don't you have something to do?");
        dogQuestInProgressDialogsPool.Add("Do I have to do everything myself?");
    }

    private void Start()
    {
        overdog.SetSprite(DogEvilState.CuteSad);

    
        ShowDialogThenFreeRoam("Jester: Let's make the villagers laugh!", () =>
        {
            overdog.QuestsState = QuestsState.QuestMakeVillagersLaugh;
        });
    }

   
    public void OnGenericNPCTalkedTo()
    {
        if (overdog.QuestsState == QuestsState.QuestMakeVillagersLaugh)
        {
            StartCoroutine(TellRandomBadJokeToVillager());
        }
        else
        {
            DialogManager.Instance.QueueDialogToShow("Villager: " + villagerDoesNotWantToTalkToYouPool.TakeRandomLine());            
        }
    }

    public IEnumerator TellPlayerSelectedBadJoke()
    {
        int[] jokeIndexes = badJokesJesterPool.GetRandomUniqueJokesIndexes(3);

        var dialog1 = new DialogManager.DialogData();
        for (int i = 0; i < jokeIndexes.Length; i++)
        {
            var jokeText = badJokesJesterPool.PeekJokeAtIndex(jokeIndexes[i]);
            if (jokeText.Length > 45)
            {
                var firstSentence = jokeText.Split('.', '?', '!').FirstOrDefault(); // take first sentence only
                jokeText = jokeText.Substring(0, firstSentence.Length + 1);
                if (jokeText.Length > 45)
                {
                    var parts = jokeText.Split(" ");
                    jokeText = string.Empty;
                    foreach (var part in parts)
                    {
                        if (jokeText.Length + part.Length > 45)
                            break;
                        jokeText += part + " "; // keep adding words unless it would be over limit
                    }
                }
                if (!char.IsWhiteSpace(jokeText[jokeText.Length - 1]))
                    jokeText += " ";
                jokeText += "...";
            }
            dialog1.choices.Add(jokeText);
        }
        yield return DialogManager.Instance.ShowDialogCoroutine(dialog1);

        var selectedJokeIndex = jokeIndexes[dialog1.resultChoiceSelected.Value];
        var dialog2 = new DialogManager.DialogData();
        dialog2.lines.Add("Jester: " + badJokesJesterPool.TakeJokeAtIndex(selectedJokeIndex));
        yield return DialogManager.Instance.ShowDialogCoroutine(dialog2);
    }
    
    IEnumerator TellRandomBadJokeToVillager()
    {
        if (DialogManager.Instance.isShowing)
            yield break;

        yield return TellPlayerSelectedBadJoke();

        var dialog3 = new DialogManager.DialogData();
        dialog3.lines.Add("Villager: " + badJokesVillagerResponsesPool.TakeRandomLine());
        yield return DialogManager.Instance.ShowDialogCoroutine(dialog3);

        gameState = GameState.FreeRoam;

        numBadJokesToldToVillagers++;
        if (numBadJokesToldToVillagers >= 3 && overdog.QuestsState == QuestsState.QuestMakeVillagersLaugh)
        {
            overdog.QuestsState = QuestsState.QuestMakeVillagersLaugh_Finished;
            ShowDialogThenFreeRoam("Jester: I can't make them laugh :C", () =>
            {
                ShowDialogThenFreeRoam("Dog: Woof woof", () =>
                {
                    ShowDialogThenFreeRoam("Jester: Oh, the puppy is sad, maybe I can make it laugh!", () =>
                    {
                        overdog.QuestsState = QuestsState.CantMakeVillagersLaughLetsTryDog;
                        overdog.SetSprite(DogEvilState.CuteSad);
                        overdog.ChangeSpeechBubble(DogSpeechBubble.Sad);
                    });
                });
            });
        }
    }


    public void OnPlayerInteractedWithOverDog()
    {
        if (overdog.QuestsState == QuestsState.CantMakeVillagersLaughLetsTryDog)
        {
            overdog.QuestsState = QuestsState.QuestBanana_GaveBanana;
            overdog.ChangeSpeechBubble(DogSpeechBubble.Sad);

            ShowDialogThenFreeRoam(
                "Dog: Hi here is a banana! Use it and make me happy happy happy!",
                () =>
                {
                    SpawnItemInfrontOfDog(prefabItemBanana);
                }
            );
        }
        else if (overdog.QuestsState == QuestsState.QuestBanana_Finished_SeenPeopleFallNoLongerInterested)
        {
            overdog.QuestsState = QuestsState.QuestScareGrandma_GaveScissors;
            overdog.SetSprite(DogEvilState.CuteSad);
            overdog.ChangeSpeechBubble(DogSpeechBubble.Sad);

            ShowDialogThenFreeRoam(
                "Dog: Here are scissors! Use them on something and scare that grandma!",
                () =>
                {
                    SpawnItemInfrontOfDog(prefabItemScissors);
                }
            );
            quest2.SetActive(true);
        }
        else if (overdog.QuestsState == QuestsState.QuestScareGrandma_Finished)
        {
            overdog.QuestsState = QuestsState.QuestPoisonCake_GavePoison_WaitingForPeopleToDie;
            overdog.SetSprite(DogEvilState.BigSad);
            overdog.ChangeSpeechBubble(DogSpeechBubble.Sad);

            ShowDialogThenFreeRoam(
                "Dog: Find something poisonus! Use it and make me happy happy happy!",
                () =>
                {
                    CakeQuest.StartQuestt();
                }
            );
            quest3.SetActive(true);
        }
        else if (overdog.QuestsState == QuestsState.QuestPoisonCake_Finished_PeopleDied)
        {
            overdog.QuestsState = QuestsState.QuestBurningHouse;
            overdog.SetSprite(DogEvilState.DemonSad);
            overdog.ChangeSpeechBubble(DogSpeechBubble.Sad);

            ShowDialogThenFreeRoam(
                "Dog: Hey! Take the torch from the house and get it hot in here!"
            );
            quest4.SetActive(true);
        }
        else if (
            overdog.QuestsState == QuestsState.EndGame_HousesBurned || 
            overdog.QuestsState == QuestsState.EndGame_ComeLetMeRewardYou || 
            overdog.QuestsState == QuestsState.EndGame_YouAreFunAfterAll
        ) {
            ShowDialogThenFreeRoam(
                "Dog: Here is your reward!",
                () => 
                {
                    overdog.EatPlayer();
                    AudioManager.i.PlaySfx(happySounds[2]);
                }
            );
        }
        else if (overdog.QuestsState < QuestsState.CantMakeVillagersLaughLetsTryDog)
        {
            ShowDialogThenFreeRoam(
                "Dog: Woof woof.",
                () =>
                {
                    ShowRandomDialogThenFreeRoam(
                        new[]
                        {
                            "Jester: He is so cute.",
                            "Jester: Cute little puppy!",
                            "Jester: Awwhhh.",
                        }
                    );
                }
            );
        }
        else if (overdog.QuestsState < QuestsState.QuestBurningHouse_Finished)// during quest
        {
            ShowDialogThenFreeRoam("Dog: " + dogQuestInProgressDialogsPool.TakeRandomLine());
        }
    }
    public void OnCharacterHasFallen(Character character)
    {
        if (character.IsNPCCharacter)
        {
            if (overdog.QuestsState == QuestsState.QuestBanana_GaveBanana)
            {
                overdog.QuestsState = QuestsState.QuestBanana_Finished_SeenPeopleFallNoLongerInterested;
                overdog.SetSprite(DogEvilState.CuteHappy);
                overdog.ChangeSpeechBubble(DogSpeechBubble.Happy);
                AudioManager.i.PlaySfx(happySounds[0]);
                ShowDialogThenFreeRoam("Dog: I'm pleased *hahaha*.");
            }
        }
    }

    public void OnGrandmaScared()
    {
        if (overdog.QuestsState == QuestsState.QuestScareGrandma_WaitingForGrandmaScare)
        {
            overdog.QuestsState = QuestsState.QuestScareGrandma_Finished;
            overdog.SetSprite(DogEvilState.BigHappy);
            overdog.ChangeSpeechBubble(DogSpeechBubble.Happy);
            AudioManager.i.PlaySfx(happySounds[1]);
            ShowDialogThenFreeRoam("Dog: *hahaha* goood.");

        }
    }


    public void OnHouseLit()
    {
        if (overdog.QuestsState == QuestsState.QuestBurningHouse_Torch_Lit)
        {
            overdog.QuestsState = QuestsState.QuestBurningHouse_Finished;
            overdog.SetSprite(DogEvilState.DemonHappy);
            overdog.ChangeSpeechBubble(DogSpeechBubble.Happy);
            AudioManager.i.PlaySfx(happySounds[2]);
            ShowDialogThenFreeRoam(
                "Dog: That's what I call a fire *hahh*",
                () => { 
                    overdog.QuestsState = QuestsState.EndGame_HousesBurned;
                    Invoke(nameof(EndGame_YouAreFunAfterAll), 20 + UnityEngine.Random.Range(-5, 5));
                }
            );
        }
    }
    void EndGame_YouAreFunAfterAll()
    {
        ShowDialogThenFreeRoam(
            "Dog: The world seems fun like this, maybe you're funny after all.",
            () =>
            {
                overdog.QuestsState = QuestsState.EndGame_YouAreFunAfterAll;
                Invoke(nameof(EndGame_ComeLetMeRewardYou), 10 + UnityEngine.Random.Range(-5, 5));
            }
        );
    }

    void EndGame_ComeLetMeRewardYou()
    {
        ShowDialogThenFreeRoam(
            "Dog: Come to me for your reward!",
            () =>
            {
                overdog.QuestsState = QuestsState.EndGame_ComeLetMeRewardYou;
            }
        );
    }

    public void OnTorchTaken()
    {
        if (overdog.QuestsState == QuestsState.QuestBurningHouse)
        {
            overdog.QuestsState = QuestsState.QuestBurningHouse_TorchTaken;
        }
    }
    public void OnTorchLit()
    {
        if (overdog.QuestsState == QuestsState.QuestBurningHouse_TorchTaken)
        {
            overdog.QuestsState = QuestsState.QuestBurningHouse_Torch_Lit;
        }
    }

    public void OnHouseBurned()
    {
        if (OverDog.i.QuestsState == QuestsState.QuestBurningHouse_Torch_Lit)
            houseBurned++;
    }


    public void EndGame()
    {
        DialogManager.Instance.QueueDialogToShow("Jester: Who's laughing now?");
    }

    public void SpawnItemInfrontOfDog(GameObject item)
    {
        var banana = Instantiate(item, overdog.gameObject.transform.position + new Vector3(-2, 0, 0), Quaternion.identity);
        banana.GetComponent<ItemBase>().SnapToTileAtCurrentPosition();
        gameState = GameState.FreeRoam;
    }

    public void PauseGame(bool pause)
    {
        if (pause)
        {
            prevState = gameState;
            gameState = GameState.Paused;
        }
        else
        {
            gameState = prevState;
        }
    }

    bool openedMenuAfterPlayerDied = false;
    void OpenMenu()
    {
        if (!playerController || !playerController.Character || playerController.Character.IsDead)
        {
            menuController.OpenMenu_PlayerDead();
        }
        else
        {
            menuController.OpenMenu_InGame();
        }
        gameState = GameState.Menu;
    }

    
    private void Update()
    {
        worldCamera.gameObject.SetActive(true);
        if (playerController.connectCamera)
        {
            Vector3 cameraPosition = worldCamera.transform.position;
            cameraPosition.x = playerController.gameObject.transform.position.x;
            cameraPosition.y = playerController.gameObject.transform.position.y;
            worldCamera.transform.position = cameraPosition;
        }
        else if (playerController.Character.IsDead)
        {
            if (!openedMenuAfterPlayerDied)
            {
                openedMenuAfterPlayerDied = true;
                Invoke(nameof(OpenMenu), 1);
            }
        }


        if (overdog.QuestsState == QuestsState.QuestPoisonCake_GavePoison_WaitingForPeopleToDie)
        {
            if (NumNPCSPoisoned == 3)
            {
                overdog.QuestsState = QuestsState.QuestPoisonCake_Finished_PeopleDied;
                overdog.ChangeSpeechBubble(DogSpeechBubble.Happy);
                overdog.SetSprite(DogEvilState.DemonHappy);
                AudioManager.i.PlaySfx(happySounds[1]);
                ShowDialogThenFreeRoam("Dog: *hahaha* nice.");
            }
        }

        if (gameState == GameState.EvilDogSideview)
        {
            evilDogSideviewUI.gameObject.SetActive(true);
        }
        else
        {
            evilDogSideviewUI.gameObject.SetActive(false);
        }

        if (gameState == GameState.FreeRoam && !DialogManager.Instance.isShowing)
        {
            if (playerController)
            {
                playerController.HandleUpdate();

                if (PlayerController.OpenMenuKeyDown())
                {
                    OpenMenu();
                }
            }
        }
        else if (gameState == GameState.Menu)
        {
            menuController.HandleUpdate();
        }
        else if (gameState == GameState.Knocked)
        {
            // Stop the player Movement
        }
    }

    public void TriggerKnockout()
    {
        gameState = GameState.Knocked;
    }

    public IEnumerator MoveCamera(Vector2 moveOffset, bool waitForFadeOut = false)
    {
        yield return Fader.i.FadeIn(0.5f);

        worldCamera.transform.position += new Vector3(moveOffset.x, moveOffset.y);

        if (waitForFadeOut)
            yield return Fader.i.FadeOut(0.5f);
        else
            StartCoroutine(Fader.i.FadeOut(0.5f));
    }

    public void ShowRandomDialogThenFreeRoam(string[] dialogs, Action onDialogFinished = null)
    {
        var dialogData = new DialogManager.DialogData()
        {
            onDialogFinished = () =>
            {
                onDialogFinished?.Invoke();
                gameState = GameState.FreeRoam;
            },
        };
        string text = dialogs[UnityEngine.Random.Range(0, dialogs.Length)];
        dialogData.lines.Add(text);
        DialogManager.Instance.QueueDialogToShow(dialogData);
    }

    public void ShowDialogThenFreeRoam(string text, Action onDialogFinished = null)
    {
        var dialogData = new DialogManager.DialogData()
        {
            onDialogFinished = () =>
            {
                onDialogFinished?.Invoke();
                gameState = GameState.FreeRoam;
            },
        };
        dialogData.lines.Add(text);
        DialogManager.Instance.QueueDialogToShow(dialogData);
    }
}
