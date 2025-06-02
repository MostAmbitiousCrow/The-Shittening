using UnityEngine;
using UnityEngine.InputSystem;

public class Player_Controller : MonoBehaviour
{
    private PlayerInput playerInput;
    public int playerNumber;
    [SerializeField] Transform playerCameraPoint;
    [SerializeField] Vector2 moveDirection;
    [SerializeField] private Rigidbody rb;

    [Header("Player Data")]
    [SerializeField] float speed = 5;
    [SerializeField] float playerSensitivity = 1;

    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        playerNumber = playerInput.playerIndex;
        LockCursor(true);
        transform.position = GameManager.instance.playerSpawnpos;
    }

    public void LockCursor(bool state)
    {
        Cursor.lockState = state ? CursorLockMode.Locked : CursorLockMode.None;
    }

    // These methods are called by PlayerInput when the corresponding action is triggered
    public void OnMove(InputValue value)
    {
        moveDirection = value.Get<Vector2>();
        Debug.Log($"Player is Moving. Direction = {moveDirection}");
    }

    public void OnOpenBomb(InputValue value)
    {
        if (value.isPressed)
        {

        }
    }

    public void OnLook(InputValue value)
    {
        float mouseX = value.Get<Vector2>().x * playerSensitivity;
        float mouseY = value.Get<Vector2>().y * playerSensitivity;

        Vector3 newPlayerRotation = transform.localEulerAngles;
        newPlayerRotation.y += mouseX;
        transform.localRotation = Quaternion.AngleAxis(newPlayerRotation.y, Vector3.up);

        Vector3 newCameraRotation = playerCameraPoint.localEulerAngles;
        newCameraRotation.x -= mouseY;
        newCameraRotation.x = Mathf.Clamp(newCameraRotation.x, 0, 45);
        playerCameraPoint.localRotation = Quaternion.AngleAxis(newCameraRotation.x, Vector3.right);
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

    void FixedUpdate()
    {
        if (!GameData.isPaused || !GameManager.playerData[playerNumber].isDead)
        {
            moveDirection = transform.forward * moveDirection.y + transform.right * moveDirection.x;
            rb.AddForce(moveDirection.normalized * Global_Game_Speed.GetFixedDeltaTime() * speed * 10, ForceMode.Force);
        }
    }

    public void AssignCamera()
    {
        Transform cameraT = Camera.main.transform;
        cameraT.SetParent(playerCameraPoint);
        cameraT.SetLocalPositionAndRotation(new(), new());
    }
}
