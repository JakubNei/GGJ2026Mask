using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class HoleBlocker : SignalReceiver
{
    [SerializeField] bool startEnabled = true;

    void Awake()
    {
        EnableHoleBlocker(startEnabled);
    }

    public override void OnReceiveSignalOn()
    {
        EnableHoleBlocker(!startEnabled);
    }

    public override void OnReceiveSignalOff()
    {
        EnableHoleBlocker(startEnabled);
    }


    void EnableHoleBlocker(bool enable)
    {
        gameObject.SetActive(enable);
    }

}
