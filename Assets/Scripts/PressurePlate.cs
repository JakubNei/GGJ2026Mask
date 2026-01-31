using System.ComponentModel;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class PressurePlate : SignalSender
{

    public override bool IsSendingSignal => isTriggered;

    public float configTimeToReset = -1;

    public bool isTriggered = false;

    public SpriteRenderer spriteRenderer;
    public float timeToReset = 0;
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
    
    void SetTrigerred(bool newState)
    {
        timeToReset = configTimeToReset;
        isTriggered = newState;
        spriteRenderer.color = isTriggered ? Color.red : Color.wheat;
    }
}
