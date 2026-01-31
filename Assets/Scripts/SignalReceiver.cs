using UnityEngine;

public class SignalReceiver : MonoBehaviour
{

    public virtual void OnReceiveSignalOn()
    {

    }
    public virtual void OnReceiveSignalOff()
    {

    }
}

public static class SignalReceiverExtensions
{
    public static void ReceiveSignalOn(this SignalReceiver[] receivers)
    {
        if (receivers == null)
            return;
        foreach (var receiver in receivers)
        {
            if (receiver == null)
                continue;
            receiver.OnReceiveSignalOn();
        }
    }

    public static void ReceiveSignalOff(this SignalReceiver[] receivers)
    {
        if (receivers == null)
            return;
        foreach (var receiver in receivers)
        {
            if (receiver == null)
                continue;
            receiver.OnReceiveSignalOff();
        }
    }

}