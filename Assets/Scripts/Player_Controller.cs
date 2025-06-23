using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player_Controller : MonoBehaviour
{
    [Header("Player Components")]
    private PlayerInput playerInput;
    public int playerNumber;
    [SerializeField] Transform playerCameraPoint;
    [SerializeField] Rigidbody rb;
    [SerializeField] TextMeshPro bombText;

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
    [SerializeField] Vector3 timerViewRotation = new();
    [SerializeField] Vector3 keyViewRotation = new(0, 180, 0);
    [Space(10)]
    [SerializeField] GameObject[] bombKeys;

    [Header("Health")]
    [SerializeField] float damageFlashTime = .25f;
    public int Health { get; private set; } = 3;
    public int MaxHealth { get; private set; } = 3;

    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        playerNumber = playerInput.playerIndex;
        Health = MaxHealth;
    }

    public void UpdateSpawn(Vector3 vector)
    {
        rb.velocity = new();
        transform.position = GameManager.playerData[0].playerTransform.position = vector;
        Debug.Log(vector);
        Debug.Log(transform.position);
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
            // OpenBomb();
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
            if (bombOpen && !bombAnimating)
            {
                StartCoroutine(BombFlipTransition());
            }
        }
    }

    #region Pause
    public void OnPause(InputValue value)
    {
        if (value.isPressed && Player_Game_UI_Manager.instance != null)
        {
            bool paused = !GameData.isPaused;
            Player_Game_UI_Manager.instance.ShowPauseMenu(paused, playerNumber);
            LockCursor(!paused);
        }
    }
    #endregion

    #region Movement
    void FixedUpdate()
    {
        if (GameData.isPaused || GameManager.playerData[playerNumber].isDead)
            return;

        Vector3 move = transform.forward * inputMove.y + transform.right * inputMove.x;
        Vector3 velocity = move.normalized * speed;
        rb.velocity = new Vector3(velocity.x, rb.velocity.y, velocity.z);
    }

    #endregion

    #region Camera Look
    void Update()
    {
        if (GameData.isPaused) return;

        // Mouse look
        float mouseX = inputLook.x * Settings_Manager.cursorSensitivity;
        float mouseY = inputLook.y * Settings_Manager.cursorSensitivity;

        // Horizontal rotation
        transform.Rotate(Vector3.up * mouseX);

        // Vertical rotation
        cameraPitch -= mouseY;
        cameraPitch = Mathf.Clamp(cameraPitch, -80f, 80f);
        playerCameraPoint.localEulerAngles = new Vector3(cameraPitch, 0f, 0f);
    }
    #endregion

    public void AssignCamera(bool state)
    {
        if (!state)
        {
            if (playerCameraPoint.childCount > 0 && playerCameraPoint.GetChild(0) != null)
                Destroy(playerCameraPoint.GetChild(0).gameObject);
        }
        else
        {
            Transform cameraT = Camera.main.transform;
            cameraT.SetParent(playerCameraPoint);
            cameraT.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        }
    }

    #region Bomb Controls
    // public void OpenBomb()
    // {
    //     bombOpen = !bombOpen;
    //     if (!bombAnimating)
    //         StartCoroutine(BombOpenTransition());
    // }

    // IEnumerator BombOpenTransition()
    // {
    //     bombAnimating = true;
    //     float t = 0;
    //     Quaternion r;
    //     Vector3 lerp;

    //     while (t < animationTime)
    //     {
    //         t += Global_Game_Speed.GetDeltaTime();
    //         lerp = Vector3.Lerp(bombOpen ? closedRotation : openedRotation, bombOpen ? openedRotation : closedRotation, t);
    //         r = Quaternion.Euler(lerp.x, lerp.y, lerp.z);
    //         bombPivot.localRotation = r;
    //         yield return null;
    //     }
    //     lerp = Vector3.Lerp(bombFlipped ? keyViewRotation : timerViewRotation, bombFlipped ? timerViewRotation : keyViewRotation, 1);
    //     r = Quaternion.Euler(lerp.x, lerp.y, lerp.z);
    //     bomb.localRotation = r;

    //     bombAnimating = false;
    //     yield break;
    // }

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
    #endregion

    #region Bomb Timer
    public void UpdateBomb(float time, int key, bool isKey = false)
    {
        // Clamp to non-negative
        time = Mathf.Max(0, time);

        // Get minutes, seconds, and milliseconds
        int minutes = (int)(time / 60);
        int seconds = (int)(time % 60);

        // Format as analog-like string
        bombText.text = $"{minutes:00}:{seconds:00}";

        if(isKey)
            bombKeys[key].SetActive(true);
    }
    #endregion

    #region Health and Damage
    public void Damage()
    {
        Health--;
        if (Health <= 0)
        {
            GameManager.instance.GameOver(Player_Game_UI_Manager.GameOverEvent.Killed);
            return;
        }
        Player_Game_UI_Manager.instance.DamageFlashEvent();
    }
    #endregion
}
