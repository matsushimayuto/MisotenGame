using System.Net;
using UnityEngine;
using UnityEngine.UI;

public class WorldAreaTrigger : MonoBehaviour
{
    


    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        other.GetComponent<InteractionController>()
             ?.OnEnterArea();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        other.GetComponent<InteractionController>()
             ?.OnExitArea();
    }
}
