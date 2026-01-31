using System.ComponentModel;
using UnityEngine;
using UnityEngine.Timeline;

[RequireComponent(typeof(Collider2D))]
public class PressurePlate : MonoBehaviour
{
    public SignalReceiver[] configSendSignalTo;

    public float configTimeToReset = -1;

    public bool isTriggered = false;

    public SpriteRenderer spriteRenderer;
    public float timeToReset = 0;
    void Update()
    {
        if (isTriggered)
            configSendSignalTo.ReceiveSignal();
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
        timeToReset = configTimeToReset;
        isTriggered = newState;
        spriteRenderer.color = isTriggered ? Color.red : Color.white;
    }
}
