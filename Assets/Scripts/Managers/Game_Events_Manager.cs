using System.Collections;
using UnityEngine;

public class Game_Events_Manager : MonoBehaviour
{
    //========================================
    // The Game Events Manager class.
    // This class is used to manage game events and triggers.
    //========================================

    public static Game_Events_Manager instance;
    public Enemy_StateMachine enemyScript;
    public Escape_Door_Script escape_Door_Script;
    public Transform playerSpawnPoint;

    public float gameTime;
    public float maxGameTime = 60;

    private void Awake()
    {
        instance = this;
    }

    // Add methods to handle game events here
    #region Give Key
    public void GiveKey()
    {
        GameData.keysCollected++;
        Debug.Log("Key collected! Total keys: " + GameData.keysCollected);
        if (GameData.keysCollected >= GameData.maxKeys)
        {
            Debug.Log("All keys collected! Exit Door is Open.");
            ReleaseDoor();
        }
    }
    #endregion

    #region Release Door
    public void ReleaseDoor()
    {
        escape_Door_Script.ExitDoorState(true);
        AudioManager.PlayPlayerSound(PlayerCategory.PlayerSoundTypes.Gate_Opened);
    }
    #endregion

    public IEnumerator GameTimer()
    {
        gameTime = maxGameTime;
        while (gameTime > 0)
        {
            GameManager.playerData[0].playerController.UpdateBomb(gameTime, 0);
            gameTime -= Global_Game_Speed.GetDeltaTime();
            yield return null;
        }
        GameManager.playerData[0].playerController.UpdateBomb(0, 0);
        PlayerBlowUp();
    }

    public void PlayerBlowUp()
    {
        GameManager.instance.GameOver(Player_Game_UI_Manager.GameOverEvent.BlownUp);
        StartCoroutine(Player_Game_UI_Manager.instance.ExplosionFlash());
        AudioManager.PlayPlayerSound(PlayerCategory.PlayerSoundTypes.Bomb_Explode);
    }
}
