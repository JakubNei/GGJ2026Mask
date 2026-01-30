using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class DialogManager : MonoBehaviour
{
    [SerializeField] GameObject dialogBox;
    [SerializeField] ChoiceBox choiceBox;
    [SerializeField] Text dialogText;
    [SerializeField] int lettersPerSecond;

    public event Action OnShowDialog;
    public event Action OnDialogFinished;

    public static DialogManager Instance { get; private set; }

    public bool isShowing { get; private set; }

    public Queue<DialogData> queuedDialogsToShow = new();

    [Serializable] // for debug
    public class DialogData
    {
        public List<string> lines = new();
        public bool waitForInput = true;
        public List<string> choices = new();
        public Action<int> onChoiceSelected = null;
        public Action onDialogFinished = null;
        public int? resultChoiceSelected;
    }

    bool? skipDialog;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (!isShowing && queuedDialogsToShow.Count > 0)
        {
            StartCoroutine(ShowDialogCoroutine(queuedDialogsToShow.Dequeue()));
        }

        if (skipDialog.HasValue && WaitForInput_GetKeyDown())
        {
            skipDialog = true;
        }
    }

    public static bool WaitForInput_GetKeyDown()
    {
        return PlayerController.IsInteractInputKey();
    }

    public void QueueDialogToShow(DialogData dialogData)
    {
        queuedDialogsToShow.Enqueue(dialogData);
    }

    public void QueueDialogToShow(string text)
    {
        var dialogData = new DialogData();
        dialogData.lines.Add(text);
        QueueDialogToShow(dialogData);
    }


    public IEnumerator ShowDialogText(string text)
    {
        var dialogData = new DialogData();
        dialogData.lines.Add(text);
        return ShowDialogCoroutine(dialogData);
    }

    public IEnumerator ShowDialog(Dialog dialog, List<string> choices = null,
          Action<int> onChoiceSelected = null, Action onDialogFinished = null)
    {
        var dialogData = new DialogData()
        {
            lines = dialog.Lines,
            choices = choices,
            onChoiceSelected = onChoiceSelected,
            onDialogFinished = onDialogFinished,
        };
        return ShowDialogCoroutine(dialogData);
    }


    public IEnumerator ShowDialogCoroutine(DialogData dialogData)
    {
        yield return new WaitForEndOfFrame();

        PlayerController.Instance.Character.Animator.IsMoving = false;
        OnShowDialog?.Invoke();
        isShowing = true;
        dialogText.text = "";
        dialogBox.SetActive(true);
        
        if (dialogData.lines != null)
        {
            foreach (var line in dialogData.lines)
            {
                AudioManager.i.PlaySfx(AudioId.UISelect);
                yield return TypeDialog(line);
                if (dialogData.waitForInput)
                {
                    yield return new WaitUntil(() => WaitForInput_GetKeyDown());
                }
            }
        }

        if (dialogData?.choices.Count > 1)
        {
            yield return choiceBox.ShowChoices(dialogData.choices, dialogData.onChoiceSelected);
            dialogData.resultChoiceSelected = choiceBox.currentChoice;
        }

        dialogBox.SetActive(false);
        isShowing = false;
        OnDialogFinished?.Invoke();
        dialogData.onDialogFinished?.Invoke();
    }

    private IEnumerator TypeDialog(string line)
    {
        skipDialog = false;
        dialogText.text = "";
        foreach (var letter in line.ToCharArray())
        {
            dialogText.text += letter;
            double t = Time.realtimeSinceStartupAsDouble;
            while (t + 1.0 / 30.0 > Time.realtimeSinceStartupAsDouble)
            {
                if (skipDialog.Value)
                {
                    dialogText.text = line;
                    skipDialog = null;
                    yield break;
                }
                yield return new WaitForEndOfFrame();
            }
        }
        skipDialog = null;
    }

}
