using UnityEngine;
using UnityEngine.AI;

public class Enemy_StateMachine : MonoBehaviour
{
    public NavMeshAgent meshAgent;
    public bool playerVisible;
    [SerializeField] LayerMask layerMask;
    public Enemy_State CurrentState { set; get; }
    public PatrolState patrolState;
    public ChaseState chaseState;

    public Transform currentDestination;
    public float visibilityMeter = 5f; // seconds of lost sight before giving up
    public float visibilityMeterMax = 5f;
    public float patrolDelay = 1f;

    void Awake()
    {
        patrolState = new PatrolState(this);
        chaseState = new ChaseState(this);
    }

    void Start()
    {
        // if (Environment_Manager.instance != null)
        //     Environment_Manager.instance.SendMessage("GetPatrols");
        ChangeState(patrolState);
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
            CurrentState.LogicUpdate();
        }
    }

    void DetectPlayer()
    {
        Vector3 playerPos = GameManager.playerData[0].playerTransform.position;
        // If linecast hits something in the wall layer, player is NOT visible
        if (!Physics.Linecast(transform.position, playerPos, layerMask))
        {
            playerVisible = true;
        }
        else
        {
            playerVisible = false;
        }
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
    public virtual void PhysicsUpdate() { }
    public virtual void TransitionChecks() { }
    public virtual void AnimationTrigger() { isAnimationFinished = true; }
}

public class PatrolState : Enemy_State
{
    private readonly float arrivalThreshold = 1.0f;
    private float delayTime = 0;

    public PatrolState(Enemy_StateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        base.Enter();
        PickNewDestination();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        // If player is seen, switch to chase
        if (stateMachine.playerVisible)
        {
            stateMachine.ChangeState(stateMachine.chaseState);
            return;
        }

        // Move to current destination
        if (stateMachine.currentDestination != null)
        {
            stateMachine.meshAgent.destination = stateMachine.currentDestination.position;
            float dist = Vector3.Distance(stateMachine.transform.position, stateMachine.currentDestination.position);
            Debug.Log($"Distance: {dist}");
            if (dist < arrivalThreshold)
            {
                delayTime += Global_Game_Speed.GetDeltaTime();
                Debug.Log($"Delay Time: {delayTime}");
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
        if (patrols == null || patrols.Count == 0) return;
        int id = Random.Range(0, patrols.Count);
        stateMachine.currentDestination = patrols[id];
        stateMachine.meshAgent.destination = stateMachine.currentDestination.position;
    }
}

public class ChaseState : Enemy_State
{
    private readonly float loseRate = 1.5f; // seconds to lose sight
    private readonly float gainRate = 2.5f; // seconds to fill meter

    public ChaseState(Enemy_StateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        base.Enter();
        // Optionally play alert animation/sound
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        Vector3 playerPos = GameManager.playerData[0].playerTransform.position;
        stateMachine.meshAgent.destination = playerPos;

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
    }
}