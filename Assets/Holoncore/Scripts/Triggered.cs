using UnityEngine;
using UnityEngine.Events;

public class Triggered : MonoBehaviour
{
    public UnityEvent TriggerEvent;
    
    private void OnTriggerEnter(Collider other)
    {
        TriggerEvent.Invoke();
    }
    
}
