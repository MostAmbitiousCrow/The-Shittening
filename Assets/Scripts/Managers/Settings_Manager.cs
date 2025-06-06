using UnityEngine;

public class Settings_Manager : MonoBehaviour // By Samuel White
{
    //========================================
    // The Settings Manager is responsible for managing the player settings.
    // It will save, load and reset the settings.
    // Settings are stored via playerprefs.
    //========================================

    public static Settings_Manager instance;

    // =========================================
    // User Assigned Setting Variables

    [Header("User Assigned Settings")]
    [Header("Volume Settings")]
    public static float masterVolume = 1f;
    public static float musicVolume = 1f;
    public static float enemyVolume = 1f;
    public static float playerVolume = 1f;
    public static float interfaceVolume = 1f;

    [Space(10)]

    [Header("Accessibility Settings")]
    public static float gameSpeed = 1f; // Global Gameplay Speed Multiplier
    public static bool playerAutoShoot = false; // 0 = false, 1 = true
    public static bool playerInvicible = false;
    
    [Space(10)]

    /// <summary>
    /// Gameplay Settings
    /// These settings are used to determine how the player moves in the game.
    /// Will be assigned automatically based on the player input.  
    /// </summary>

    [Header("Gameplay Settings")]
    public static bool useCursorMovement = true; // 0 = false, 1 = true
    public static bool useControllerMovement = true; // 0 = false, 1 = true
    public static bool useKeyboardMovement = true; // 0 = false, 1 = true
    public static bool useTouchMovement = true; // 0 = false, 1 = true
    
    // =========================================
    // Default Settings

    [Header("Default Setting Variables")]
    [Header("Default Volume Settings")]
    public float defaultMasterVolume = 1f;
    public float defaultMusicVolume = 1f;
    public float defaultEnemyVolume = 1f;
    public float defaultPlayerVolume = 1f;
    public float defaultInterfaceVolume = 1f;

    [Space(10)]

    [Header("Accessibility Settings")]
    public float defaultGameSpeed = 1f; // Global Gameplay Speed Multiplier
    public bool defaultPlayerAutoShoot = false; // 0 = false, 1 = true
    public bool defaultEnableParticles = true; // 0 = false, 1 = true
    public bool defaultPlayerInvicible = false;
    [Range(0, 1)] public float defaultDamageFlashIntensity = 1f;
    public float cursorSensitivity = 1;

    // =========================================

    void Awake()
    {
        if (instance == null) instance = this;
    }

    public static void SaveSettings()
    {
        // Save Volume Settings
        PlayerPrefs.SetFloat("VolumeMaster", masterVolume);
        PlayerPrefs.SetFloat("VolumeMusic", musicVolume);
        PlayerPrefs.SetFloat("VolumeEnemy", enemyVolume);
        PlayerPrefs.SetFloat("VolumePlayer", playerVolume);
        PlayerPrefs.SetFloat("VolumeInterface", interfaceVolume);

        // Set Accessibility Settings
            // Motor
            PlayerPrefs.SetInt("PlayerAutoShoot", playerAutoShoot ? 1 : 0); // 0 = false, 1 = true
            PlayerPrefs.SetFloat("GameSpeed", gameSpeed); // Global Gameplay Speed Multiplier. 1 = normal speed
            // Other
            PlayerPrefs.SetInt("PlayerInvincibility", playerInvicible ? 1 : 0);

        // // Save Gameplay Settings
        //     PlayerPrefs.SetInt("CursorMovement", cursorMovement ? 1 : 0); // 0 = false, 1 = true
        //     PlayerPrefs.SetInt("ControllerMovement", controllerMovement ? 1 : 0); // 0 = false, 1 = true
        //     PlayerPrefs.SetInt("KeyboardMovement", keyboardMovement ? 1 : 0); // 0 = false, 1 = true
        //     PlayerPrefs.SetInt("TouchMovement", touchMovement ? 1 : 0); // 0 = false, 1 = true

        // Save Settings
            PlayerPrefs.Save();
            Debug.Log("Settings saved");
    }

    public void LoadSettings()
    {
        // Load Volume Settings
        masterVolume = PlayerPrefs.GetFloat("VolumeMaster", defaultMasterVolume);
        musicVolume = PlayerPrefs.GetFloat("VolumeMusic", defaultMusicVolume);
        enemyVolume = PlayerPrefs.GetFloat("VolumeEnemy", defaultEnemyVolume);
        playerVolume = PlayerPrefs.GetFloat("VolumePlayer", defaultPlayerVolume);
        interfaceVolume = PlayerPrefs.GetFloat("VolumeInterface", defaultInterfaceVolume);

        // Load Accessibility Settings
            // Motor
            playerAutoShoot = PlayerPrefs.GetInt("PlayerAutoShoot", defaultPlayerAutoShoot ? 1 : 0) == 1; // 0 = false, 1 = true
            gameSpeed = PlayerPrefs.GetFloat("GameSpeed", defaultGameSpeed); // Global Gameplay Speed Multiplier. 1 = normal speed
            // Other
            PlayerPrefs.GetInt("PlayerInvincibility", defaultPlayerInvicible ? 1 : 0);

        Debug.Log("Settings loaded");
    }

    // ========================================
    // Audio Control

    public static void SetMasterVolume(float volume)
    {
        masterVolume = volume;
        AudioListener.volume = masterVolume;
    }

    public static void SetMusicVolume(float volume)
    {
        musicVolume = volume;
        AudioListener.volume = musicVolume;
    }

    public static void SetEnemyVolume(float volume)
    {
        enemyVolume = volume;
        AudioListener.volume = enemyVolume;
    }

    public static void SetPlayerVolume(float volume)
    {
        playerVolume = volume;
        AudioListener.volume = playerVolume;
    }

    public static void SetInterfaceVolume(float volume)
    {
        interfaceVolume = volume;
        AudioListener.volume = interfaceVolume;
    }

    public static void SetGameSpeed(float speed)
    {
        gameSpeed = speed;
        Time.timeScale = gameSpeed;
    }

    // ========================================
    // Accessibility Control

    public static void SetPlayerAutoShoot(bool autoShoot)
    {
        playerAutoShoot = autoShoot;
    }

    // ========================================
    // Other Settings

    public static void SetPlayerInvincibility(bool invincible)
    {
        playerInvicible = invincible;
    }

    // ========================================
    // Reset Settings to Default

    public static void SetDefaultSettings()
    {
        // Set all settings to default values
        masterVolume = instance.defaultMasterVolume;
        musicVolume = instance.defaultMusicVolume;
        enemyVolume = instance.defaultEnemyVolume;
        playerVolume = instance.defaultPlayerVolume;
        interfaceVolume = instance.defaultInterfaceVolume;

        gameSpeed = instance.defaultGameSpeed;
        playerAutoShoot = instance.defaultPlayerAutoShoot;
        playerInvicible = instance.defaultPlayerInvicible;

        SaveSettings();
    }
}
