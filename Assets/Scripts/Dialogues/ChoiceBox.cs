using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChoiceBox : MonoBehaviour
{
    [SerializeField] ChoiceText choiceTextPrefab;

    bool choiceSelected = false;

    List<ChoiceText> choiceTexts;
    public int currentChoice { get; private set; }

    public IEnumerator ShowChoices(List<string> choices, Action<int> onChoiceSelected)
    {
        choiceSelected = false;
        currentChoice = 0;

        gameObject.SetActive(true);

        // delete existing choices
        foreach (Transform child in transform)
            Destroy(child.gameObject);

        choiceTexts = new List<ChoiceText>();
        foreach (var choice in choices)
        {
            var choiceTextObj = Instantiate(choiceTextPrefab, transform);
            choiceTextObj.TextField.text = choice;
            choiceTexts.Add(choiceTextObj);
        }

        yield return new WaitUntil(() => choiceSelected == true);

        onChoiceSelected?.Invoke(currentChoice);
        gameObject.SetActive(false);
    }

    private void Update()
    {
        if (choiceTexts == null)
            return;

        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
            ++currentChoice;
        else if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
            --currentChoice;

        var newCurrent = Mathf.Clamp(currentChoice, 0, choiceTexts.Count - 1);
        if (newCurrent != currentChoice)
        {
            AudioManager.i.PlaySfx(AudioId.HoveringOverMenu);
            currentChoice = newCurrent;
        }

        for (int i = 0; i < choiceTexts.Count; i++)
        {
            choiceTexts[i].SetSelected(i == currentChoice);
        }

        if (DialogManager.WaitForInput_GetKeyDown())
            choiceSelected = true;
    }
}
