using UnityEngine;

public class Key_Item_Script : MonoBehaviour
{
    [SerializeField] Transform spriteT;
    [SerializeField] bool isGateKey;
    
    void Update()
    {
        if (!GameData.isPaused)
        {
            SpinAnimation();
        }
    }

    void SpinAnimation()
    {
        spriteT.Rotate(Vector3.up, 100 * Global_Game_Speed.GetDeltaTime());
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (isGateKey)
            {
                Game_Events_Manager.instance.ReleaseDoor();
                GameData.gateKeyCollected = true;
                AudioManager.PlayPlayerSound(PlayerCategory.PlayerSoundTypes.Gate_Key_Collected);
            }
            else
            {
                // Add key to player's inventory
                Game_Events_Manager.instance.GiveKey();
                Game_Events_Manager.instance.enemyScript.UpdateStage();
                AudioManager.PlayPlayerSound(PlayerCategory.PlayerSoundTypes.Key_Collected);
                GameManager.playerData[0].playerController.UpdateBomb(60);
            }
            // Destroy the key item
            Destroy(gameObject);
        }
    }
}
