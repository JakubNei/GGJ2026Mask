using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Timeline;

[RequireComponent(typeof(Collider2D))]
public class PressurePlate : MonoBehaviour
{
    public SignalReceiver[] configSendSignalTo;
    [SerializeField] GameObject configOffVisual;
    [SerializeField] GameObject configOnVisual;


    public bool isTriggered = false;

    void Start()
    {
        SetTrigerred(false);
    }

    public List<Collider2D> currentlyColliding = new();
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (currentlyColliding.Count == 0)
            SetTrigerred(true);
        if (!currentlyColliding.Contains(collision))
            currentlyColliding.Add(collision);
    }
    void OnTriggerExit2D(Collider2D collision)
    {
        currentlyColliding.Remove(collision);
        if (currentlyColliding.Count == 0)
            SetTrigerred(false);
    }

    [ContextMenu("Debug Test Trigger On")]
    void DebugTestTriggerOn()
    {
        SetTrigerred(true);
    }

    [ContextMenu("Debug Test Trigger Off")]
    void DebugTestTriggerOff()
    {
        SetTrigerred(false);
    }

    void SetTrigerred(bool newState)
    {
        if (newState == isTriggered)
            return;
        configOnVisual.SetActive(newState);
        configOffVisual.SetActive(!newState);
        isTriggered = newState;
        if (isTriggered)
            configSendSignalTo.ReceiveSignalOn();
        else
            configSendSignalTo.ReceiveSignalOff();

    }
}
