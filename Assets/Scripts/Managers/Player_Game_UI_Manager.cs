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

    [SerializeField] Image playerHealthBar;
    [SerializeField] TMP_Text objectiveText;
    [SerializeField] Image damageFlashImage;
    [SerializeField] Color startColor = Color.red;
    [SerializeField] Color endColor = Color.red;
    [Space(10)]
    [SerializeField] Color explosionStartColor = Color.yellow;
    [SerializeField] Color explosionEndColor = Color.black;

    [Header("===========Game UI Content===========")]
    [SerializeField] GameObject pauseMenu;
    [Space(10)]
    [SerializeField] GameObject resultsScreen;
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


    private Coroutine coroutine;

    void Awake()
    {
        if (instance == null) instance = this;
        ResetGameUI();
    }

    #region Update Player UI

    public void UpdateUI()
    {
       float minHealth = GameManager.playerData[0].playerController.Health;
        float maxHealth = GameManager.playerData[0].playerController.MaxHealth;
        playerHealthBar.fillAmount = minHealth / maxHealth;
        if (minHealth <= 0) playerHealthBar.fillAmount = 0f;
    }
    #endregion

    #region Update Objective
    public void UpdateObjective(string t)
    {
        objectiveText.text = t;
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
        ShowPauseMenu(show, 0);
    }

    public void ShowPauseMenu(bool show, int pNum)
    {
        if (!GameData.canPause) return;
        pauseMenu.SetActive(show);
        GameManager.instance.PauseGame(show);
        GameManager.instance.Player_ChangeActionMap(true, 0, show ? "UI" : "Player");
        GameManager.playerData[0].playerController.LockCursor(!show);
        GameManager.instance.EventSystem_SelectUIButton(pause_resumeButton);
        AudioManager.UpdateMusic(show ? AudioManager.MusicOptions.Pause : AudioManager.MusicOptions.Resume);
    }

    public enum GameOverEvent { BlownUp, Killed, Escaped }
    public void ShowEndScreen(GameOverEvent gameOverEvent)
    {
        resultsBackground.sprite = resultsBackgrounds[(int)gameOverEvent];
        Debug.Log($"End Screen: {gameOverEvent} | Num = {(int)gameOverEvent}");
        resultsScreen.SetActive(true);
        GameManager.playerData[0].playerController.LockCursor(false);
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
        playerHealthBar.fillAmount = 1;
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

        GameData.isGameStarted = false;
        GameData.isGameOver = false;
        GameData.isGameFinished = false;
        GameData.isPaused = false;
        GameData.canPause = false;

        Scene_Loader_Transition.LoadScene(Scene_Loader_Transition.SceneNames.Main_Menu);
    }
}
