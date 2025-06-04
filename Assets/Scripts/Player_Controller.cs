using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player_Controller : MonoBehaviour
{
    [Header("Player Components")]
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

    [Header("Bomb")]
    [SerializeField] bool bombOpen;
    [SerializeField] bool bombFlipped;
    [SerializeField] bool bombAnimating;
    [Space(10)]
    [SerializeField] Transform bombPivot;
    [SerializeField] Transform bomb;
    [SerializeField] float animationTime = .75f;
    [Space(10)]
    [SerializeField] Vector3 openedRotation = new(65, 0);
    [SerializeField] Vector3 closedRotation = new(0, 0);
    [Space(10)]
    [SerializeField] Vector3 timerViewRotation = new();
    [SerializeField] Vector3 keyViewRotation = new(0, 180 , 0);

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
        if (value.isPressed && !bombAnimating)
        {
            Debug.Log("Bomb Opened");
            OpenBomb();
        }
    }

    public void OnLook(InputValue value)
    {
        inputLook = value.Get<Vector2>();
    }

    public void OnFire(InputValue value)
    {
        if (value.isPressed)
        {
            if(bombOpen && !bombAnimating)
            {
                StartCoroutine(BombFlipTransition());
            }
        }
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

        Vector3 move = transform.forward * inputMove.y + transform.right * inputMove.x;
        Vector3 velocity = move.normalized * speed;
        rb.velocity = new Vector3(velocity.x, rb.velocity.y, velocity.z);
    }

    void Update()
    {
        if (GameData.isPaused) return;

        // Mouse look
        float mouseX = inputLook.x * playerSensitivity;
        float mouseY = inputLook.y * playerSensitivity;

        // Horizontal rotation
        transform.Rotate(Vector3.up * mouseX);

        // Vertical rotation
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

    public void OpenBomb()
    {
        bombOpen = !bombOpen;
        if(!bombAnimating)
            StartCoroutine(BombOpenTransition());
    }

    IEnumerator BombOpenTransition()
    {
        bombAnimating = true;
        float t = 0;
        Quaternion r;
        Vector3 lerp;

        while (t < animationTime)
        {
            t += Global_Game_Speed.GetDeltaTime();
            lerp = Vector3.Lerp(bombOpen ? closedRotation : openedRotation, bombOpen ? openedRotation : closedRotation, t);
            r = Quaternion.Euler(lerp.x, lerp.y, lerp.z);
            bombPivot.localRotation = r;
            yield return null;
        }
        lerp = Vector3.Lerp(bombFlipped ? keyViewRotation : timerViewRotation, bombFlipped ? timerViewRotation : keyViewRotation, 1);
        r = Quaternion.Euler(lerp.x, lerp.y, lerp.z);
        bomb.localRotation = r;

        bombAnimating = false;
        yield break;
    }

    IEnumerator BombFlipTransition()
    {
        bombAnimating = true;
        bombFlipped = !bombFlipped;
        float t = 0;
        Quaternion r;
        Vector3 lerp;

        while (t < animationTime)
        {
            t += Global_Game_Speed.GetDeltaTime();
            lerp = Vector3.Lerp(bombFlipped ? keyViewRotation : timerViewRotation, bombFlipped ? timerViewRotation : keyViewRotation, t);
            r = Quaternion.Euler(lerp.x, lerp.y, lerp.z);
            bomb.localRotation = r;
            yield return null;
        }
        lerp = Vector3.Lerp(bombFlipped ? keyViewRotation : timerViewRotation, bombFlipped ? timerViewRotation : keyViewRotation, 1);
        r = Quaternion.Euler(lerp.x, lerp.y, lerp.z);
        bomb.localRotation = r;

        bombAnimating = false;
        yield break;
    }
}
