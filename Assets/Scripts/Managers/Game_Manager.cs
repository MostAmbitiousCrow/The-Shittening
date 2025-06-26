using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

public class GameManager : MonoBehaviour // By Samuel White
{
    //========================================
    // The Game Manager class.
    // This class is used to store the fundamental game data.
    //========================================

    public static GameManager instance;
    public static List<PlayerData> playerData = new();

    [Header("---Players---")]
    public InputActionAsset defaultInputActions;
    [SerializeField] private Transform playerCamera;

    [Header("---Event Systems---")]
    public EventSystem eventSystem;
    public MultiplayerEventSystem multiplayerEventSystem;

    [Header("---Starting Level---")]
    public Scene_Loader_Transition.SceneNames startingLevel = Scene_Loader_Transition.SceneNames.Main_Menu;

    private void Awake()
    {
        if (instance == null) instance = this; // If a Game Manager already exists, destroy
        else
        {
            Destroy(transform.root.gameObject);
            return;
        }
        DontDestroyOnLoad(transform.root);
        GameData.currentLevel = startingLevel; // Set inital Scene. Default case: Main Menu
    }

    private void Start()
    {
        SceneLoaded(); // Get Event Systems
        Settings_Manager.instance.LoadSettings();
        SetGameDifficulty(GameData.Difficulty.Easy);
    }

    #region Set Game Difficulty
    // ======================================== Set Game Difficulty ========================================
    public void SetGameDifficulty(GameData.Difficulty difficulty)
    {
        GameData.gameDifficulty = difficulty;

        // Add any difficulty-related functionality here!
    }
    #endregion

    #region Start Game
    // ======================================== Start Game ========================================
    public void StartGame()
    {
        GameData.isGameStarted = true;
        GameData.isGameOver = false;
        GameData.isGameFinished = false;
        GameData.isPaused = false;
        GameData.canPause = true;
        Player_Input_Manager.instance.CreatePlayerCharacter();

        AudioManager.PlayMusic(AudioManager.MusicOptions.Play, MusicCategory.MusicSoundTypes.AmbientMusic, 1);
        Time.timeScale = Settings_Manager.gameSpeed;
        playerData[0].playerController.AssignCamera(true);
        playerData[0].playerController.LockCursor(true);

        Environment_Manager.instance.SpawnKeys();
        playerData[0].playerController.UpdateSpawn(Game_Events_Manager.instance.playerSpawnPoint.position);
        foreach (var item in playerData) Player_ChangeActionMap(true, 0, "Player"); // Change Player Input Map
        StartCoroutine(Game_Events_Manager.instance.GameTimer());
    }
    #endregion

    #region Game Complete
    // ======================================== Game Complete ========================================
    public void GameComplete()
    {
        GameData.isGameStarted = false;
        GameData.isGameOver = true;
        GameData.isGameFinished = true;
        GameData.isPaused = false;
        GameData.canPause = false;
        playerData[0].isDead = false;

        // Toggle Game Complete Menu
        Player_Game_UI_Manager.instance.ShowEndScreen(Player_Game_UI_Manager.GameOverEvent.Escaped);
        // Player_Game_UI_Manager.instance.ShowResultsMenu(true); // <<<< Show the results menu (if applicable)
    }
    #endregion

    #region Pause Game
    // ======================================== Pause Game ========================================
    public void PauseGame(bool pause)
    {
        if (!GameData.canPause) return;
        Debug.Log($"Game Paused: {pause}");

        GameData.isPaused = pause;
        Time.timeScale = pause ? 0 : Settings_Manager.gameSpeed;
        if (pause) AudioManager.UpdateMusic(AudioManager.MusicOptions.Pause);
        else AudioManager.UpdateMusic(AudioManager.MusicOptions.Resume);
    }
    #endregion

    #region Game Over
    // ======================================== Game Over ========================================
    public void GameOver(Player_Game_UI_Manager.GameOverEvent gameEvent)
    {
        GameData.isGameStarted = false;
        GameData.isGameOver = true;
        GameData.isGameFinished = true;
        GameData.isPaused = true;
        GameData.canPause = false;
        if (gameEvent != Player_Game_UI_Manager.GameOverEvent.Escaped) playerData[0].isDead = true;

        // Toggle Game Over Menu
        Player_Game_UI_Manager.instance.DamageFlashEvent(1);
        Player_Game_UI_Manager.instance.ShowEndScreen(gameEvent);
    }
    #endregion

    #region Restart Game
    // ======================================== Restart Game ========================================
    public void RestartGame()
    {
        GameData.isGameOver = false;
        GameData.isGameFinished = false;
        GameData.isPaused = false;

        if (GameData.isMultiplayer)
        {
            foreach (var item in playerData)
            {
                item.score = 0;
                // item.lives = GameData.current_DifficultyData.difficultyData.playerLives; // Set lives (if applicable)
                item.kills = 0;
                item.deaths = 0;
                item.isDead = false;
            }
        }
        else
        {
            playerData[0].score = 0;
            // playerData[0].lives = GameData.current_DifficultyData.difficultyData.playerLives; // Set lives (if applicable)
            playerData[0].kills = 0;
            playerData[0].deaths = 0;
            playerData[0].isDead = false;
        }

        // TODO - Restart the game
        Scene_Loader_Transition.LoadScene(GameData.currentLevel); // <<< Make sure to update the current level before reloading.
    }
    #endregion

    #region UI Button Select

    public void EventSystem_SelectUIButton(GameObject button)
    {
        if (GameData.isMultiplayer)
            multiplayerEventSystem.SetSelectedGameObject(button);
        else eventSystem.SetSelectedGameObject(button);
    }
    #endregion

    #region Update Event System Mapping
    public void EventSystem_UpdateUIActions()
    {
        if (PlayerInputManager.instance.playerCount < 1) return;

        InputSystemUIInputModule uiModule = eventSystem.GetComponent<InputSystemUIInputModule>();
        if (GameData.isMultiplayer)
        {
            foreach (var item in playerData)
            {
                uiModule.actionsAsset = item.playerInput.actions;
                item.playerInput.uiInputModule = uiModule;
            }
        }
        else
        {
            uiModule.actionsAsset = playerData[0].playerInput.actions;
            playerData[0].playerInput.uiInputModule = uiModule;
        }
    }
    #endregion

    #region Change Action Map

    public void Player_ChangeActionMap(bool changeAll, int playerNum, string mapName)
    {
        if (changeAll) foreach (var item in playerData) item.playerInput.SwitchCurrentActionMap(mapName);
        else playerData[playerNum].playerInput.SwitchCurrentActionMap(mapName);
    }
    #endregion

    #region Scene Loaded

    public void SceneLoaded()
    {
        // Get Event Systems
        if (GameData.isMultiplayer)
        {
            if (GameObject.FindGameObjectWithTag("MultiplayerEventSystem").TryGetComponent(out MultiplayerEventSystem system)) multiplayerEventSystem = system;
        }
        else if (GameObject.FindGameObjectWithTag("EventSystem").TryGetComponent(out EventSystem mSystem)) eventSystem = mSystem;
        EventSystem_UpdateUIActions();
    }
    #endregion
}

#region Game Data

public static class GameData
{
    public static bool isMultiplayer;

    public static bool isGameOver;
    public static bool isPaused;
    public static bool canPause;
    public static bool isGameStarted;
    public static bool isGameFinished;

    public enum Difficulty { Easy, Normal, Hard }
    public static Difficulty gameDifficulty;

    public static Scene_Loader_Transition.SceneNames currentLevel;

    public static int maxKeys = 4;
    public static int keysCollected = 0;
    public static bool gateKeyCollected = false;
}
#endregion

#region Player Data

// Insert all your player data here for easy access.
public class PlayerData
{
    public GameObject playerObject;
    public Transform playerTransform;
    public PlayerInput playerInput;

    public Player_Controller playerController; // <<< Put your Player Data script here

    public float score;
    public float highScore;
    public float scoreMultiplier = 1;

    public int lives;
    public int kills;
    public int deaths;

    public bool isDead;
    public bool isInvincible;
}
#endregion