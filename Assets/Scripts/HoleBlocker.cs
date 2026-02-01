using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class HoleBlocker : SignalReceiver
{
    [SerializeField] bool startEnabled = true;

    [SerializeField] Collider2D coolider;
    [SerializeField] GameObject visual;

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
        coolider.enabled = enable;
        visual.SetActive(enable);
    }

}
