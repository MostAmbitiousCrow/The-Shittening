using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class Settings_Menu_Manager : MonoBehaviour // By Samuel White
{
    //========================================
    // Settings Menu Content. 
    // Shared with Main Menu Manager and the Gameplay UI.
    //========================================

    [Header("Volume Scrollers")]
    // Master = 0 | Music = 1 | Enemy = 2 | Player = 3 | Interface = 4 |
    public VolumeSliderData[] volumeSliderDatas = new VolumeSliderData[5]; // See below for example values
    [System.Serializable]
    public struct VolumeSliderData
    {
        public string name;
        public Slider slider;
        public TextMeshProUGUI text;
    }

    [Header("Accessibility Components")]
    // Game Speed = 0 |
    public AccessibilitySliderData[] accessibilitySliderDatas = new AccessibilitySliderData[1]; // See below for example values
    [System.Serializable]
    public struct AccessibilitySliderData
    {
        public string name;
        public Slider slider;
        public TextMeshProUGUI text;
    }
    [SerializeField] private Toggle[] accessibilityToggles; // Player Invincible = 0

    [Header("Menus")]
    [SerializeField] MenuData[] menuDatas; // Main Menu = 0 | Settings Menu = 1| Volume = 2| Accessibility = 3 |
    [System.Serializable]
    public struct MenuData
    {
        public string name;
        public GameObject menu;
        public GameObject enterButton;
    }

    private bool sliderSelected;

    #region Settings Menu Content
    // ============================= Settings Menu Content =============================

    public void SaveSettings() => Settings_Manager.SaveSettings();

    public void EnableMenu(int menuID)
    {
        menuDatas[menuID].menu.SetActive(true);
        if (menuDatas[menuID].enterButton != null)
        {
            if (GameData.isMultiplayer) GameManager.instance.multiplayerEventSystem.SetSelectedGameObject(menuDatas[menuID].enterButton);
            else GameManager.instance.eventSystem.SetSelectedGameObject(menuDatas[menuID].enterButton);
        }
    }

    public void DisableMenu(int menuID) => menuDatas[menuID].menu.SetActive(false);

    // Volume Control

    public void MasterVolumeSlider()
    {
        Settings_Manager.masterVolume = Mathf.Clamp(volumeSliderDatas[0].slider.value / 10, .0001f, 1);
        float v = volumeSliderDatas[0].slider.value / 10;
        volumeSliderDatas[0].text.text = v.ToString();
        AudioManager.UpdateAudioManagerVolume();
    }

    public void MusicVolumeSlider()
    {
        Settings_Manager.musicVolume = Mathf.Clamp(volumeSliderDatas[1].slider.value / 10, .0001f, 1);
        float v = volumeSliderDatas[1].slider.value / 10;
        volumeSliderDatas[1].text.text = v.ToString();
        AudioManager.UpdateAudioManagerVolume();
    }

    public void EnemyVolumeSlider()
    {
        Settings_Manager.enemyVolume = Mathf.Clamp(volumeSliderDatas[2].slider.value / 10, .0001f, 1);
        float v = volumeSliderDatas[2].slider.value / 10;
        volumeSliderDatas[2].text.text = v.ToString();
        AudioManager.UpdateAudioManagerVolume();
    }

    public void PlayerVolumeSlider()
    {
        Settings_Manager.playerVolume = Mathf.Clamp(volumeSliderDatas[3].slider.value / 10, .0001f, 1);
        float v = volumeSliderDatas[3].slider.value / 10;
        volumeSliderDatas[3].text.text = v.ToString();
        AudioManager.UpdateAudioManagerVolume();
    }

    public void InterfaceVolumeSlider()
    {
        Settings_Manager.interfaceVolume = Mathf.Clamp(volumeSliderDatas[4].slider.value / 10, .0001f, 1);
        float v = volumeSliderDatas[4].slider.value / 10;
        volumeSliderDatas[4].text.text = v.ToString();
        AudioManager.UpdateAudioManagerVolume();
    }

    // Accessibility Triggers

    //public void GameSpeedSlider()
    //{
    //    Settings_Manager.SetGameSpeed(Mathf.Clamp(accessibilitySliderDatas[0].slider.value / 10, .1f, 1f));
    //    float v = accessibilitySliderDatas[0].slider.value / 10;
    //    accessibilitySliderDatas[0].text.text = v.ToString();
    //}

    public void CursorSensitivitySlider()
    {
        float value = Mathf.Clamp(accessibilitySliderDatas[0].slider.value, .1f, 1);
        Settings_Manager.SetCursorSensitivity(value);
        value = Mathf.Round(value * 100) / 100;
        accessibilitySliderDatas[0].text.text = value.ToString();
    }

    //public void AutoShoot()
    //{
    //    Settings_Manager.SetPlayerAutoShoot(accessibilityToggles[0].isOn);
    //}

    public void PlayerInvincible()
    {
        Settings_Manager.SetPlayerInvincibility(accessibilityToggles[0].isOn);
    }

    // ============================= Other Stuff =============================

    // Reset Settings
    public void ResetSettings()
    {
        Settings_Manager.SetDefaultSettings();
        UpdateUI();
    }
    #endregion

    public void UpdateUI()
    {
        // Update UI to Current Settings
        volumeSliderDatas[0].slider.value = (int)(Settings_Manager.masterVolume * 10);
        volumeSliderDatas[1].slider.value = (int)(Settings_Manager.musicVolume * 10);
        volumeSliderDatas[2].slider.value = (int)(Settings_Manager.enemyVolume * 10);
        volumeSliderDatas[3].slider.value = (int)(Settings_Manager.playerVolume * 10);
        volumeSliderDatas[4].slider.value = (int)(Settings_Manager.interfaceVolume * 10);

        //accessibilitySliderDatas[0].slider.value = Mathf.Clamp((int)(Settings_Manager.gameSpeed * 10), 1f, 10f);
        float value = Mathf.Round(Settings_Manager.cursorSensitivity * 100) / 100;
        accessibilitySliderDatas[0].text.text = value.ToString();
        accessibilitySliderDatas[0].slider.value = value;


        accessibilityToggles[0].isOn = Settings_Manager.playerInvicible;

        for (int i = 0; i < volumeSliderDatas.Length; i++)
        {
            float v = volumeSliderDatas[i].slider.value / 10;
            volumeSliderDatas[i].text.text = v.ToString();
        }
        for (int i = 0; i < accessibilitySliderDatas.Length; i++)
        {
            float v = accessibilitySliderDatas[i].slider.value / 10;
            accessibilitySliderDatas[i].text.text = v.ToString();
        }
    }

    #region Play Sounds
    public void PlaySound_UIHover() => AudioManager.PlayInterfaceSound(InterfaceCategory.InterfaceSoundTypes.Button_Hover, .5f);
    public void PlaySound_UIPress() => AudioManager.PlayInterfaceSound(InterfaceCategory.InterfaceSoundTypes.Button_Press, .5f);
    public void PlaySound_UIBack() => AudioManager.PlayInterfaceSound(InterfaceCategory.InterfaceSoundTypes.Button_Back, .5f);
    public void PlaySound_UIStartGame() => AudioManager.PlayInterfaceSound(InterfaceCategory.InterfaceSoundTypes.Button_GameStart, .5f);

    #endregion

#if UNITY_EDITOR
    [ContextMenu("Create Default Slider Datas")]
    private void PopulateDefaultSliderData()
    {
        volumeSliderDatas = new VolumeSliderData[5];
        volumeSliderDatas[0].name = "Master";
        volumeSliderDatas[1].name = "Music";
        volumeSliderDatas[2].name = "Enemy";
        volumeSliderDatas[3].name = "Player";
        volumeSliderDatas[4].name = "Interface";

        accessibilitySliderDatas = new AccessibilitySliderData[1];
        accessibilitySliderDatas[0].name = "Cursor Speed";

        menuDatas = new MenuData[4];
        menuDatas[0].name = "Main Menu";
        menuDatas[1].name = "Settings Menu";
        menuDatas[2].name = "Volume";
        menuDatas[3].name = "Accessibility";
    }
#endif

    // // ============================= Input Actions ============================= // Ignore
    // #region Input Actions
    // private InputAction submitAction;
    // private InputAction navigateAction;
    // private InputAction cancelAction;

    // void Awake()
    // {
    //     // Get actions by name or map
    //     GameManager.playerData[0].playerInput.
    //     submitAction = GameManager.instance.defaultInputActions.FindAction("Submit");
    //     navigateAction = GameManager.instance.defaultInputActions.FindAction("Navigate");
    //     cancelAction = GameManager.instance.defaultInputActions.FindAction("Cancel");
    // }

    // void OnEnable()
    // {
    //     // submitAction = GameManager.playerData[0].playerInput.currentActionMap.actionTriggered<>
    //     submitAction = GameManager.instance.eventSystem.currentInputModule.inputOverride.
    //     submitAction.performed += OnSubmit;
    //     // navigateAction.performed += OnNavigate;
    //     cancelAction.performed += OnCancel;
    //     submitAction.Enable();
    //     navigateAction.Enable();
    //     cancelAction.Enable();
    // }

    // void OnDisable()
    // {
    //     submitAction.performed -= OnSubmit;
    //     // navigateAction.performed -= OnNavigate;
    //     cancelAction.performed -= OnCancel;
    //     submitAction.Disable();
    //     navigateAction.Disable();
    //     cancelAction.Disable();
    // }

    // private void OnSubmit(InputAction.CallbackContext context)
    // {
    //     // If slider not already selected, select the current slider
    //     if (!sliderSelected)
    //     {
    //         Slider s = null;
    //         if (GameData.isMultiplayer)
    //         {
    //             if (GameManager.instance.multiplayerEventSystem.currentSelectedGameObject != null &&
    //                 GameManager.instance.multiplayerEventSystem.currentSelectedGameObject.TryGetComponent(out Slider sliderComponent))
    //             {
    //                 s = sliderComponent;
    //             }
    //         }
    //         else
    //         {
    //             if (GameManager.instance.eventSystem.currentSelectedGameObject != null &&
    //                 GameManager.instance.eventSystem.currentSelectedGameObject.TryGetComponent(out Slider sliderComponent))
    //             {
    //                 s = sliderComponent;
    //             }
    //         }

    //         sliderSelected = true;
    //         GameManager.instance.EventSystem_SelectUIButton(s.gameObject);
    //         Navigation nav = s.navigation;
    //         nav.mode = Navigation.Mode.None;
    //         s.navigation = nav;
    //     }

    // }

    // private void OnCancel(InputAction.CallbackContext context)
    // {
    //     if (sliderSelected)
    //     {
    //         sliderSelected = false;
    //         GameManager.instance.EventSystem_SelectUIButton(null);
    //     }
    // }
    // #endregion
}