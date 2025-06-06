using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Player_Game_UI_Manager : MonoBehaviour // By Samuel White
{
    //========================================
    // Used to manage the in-game gameplay and menu UI.
    // Pause menu, results menu, Game UI
    //========================================

    public static Player_Game_UI_Manager instance;

    [Header("===========Player UI Content===========")]
    [SerializeField] GameObject[] playerGameUI;

    [Space(10)]

    [SerializeField] Image[] playerHealthBars; // 0 = Player 1, 1 = Player 2
    [SerializeField] Image damageFlashImage;
    [SerializeField] Color startColor = Color.red;
    [SerializeField] Color endColor = Color.red;
    [Space(10)]
    [SerializeField] Color explosionStartColor = Color.yellow;
    [SerializeField] Color explosionEndColor = Color.black;
    
    [Space(10)]
    
    [SerializeField] GameObject[] playerTwoStats;

    [Space(10)]

    [Header("===========Game UI Content===========")]
    [SerializeField] GameObject pauseMenu;
    [Space(10)]
    [SerializeField] GameObject resultsScreen;
    [SerializeField] GameObject[] resultTitles; // 0 = BlownUp, 1 = Killed, 2 = Escaped 
    [SerializeField] Image resultsBackground;
    [SerializeField] Sprite[] resultsBackgrounds; // 0 = BlownUp, 1 = Killed, 2 = Escaped 
    [Space(10)]
    [SerializeField] GameObject settingsMenu;

    [Space(10)]

    [Header("===========Game UI Content===========")]
    [SerializeField] GameObject pause_resumeButton;
    [Space(10)]
    [SerializeField] GameObject results_MainMenuButton;

    [Space(10)]

    [Header("===========Settings UI Content===========")]
    [SerializeField] GameObject settingsExitButton;

    [Header("Player Stats")]
    public PlayerStatContent[] playerStats = new PlayerStatContent[2]; // 0 = Player 1, 1 = Player 2
    [System.Serializable]
    public struct PlayerStatContent
    {
        public TextMeshProUGUI Kills, Lives, Time, Score;
    }

    private int playerNum; // The Player Number who currently has the UI open
    private Coroutine coroutine;

    void Awake()
    {
        if (instance == null) instance = this;
        ResetGameUI();
    }

    #region Update Player UI

    public void UpdateUI()
    {
        int count = Mathf.Min(playerHealthBars.Length, GameManager.playerData.Count);
        for (int i = 0; i < count; i++)
        {
            // Update Player Health bar here (if applicable)

            float minHealth = GameManager.playerData[i].playerController.Health;
            float maxHealth = GameManager.playerData[i].playerController.MaxHealth;
            playerHealthBars[i].fillAmount = minHealth / maxHealth;
            if (minHealth <= 0) playerHealthBars[i].fillAmount = 0f;
        }
    }
    #endregion

    #region Damage Event
    public void DamageFlashEvent(float time = .25f)
    {
        UpdateUI();
        coroutine = StartCoroutine(DamageFlash(time));
    }

    IEnumerator DamageFlash(float time)
    {
        float t = 0;
        while (t < 1)
        {
            damageFlashImage.color = Color.Lerp(startColor, endColor, t);
            t += Global_Game_Speed.GetDeltaTime() / time;
            yield return null;
        }
        damageFlashImage.color = endColor;
        yield break;
    }
    #endregion

    #region Explosion Event

    public IEnumerator ExplosionFlash(float time = .5f)
    {
        float t = 0;
        if(coroutine != null)
            StopCoroutine(coroutine);

        while (t < 1)
        {
            damageFlashImage.color = Color.Lerp(explosionStartColor, explosionEndColor, t);
            t += Global_Game_Speed.GetDeltaTime() / time;
            yield return null;
        }
        damageFlashImage.color = explosionEndColor;
        yield break;
    }
    #endregion

    #region Show Menus
    public void UI_ShowPauseMenu(bool show)
    {
        settingsMenu.SetActive(false);
        ShowPauseMenu(show, playerNum);
    }

    public void ShowPauseMenu(bool show, int pNum)
    {
        playerNum = pNum;

        if (!GameData.canPause) return;
        pauseMenu.SetActive(show);
        GameManager.instance.PauseGame(show);
        GameManager.instance.Player_ChangeActionMap(true, 0, show ? "UI" : "Player");
        GameManager.instance.EventSystem_SelectUIButton(pause_resumeButton);
        AudioManager.UpdateMusic(show ? AudioManager.MusicOptions.Pause : AudioManager.MusicOptions.Resume);
    }

    public enum GameOverEvent { BlownUp, Killed, Escaped }
    public void ShowEndScreen(GameOverEvent gameOverEvent)
    {
        resultsBackground.sprite = resultsBackgrounds[(int)gameOverEvent];
        resultsScreen.SetActive(true);
        GameManager.instance.Player_ChangeActionMap(false, 0, "UI");
        GameManager.instance.EventSystem_SelectUIButton(results_MainMenuButton);
    }
    #endregion

    public void ShowSettings(bool show)
    {
        settingsMenu.SetActive(show);
        GameObject b = show ? settingsExitButton : pause_resumeButton;
        GameManager.instance.EventSystem_SelectUIButton(b);
    }

    #region Reset UI

    public void ResetGameUI()
    {
        foreach (var healthBar in playerHealthBars)
        {
            healthBar.fillAmount = 1f;
        }

        playerGameUI[1].SetActive(GameData.isMultiplayer);
    }
    #endregion

    public void NextLevel()
    {
        int nextLevel = (int)GameData.currentLevel + 1;
        GameData.currentLevel = (Scene_Loader_Transition.SceneNames)nextLevel;

        Scene_Loader_Transition.LoadScene(GameData.currentLevel);
        Debug.Log($"Next Level: {GameData.currentLevel}");
    }

    public void ReturnToMainMenu()
    {
        GameData.currentLevel = Scene_Loader_Transition.SceneNames.Main_Menu;
        Scene_Loader_Transition.LoadScene(Scene_Loader_Transition.SceneNames.Main_Menu);
    }
}
