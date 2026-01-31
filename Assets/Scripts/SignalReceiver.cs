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
        foreach (var receiver in receivers)
        {
            receiver.OnReceiveSignalOn();
        }
    }

    public static void ReceiveSignalOff(this SignalReceiver[] receivers)
    {
        foreach (var receiver in receivers)
        {
            receiver.OnReceiveSignalOff();
        }
    }

}