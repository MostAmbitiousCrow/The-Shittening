using UnityEngine;

public class Escape_Door_Script : MonoBehaviour
{
    public BoxCollider doorTrigger;

    public void ExitDoorState(bool state)
    {
        doorTrigger.isTrigger = state;

    }
    
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && GameData.gateKeyCollected)
        {
            // Logic to open the door
            Debug.Log("Player has reached the exit door.");

            GameManager.instance.GameComplete();
        }
    }
}
