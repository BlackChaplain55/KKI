using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        EventBus.Instance.OnMapTrigger?.Invoke();
    }

    private void OnTriggerExit(Collider other)
    {
        //EventBus.Instance.OnMapTrigger?.Invoke();
    }
}
