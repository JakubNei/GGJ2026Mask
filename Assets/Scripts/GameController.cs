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
    FreeRoam,
    Menu,
    Paused,
    Shaman,
    ToughGuy,
}

public class GameController : MonoBehaviour
{
    [SerializeField] PlayerController playerController;


    [SerializeField] GameObject normalPlane;
    [SerializeField] GameObject astralPlane;



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
    

    public void EndGame()
    {
        DialogManager.Instance.QueueDialogToShow("Jester: Who's laughing now?");
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
        if(gameState == GameState.Shaman) 
        {
            astralPlane.SetActive(true);
            normalPlane.SetActive(false);
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
