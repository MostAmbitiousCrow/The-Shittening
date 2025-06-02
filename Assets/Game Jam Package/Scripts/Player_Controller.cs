using UnityEngine;
using UnityEngine.InputSystem;

public class Player_Controller : MonoBehaviour
{
    private PlayerInput playerInput;
    public int playerNumber;

    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        playerNumber = playerInput.playerIndex;
    }

    // These methods are called by PlayerInput when the corresponding action is triggered
    public void OnMove(InputValue value)
    {
        Vector2 move = value.Get<Vector2>();
        Debug.Log($"Player is Moving. Direction = {move}");
    }

    public void OnJump(InputValue value)
    {
        if (value.isPressed)
            Debug.Log("Jumped!");
    }

    public void OnFire(InputValue value)
    {
        if (value.isPressed)
            Debug.Log("Did an action!");
    }

    public void OnPause(InputValue value)
    {
        if (value.isPressed && Player_Game_UI_Manager.instance != null)
            Player_Game_UI_Manager.instance.ShowPauseMenu(!GameData.isPaused, playerNumber);
    }

    void Update()
    {
        if (!GameData.isPaused || !GameManager.playerData[playerNumber].isDead)
        {
            // Movement logic can be handled here if needed :)
        }
    }
}
