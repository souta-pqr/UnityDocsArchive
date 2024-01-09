using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

Public class ColliderCallReceiver : MonoBehaviour
{
    public class TriggerEvent : UnityEvent<Collider> {}
    public TriggerEvent TriggerEnterEvent = new TriggerEvent();
    public TriggerEvent TriggerStayEvent = new TriggerEvent();
    public TriggerEvent TriggerExitEvent = new TriggerEvent();

    void Start()
    {

    }

    void OnTriggerEnter(Collider other)
    {
        TriggerEnterEvent?.Invoke(ohter);
    }

    void OnTriggerStay(Collider ohter)
    {
        TriggerStayEvent?.Invoke(other);
    }

    void OnTriggerExit(Collider other)
    {
        TriggerExitEvent?.Invoke(other);
    }
}