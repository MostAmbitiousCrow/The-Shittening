using System.Collections.Generic;
using System.ComponentModel.Design;
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

    public enum ScoreContext { Enemy_Hit, Enemy_Defeated, Player_Hit, Player_Defeated, PowerUp_Obtained }

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

    #region Award Score
    // ======================================== Award Score ========================================
    public void AwardScore(int playerID, ScoreContext context)
    {
        switch (context)
        {
            case ScoreContext.Enemy_Hit:
                playerData[playerID].score += 10 * playerData[playerID].scoreMultiplier; // Add 10 points for hitting an enemy
                break;
            case ScoreContext.Enemy_Defeated:
                playerData[playerID].score += 100 * playerData[playerID].scoreMultiplier; // Add 100 points for defeating an enemy
                break;
            case ScoreContext.Player_Hit:
                playerData[playerID].score -= 20 * playerData[playerID].scoreMultiplier; // Subtract 20 points for player getting hit
                break;
            case ScoreContext.Player_Defeated:
                playerData[playerID].score -= 200 * playerData[playerID].scoreMultiplier; // Subtract 200 points for player defeat
                break;
            case ScoreContext.PowerUp_Obtained:
                playerData[playerID].score += 50 * playerData[playerID].scoreMultiplier; // Add 50 points for obtaining a power-up
                break;
        }
    }
    #endregion

    #region Set High Scores
    // ======================================== Set High Scores ========================================
    public void SetHighScore() // Each Player has their own high score, feel free to change this.
    {
        if (GameData.isMultiplayer)
        {
            for (int i = 0; i < playerData.Count; i++)
            {
                if (playerData[i].score > playerData[i].highScore)
                {
                    playerData[i].highScore = playerData[i].score;
                    PlayerPrefs.SetFloat($"P{i + 1}HighScore", playerData[i].highScore);
                }
            }
        }
        else
        {
            if (playerData[0].score > playerData[0].highScore)
            {
                playerData[0].highScore = playerData[0].score;
                PlayerPrefs.SetFloat($"P1HighScore", playerData[0].highScore);
            }
        }
    }
    #endregion

    #region Reset High Scores
    // ======================================== Reset High Scores ========================================
    public void ResetHighScores()
    {
        if (GameData.isMultiplayer)
        {
            for (int i = 0; i < playerData.Count; i++)
            {
                playerData[i].highScore = 0;
                PlayerPrefs.SetFloat($"P{i + 1}HighScore", 0);
            }
        }
        else
        {
            playerData[0].highScore = 0;
            PlayerPrefs.SetFloat("P1HighScore", 0);
        }
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

        AudioManager.PlayMusic(AudioManager.MusicOptions.Play, 1, .5f, MusicCategory.MusicSoundTypes.Game_Intro);
        Time.timeScale = Settings_Manager.gameSpeed;

        foreach (var item in playerData) Player_ChangeActionMap(true, 0, "Player"); // Change Player Input Map
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
    public void GameOver()
    {
        GameData.isGameStarted = false;
        GameData.isGameOver = true;
        GameData.isGameFinished = true;
        GameData.isPaused = true;
        GameData.canPause = false;

        // Toggle Game Over Menu
        // Player_Game_UI_Manager.instance.ShowResultsMenu(true); // <<<< Show the results menu (if applicable)
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

    public struct Players
    {
        public Transform playerTransform;
        public PlayerInput playerInput;
    }
    public static List<Players> players = new();

    public static Scene_Loader_Transition.SceneNames currentLevel;
}
#endregion

#region Player Data

// Insert all your player data here for easy access.
public class PlayerData
{
    public GameObject playerObject;
    public Transform playerTransform;
    public PlayerInput playerInput;

    // public Player_Character_Data characterData; // <<< Put your Player Data script here

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