using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy_StateMachine : MonoBehaviour
{
    public NavMeshAgent meshAgent;
    public bool playerVisible;
    [SerializeField] LayerMask layerMask;
    public Enemy_State currentState { set; private get; }
    PatrolState patrolState;
    ChaseState chaseState;

    private enum States
    {
        Patrol, Chase
    }
    [SerializeField] States state;
    public Transform currentDestination;

    // Start is called before the first frame update
    void Start()
    {
        InitializeStateMachine(patrolState);
    }

    public void ChangeState(Enemy_State newState)
    {
        currentState.Exit();
        currentState = newState;
        currentState.Enter();
    }

    public void InitializeStateMachine(Enemy_State initialState)
    {
        currentState = initialState;
        currentState.Enter();
    }

    // Update is called once per frame
    void Update()
    {
        if(GameData.isGameStarted)
        {
            DetectPlayer();
            currentState.LogicUpdate();
        }
    }

    void DetectPlayer()
    {
        if (Physics.Linecast(transform.position, GameManager.playerData[0].playerTransform.position, layerMask))
        {
            playerVisible = true;
            state = States.Chase;
        }
        else
        {
            state = States.Patrol;
        }
    }
}

public class Enemy_State
{
    //public Enemy_State(Enemy_StateMachine stateMachine, string animationName, bool isExitingState, bool isAnimationFinished);
    protected Enemy_StateMachine stateMachine;
    //protected Animator animationController;
    protected string animationName;

    protected bool isExitingState;
    protected bool isAnimationFinished;

    /* Put the constructor from above right here */

    public virtual void Enter()
    {
        isAnimationFinished = false;
        isExitingState = false;
        //animationController.SetBool(animationName, true);
    }
    public virtual void Exit()
    {
        isExitingState = true;
        if (!isAnimationFinished) isAnimationFinished = true;
        //animationController.SetBool(animationName, false);
    }
    public virtual void LogicUpdate()
    {
        TransitionChecks();
    }
    public virtual void PhysicsUpdate()
    {
    }
    public virtual void TransitionChecks()
    {
    }
    public virtual void AnimationTrigger()
    {
        isAnimationFinished = true;
    }
}

public class PatrolState : Enemy_State
{
    protected float patrolTime;
    public override void LogicUpdate()
    {
        base.LogicUpdate();
        Vector3 targetPos = GameManager.playerData[0].playerTransform.position;
        if (Vector3.Distance(stateMachine.transform.position, targetPos) > 1.0f)
        {
            stateMachine.meshAgent.destination = targetPos;
        }
    }


}

public class ChaseState : Enemy_State
{
    public override void LogicUpdate()
    {
        base.LogicUpdate();
        Vector3 targetPos = GameManager.playerData[0].playerTransform.position;
        if (Vector3.Distance(stateMachine.currentDestination.position, GameManager.playerData[0].playerTransform.position) > 1.0f)
        {
            stateMachine.meshAgent.destination = targetPos;
        }

        if(!stateMachine.playerVisible)
        {

        }
    }
}