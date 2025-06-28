using System;
using UnityEngine;
using UnityEngine.AI;

public class Enemy_StateMachine : MonoBehaviour
{
    public NavMeshAgent MeshAgent { get; private set; }
    public bool playerVisible;
    [SerializeField] LayerMask wallLayer;
    public LayerMask playerLayer;
    public Enemy_State CurrentState { set; get; }
    public PatrolState patrolState;
    public ChaseState chaseState;

    public Transform currentDestination;
    public float visibilityMeter = 5f; // seconds of lost sight before giving up
    public float visibilityMeterMax = 5f;
    public float patrolDelay = 1f;
    public bool playerIsBehind;
    public float attackRange = 2;

    [Space(10)]

    public Enemy_States state;
    public enum Enemy_States
    {
        Patrol, Chase
    }

    public enum Enemy_Stages { Stage_1, Stage_2, Stage_3, Stage_4 }
    public Enemy_Stages enemyStage;
    [Serializable]
    public struct StageData
    {
        [Tooltip("Front and Back sprites for this stage")] public Sprite[] sprites;
        [Tooltip("Front and Back sprites for this stages chase appearance")] public Sprite[] chaseSprites;
        [Space(10)]
        [Tooltip("Speed of the enemy during this stage")] public float speed;
    }
    public StageData[] stageDatas;

    [Space(10)]

    public SpriteRenderer frontSR;
    public SpriteRenderer backSR;
    // [Space(10)]

    // public Animator animator;

    void Awake() { patrolState = new PatrolState(this); chaseState = new ChaseState(this); }

    void Start()
    {
        MeshAgent = GetComponent<NavMeshAgent>();
        ChangeState(patrolState);
        UpdateStage(Enemy_Stages.Stage_1);
    }

    public void ChangeState(Enemy_State newState)
    {
        CurrentState?.Exit();
        CurrentState = newState;
        CurrentState?.Enter();
    }

    void Update()
    {
        if (GameData.isGameStarted)
        {
            DetectPlayer();
            GetPlayerDirection();
            CurrentState.LogicUpdate();
        }
    }

    void DetectPlayer()
    {
        Vector3 playerPos = GameManager.playerData[0].playerTransform.position;
        Debug.DrawLine(transform.position, playerPos, Color.yellow);
        // If linecast hits something in the wall layer, player is NOT visible
        if (!Physics.Linecast(transform.position, playerPos, wallLayer))
        {
            playerVisible = true;
        }
        else
        {
            playerVisible = false;
        }
    }

    void GetPlayerDirection()
    {
        Vector3 toPlayer = (GameManager.playerData[0].playerTransform.position - transform.position).normalized;
        float dot = Vector3.Dot(transform.forward, toPlayer);
        playerIsBehind = dot < 0f;

        backSR.enabled = playerIsBehind;
        frontSR.enabled = !playerIsBehind;
    }

    public void UpdateStage(Enemy_Stages targetStage)
    {
        if ((int)targetStage > 3) return;

        enemyStage = targetStage;

        // Update Front and Back Sprites:
        frontSR.sprite = stageDatas[(int)enemyStage].sprites[0];
        frontSR.sprite = stageDatas[(int)enemyStage].sprites[1];

        MeshAgent.speed = stageDatas[(int)enemyStage].speed;

        CurrentState.UpdateSpriteState(state);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}

public abstract class Enemy_State
{
    protected Enemy_StateMachine stateMachine;
    protected bool isExitingState;
    protected bool isAnimationFinished;

    public Enemy_State(Enemy_StateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
    }

    public virtual void Enter()
    {
        isAnimationFinished = false;
        isExitingState = false;
    }
    public virtual void Exit()
    {
        isExitingState = true;
        isAnimationFinished = true;
    }
    public virtual void LogicUpdate() { }

    public virtual void UpdateSpriteState(Enemy_StateMachine.Enemy_States state)
    {
        if (state == Enemy_StateMachine.Enemy_States.Patrol)
        {
            stateMachine.frontSR.sprite = stateMachine.stageDatas[(int)stateMachine.enemyStage].sprites[0];
            stateMachine.backSR.sprite = stateMachine.stageDatas[(int)stateMachine.enemyStage].sprites[1];
        }
        else if (state == Enemy_StateMachine.Enemy_States.Chase)
        {
            stateMachine.frontSR.sprite = stateMachine.stageDatas[(int)stateMachine.enemyStage].chaseSprites[0];
            stateMachine.backSR.sprite = stateMachine.stageDatas[(int)stateMachine.enemyStage].chaseSprites[1];
        }
    }
}

public class PatrolState : Enemy_State
{
    private readonly float arrivalThreshold = 1.0f;
    private float delayTime = 0;

    public PatrolState(Enemy_StateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        base.Enter();
        stateMachine.state = Enemy_StateMachine.Enemy_States.Patrol;
        AudioManager.PlayMusic(AudioManager.MusicOptions.Overlap, MusicCategory.MusicSoundTypes.AmbientMusic, 1);
        delayTime = 0;

        // If using Animator
        // stateMachine.animator.SetTrigger("Moving"); // Move

        UpdateSpriteState(Enemy_StateMachine.Enemy_States.Patrol);

        PickNewDestination();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        // If player is seen, switch to chase
        if (stateMachine.playerVisible && !stateMachine.playerIsBehind)
        {
            stateMachine.ChangeState(stateMachine.chaseState);
            return;
        }

        // Move to random destination
        if (stateMachine.currentDestination != null)
        {
            stateMachine.MeshAgent.destination = stateMachine.currentDestination.position;
            float dist = Vector3.Distance(stateMachine.transform.position, stateMachine.currentDestination.position);
            // Debug.Log($"Distance: {dist}");
            if (dist <= arrivalThreshold)
            {
                // If using Animator
                // stateMachine.animator.SetTrigger("Idle"); // Idle

                UpdateSpriteState(Enemy_StateMachine.Enemy_States.Patrol);

                delayTime += Global_Game_Speed.GetDeltaTime();
                // Debug.Log($"Delay Time: {delayTime}");
                if (delayTime > stateMachine.patrolDelay)
                {
                    delayTime = 0;
                    PickNewDestination();
                }
            }
        }
    }

    private void PickNewDestination()
    {
        System.Collections.Generic.List<Transform> patrols = Environment_Manager.instance.patrolDestinations;
        if (patrols == null || patrols.Count == 0) Debug.LogError("No Patrol Points");
        int id = UnityEngine.Random.Range(0, patrols.Count);
        stateMachine.currentDestination = patrols[id];
        stateMachine.MeshAgent.destination = stateMachine.currentDestination.position;

        // If using Animator
        // stateMachine.animator.ResetTrigger("Idle"); // Reset Idle
        // stateMachine.animator.SetTrigger("Moving"); // Move

        UpdateSpriteState(Enemy_StateMachine.Enemy_States.Patrol);
    }
}

public class ChaseState : Enemy_State
{
    private readonly float loseRate = 1.5f; // seconds to lose sight
    private readonly float gainRate = 2.5f; // seconds to fill meter
    private readonly float attackCooldown = 1;
    private float t = 0;
    private bool attacked = false;

    public ChaseState(Enemy_StateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        base.Enter();
        stateMachine.state = Enemy_StateMachine.Enemy_States.Chase;
        AudioManager.PlayMusic(AudioManager.MusicOptions.Overlap, MusicCategory.MusicSoundTypes.ChaseMusic, 1);
        
        // If using Animator
        // stateMachine.animator.SetTrigger("Moving"); // Move

        UpdateSpriteState(Enemy_StateMachine.Enemy_States.Chase);

        attacked = false;
        // TODO Play alert animation/sound
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        if (attacked)
        {
            t += Global_Game_Speed.GetDeltaTime();
            stateMachine.MeshAgent.isStopped = true;
            if (t > attackCooldown)
            {
                attacked = false;
                t = 0;
                stateMachine.MeshAgent.isStopped = false;
            }
            return;
        }
        Vector3 playerPos = GameManager.playerData[0].playerTransform.position;
        stateMachine.MeshAgent.destination = playerPos;

        // Visibility meter logic
        if (stateMachine.playerVisible)
        {
            stateMachine.visibilityMeter += gainRate * Global_Game_Speed.GetDeltaTime();
            if (stateMachine.visibilityMeter > stateMachine.visibilityMeterMax)
                stateMachine.visibilityMeter = stateMachine.visibilityMeterMax;
        }
        else
        {
            stateMachine.visibilityMeter -= loseRate * Global_Game_Speed.GetDeltaTime();
        }

        // If player not visible for too long, return to patrol
        if (stateMachine.visibilityMeter <= 0f)
        {
            stateMachine.visibilityMeter = 0f;
            stateMachine.ChangeState(stateMachine.patrolState);
        }

        float dist = Vector3.Distance(stateMachine.transform.position, playerPos);
        if (dist < stateMachine.attackRange && !attacked)
        {
            GameManager.playerData[0].playerController.Damage();
            attacked = true;

            // If using Animator
            // stateMachine.animator.SetTrigger("Attack");

            UpdateSpriteState(Enemy_StateMachine.Enemy_States.Chase);

            Debug.Log("Player Attacked");
        }
    }
}