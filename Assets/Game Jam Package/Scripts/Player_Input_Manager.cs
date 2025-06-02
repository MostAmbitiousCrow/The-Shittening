using UnityEngine;
using UnityEngine.InputSystem;

public class Player_Input_Manager : MonoBehaviour
{
    public static Player_Input_Manager instance;
    [SerializeField] PlayerInputManager playerInputManager;
    [SerializeField] GameObject playerPrefab;

    void Awake()
    {
        instance = this;
        if (playerInputManager == null) playerInputManager = GetComponent<PlayerInputManager>();
        playerInputManager.playerPrefab = playerPrefab;
    }

    private void OnEnable()
    {
        playerInputManager.onPlayerJoined += PlayerJoined;
        playerInputManager.onPlayerLeft += PlayerDisconnected;
    }

    void OnDisable()
    {
        playerInputManager.onPlayerJoined -= PlayerJoined;
        playerInputManager.onPlayerLeft -= PlayerDisconnected;  
    }

    void PlayerJoined(PlayerInput input)
    {
        PlayerData pd = new()
        {
            playerObject = input.gameObject,
            playerTransform = input.transform,
            playerInput = input
        };

        GameManager.playerData.Add(pd);

        pd.playerTransform.parent = transform; // Child player object to the player folder

        // Use "Player" in levels, "UI" in Main Menu
        if (GameData.currentLevel == Scene_Loader_Transition.SceneNames.Main_Menu)
            GameManager.instance.Player_ChangeActionMap(false, input.playerIndex, "UI");
        else
            GameManager.instance.Player_ChangeActionMap(false, input.playerIndex, "Player");

        UpdatePlayerEvents();
    }

    void PlayerDisconnected(PlayerInput input)
    {
        Destroy(GameManager.playerData[input.playerIndex].playerObject);
        GameManager.playerData.RemoveAt(input.playerIndex);

        UpdatePlayerEvents();
    }

    public void SetPlayerJoining(bool state)
    {
        if (state) playerInputManager.EnableJoining();
        else playerInputManager.DisableJoining();
    }

    void UpdatePlayerEvents()
    {
        GameManager.instance.EventSystem_UpdateUIActions();
        //<<< Update any events here
    }
}
