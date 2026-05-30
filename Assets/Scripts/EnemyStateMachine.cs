using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStateMachine : MonoBehaviour
{
    private Animator animator;

    public enum EnemyState
    {
        Idle,
        Sprinting,
    }

    private Dictionary<EnemyState, Action> stateActions;
    public EnemyState CurrentState { get; private set; }

    public void ChangeState(EnemyState newState)
    {
        if (CurrentState == newState)
            return;

        CurrentState = newState;

        Debug.Log(CurrentState);
    }

    private void Awake()
    {


        animator = GetComponent<Animator>();

        stateActions = new Dictionary<EnemyState, Action>
        {
            {EnemyState.Idle, HandleIdle },
            {EnemyState.Sprinting, HandleSprint }


        };
    }
    private void Start()
    {
        ChangeState(EnemyState.Idle);
    }

    private void Update()
    {

        stateActions[CurrentState].Invoke();
    }

    private void AnimationStateSetter(EnemyState currentState)
    {
        foreach (EnemyState state in Enum.GetValues(typeof(EnemyState)))
        {
            if (state == currentState)
            {
                animator.SetBool(state.ToString(), true);
            }
            else
            {
                animator.SetBool(state.ToString(), false);
            }
        }
    }

    private void HandleIdle()
    {
        AnimationStateSetter(EnemyState.Idle);
    }

    private void HandleSprint()
    {
        AnimationStateSetter(EnemyState.Sprinting);
    }

 

  
}


