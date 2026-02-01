using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class HoleBlocker : SignalReceiver
{
    [SerializeField] bool startEnabled = true;
    [SerializeField] AudioSource audioSource;

    void Awake()
    {
        EnableHoleBlocker(startEnabled, playSound: false);
    }

    public override void OnReceiveSignalOn()
    {
        EnableHoleBlocker(!startEnabled, playSound: true);
    }

    public override void OnReceiveSignalOff()
    {
        EnableHoleBlocker(startEnabled, playSound: true);
    }

    void EnableHoleBlocker(bool enable, bool playSound)
    {
        gameObject.SetActive(enable);
        if (playSound && audioSource != null)
            audioSource.Play();
    }
}
