using UnityEngine;

public class Escape_Door_Script : MonoBehaviour
{
    [SerializeField] BoxCollider doorCollider;

    [Header("Door")]
    [SerializeField] Transform door;
    [SerializeField] Vector3 doorClosePos;
    [SerializeField] Vector3 doorOpenPos;

    public void ExitDoorState(bool state)
    {
        doorCollider.enabled = state;
        door.localPosition = state ? doorOpenPos : doorClosePos;
        Player_Game_UI_Manager.instance.UpdateObjective("Escape!");
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Logic to open the door
            Debug.Log("Player has reached the exit door.");

            GameManager.instance.GameComplete();
        }
    }
}
