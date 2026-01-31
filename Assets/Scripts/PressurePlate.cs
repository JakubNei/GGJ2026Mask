using System.ComponentModel;
using UnityEngine;
using UnityEngine.Timeline;

[RequireComponent(typeof(Collider2D))]
public class PressurePlate : MonoBehaviour
{
    public SignalReceiver[] configSendSignalTo;
    [SerializeField] GameObject configOffVisual;
    [SerializeField] GameObject configOnVisual;

    public float configTimeToReset = -1;

    public bool isTriggered = false;

    public float timeToReset = 0;
    void Start()
    {
        SetTrigerred(false);
    }
    void Update()
    {
        if (timeToReset <= 0)
            return;
        timeToReset -= Time.deltaTime;
        if (timeToReset <= 0)
            SetTrigerred(false);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        SetTrigerred(true);
    }
    void OnTriggerExit2D(Collider2D collision)
    {
        if (configTimeToReset > 0)
            return;
        SetTrigerred(false);
    }

    void SetTrigerred(bool newState)
    {
        if (newState == isTriggered)
            return;
        configOnVisual.SetActive(newState);
        configOffVisual.SetActive(!newState);
        timeToReset = configTimeToReset;
        isTriggered = newState;
        if (isTriggered)
            configSendSignalTo.ReceiveSignalOn();
        else
            configSendSignalTo.ReceiveSignalOff();

    }
}
