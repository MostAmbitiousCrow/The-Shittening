using UnityEngine;

public class Main_Menu_Manager : MonoBehaviour // By Samuel White
{
    //========================================
    // Main Menu Manager:
    // Manages the toggles between menus
    // Also triggers the Start Game Functions
    //========================================

    #region Variables
    [Header("Transition Components")]
    [SerializeField] Settings_Menu_Manager settingsMenuManager;

    [Header("Menu Components")]

    [SerializeField] MenuData[] menuDatas;
    [System.Serializable]
    public struct MenuData
    {
        public string name;
        public GameObject menu;
        public GameObject enterButton;
    }

    #endregion

    void Start()
    {
        settingsMenuManager.UpdateUI();
        AudioManager.PlayMusic(AudioManager.MusicOptions.Play, 1, 2, MusicCategory.MusicSoundTypes.MainMenu);
        if (settingsMenuManager.gameObject.activeSelf) settingsMenuManager.gameObject.SetActive(false);
    }

    #region Menu Navigation
    // ============================= Menu Navigation =============================
    // 0 = Main Menu | 1 = Settings Menu | 2 = Credits Menu | 3 = none

    public void OpenMenu(int newMenu)
    {
        menuDatas[newMenu].menu.SetActive(true);
        Debug.Log($"Activated: {menuDatas[newMenu].menu}");
        if (GameData.isMultiplayer)
            GameManager.instance.multiplayerEventSystem.SetSelectedGameObject(menuDatas[newMenu].enterButton);
        else GameManager.instance.eventSystem.SetSelectedGameObject(menuDatas[newMenu].enterButton);
    }

    public void CloseMenu(int oldMenu)
    {
        if(oldMenu < 4)
        menuDatas[oldMenu].menu.SetActive(false);
    }

    // Quit Game
    public void QuitGame()
    {
        Application.Quit(); // Quit the game
        Debug.Log("Player has quit the game.");
    }
    #endregion

    public void PlayGame()
    {
        // Load the game scene
        AudioManager.PlayMusic(AudioManager.MusicOptions.Stop, 1, 0, MusicCategory.MusicSoundTypes.None);
        GameData.currentLevel = Scene_Loader_Transition.SceneNames.Level_1;
        Scene_Loader_Transition.LoadScene(GameData.currentLevel);
    }

    #region Play Sounds
    public void PlaySound_UIHover() => AudioManager.PlayInterfaceSound(InterfaceCategory.InterfaceSoundTypes.Button_Hover, .5f);
    public void PlaySound_UIPress() => AudioManager.PlayInterfaceSound(InterfaceCategory.InterfaceSoundTypes.Button_Press, .5f);
    public void PlaySound_UIBack() => AudioManager.PlayInterfaceSound(InterfaceCategory.InterfaceSoundTypes.Button_Back, .5f);
    public void PlaySound_UIStartGame() => AudioManager.PlayInterfaceSound(InterfaceCategory.InterfaceSoundTypes.Button_GameStart, .5f);

    #endregion

    #if UNITY_EDITOR
    [ContextMenu("Create Menu Names")]
    private void PopulateDefaultSliderData()
    {
        menuDatas = new MenuData[3];
        menuDatas[0].name = "Main Menu";
        menuDatas[1].name = "Settings Menu";
        menuDatas[2].name = "Credits Menu";
    }
    #endif
}
