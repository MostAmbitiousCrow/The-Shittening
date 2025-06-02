using UnityEngine;
using UnityEngine.InputSystem;

public class Player_Controller : MonoBehaviour
{
    private PlayerInput playerInput;
    public int playerNumber;
    [SerializeField] Transform playerCameraPoint;
    [SerializeField] private Rigidbody rb;

    [Header("Player Data")]
    [SerializeField] float speed = 5;
    [SerializeField] float playerSensitivity = 1;

    private Vector2 inputMove;
    private Vector2 inputLook;
    private float cameraPitch = 0f;

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
        Cursor.visible = !state;
    }

    // Input callbacks
    public void OnMove(InputValue value)
    {
        inputMove = value.Get<Vector2>();
    }

    public void OnOpenBomb(InputValue value)
    {
        if (value.isPressed)
        {
            Debug.Log("Bomb Opened");
        }
    }

    public void OnLook(InputValue value)
    {
        inputLook = value.Get<Vector2>();
    }

    public void OnFire(InputValue value)
    {
        if (value.isPressed)
            Debug.Log("Did an action!");
    }

    public void OnPause(InputValue value)
    {
        if (value.isPressed && Player_Game_UI_Manager.instance != null)
        {
            bool paused = !GameData.isPaused;
            Player_Game_UI_Manager.instance.ShowPauseMenu(paused, playerNumber);
            LockCursor(!paused);
        }
    }

    void FixedUpdate()
    {
        if (GameData.isPaused || GameManager.playerData[playerNumber].isDead)
            return;

        // Calculate movement direction in world space
        Vector3 move = transform.forward * inputMove.y + transform.right * inputMove.x;
        Vector3 velocity = move.normalized * speed;
        rb.velocity = new Vector3(velocity.x, rb.velocity.y, velocity.z); // preserve y velocity (gravity/jumping)
    }

    void Update()
    {
        if (GameData.isPaused) return;

        // Mouse look
        float mouseX = inputLook.x * playerSensitivity;
        float mouseY = inputLook.y * playerSensitivity;

        // Horizontal rotation (yaw)
        transform.Rotate(Vector3.up * mouseX);

        // Vertical rotation (pitch)
        cameraPitch -= mouseY;
        cameraPitch = Mathf.Clamp(cameraPitch, -80f, 80f);
        playerCameraPoint.localEulerAngles = new Vector3(cameraPitch, 0f, 0f);
    }

    public void AssignCamera()
    {
        Transform cameraT = Camera.main.transform;
        cameraT.SetParent(playerCameraPoint);
        cameraT.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
    }
}
