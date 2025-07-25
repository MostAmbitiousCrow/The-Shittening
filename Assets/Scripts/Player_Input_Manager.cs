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

    public void CreatePlayerCharacter()
    {
        Instantiate(playerPrefab);
    }
    public void DestroyPlayerCharacter(int num = 0)
    {
        Destroy(GameManager.playerData[num].playerObject);
    }

    void OnEnable()
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
            playerInput = input,
            playerController = input.GetComponent<Player_Controller>()
        };
        Debug.Log($"Plater Data: {pd.playerObject} | {pd.playerTransform}");

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

        // UpdatePlayerEvents();
    }

    public void SetPlayerJoining(bool state)
    {
        if (state) playerInputManager.EnableJoining();
        else playerInputManager.DisableJoining();
    }

    void UpdatePlayerEvents()
    {
        GameManager.instance.EventSystem_UpdateUIActions();
        GameManager.playerData[0].playerController.AssignCamera(false);

        // GameManager.instance.StartGame();
        //<<< Update any events here
    }
}
