using System.ComponentModel;
using UnityEngine;
using UnityEngine.Timeline;

[RequireComponent(typeof(Collider2D))]
public class PressurePlate : MonoBehaviour
{
    public SignalReceiver[] configSendSignalTo;
    [SerializeField] GameObject configOffVisual;
    [SerializeField] GameObject configOnVisual;

    public float configTimeToRelease = -1;

    public bool isTriggered = false;

    public float currentTimeToRelease = 0;
    void Start()
    {
        SetTrigerred(false);
    }
    void Update()
    {
        if (currentTimeToRelease <= 0)
            return;
        currentTimeToRelease -= Time.deltaTime;
        if (currentTimeToRelease <= 0)
            SetTrigerred(false);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        SetTrigerred(true);
    }
    void OnTriggerExit2D(Collider2D collision)
    {
        if (configTimeToRelease > 0)
            return;
        SetTrigerred(false);
    }

    void SetTrigerred(bool newState)
    {
        if (newState == isTriggered)
            return;
        configOnVisual.SetActive(newState);
        configOffVisual.SetActive(!newState);
        currentTimeToRelease = configTimeToRelease;
        isTriggered = newState;
        if (isTriggered)
            configSendSignalTo.ReceiveSignalOn();
        else
            configSendSignalTo.ReceiveSignalOff();

    }
}
