using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapTrigger : MonoBehaviour
{
    [SerializeField] string puzzleName = "";
    private void OnTriggerEnter(Collider other)
    {
        EventBus.Instance.OnMapTrigger?.Invoke(gameObject, puzzleName);
    }

    private void OnTriggerExit(Collider other)
    {
        //EventBus.Instance.OnMapTrigger?.Invoke();
    }
}
