using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStateMachine : MonoBehaviour
{
    private Animator animator;

    public enum EnemyState
    {
        Idle,
        Moving
    }

    private Dictionary<EnemyState, Action> stateActions;

    public EnemyState CurrentState{get; private set;}

    private void Awake()
    {
        animator = GetComponent<Animator>();

        stateActions =
            new Dictionary<EnemyState, Action>
            {
                { EnemyState.Idle, HandleIdle },
                { EnemyState.Moving, HandleMoving }
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

    public void ChangeState(EnemyState newState)
    {
        if (CurrentState == newState)
            return;

        CurrentState = newState;

        //Debug.Log(CurrentState);
    }

    private void AnimationStateSetter(
        EnemyState currentState)
    {
        foreach ( EnemyState state in Enum.GetValues(typeof(EnemyState)))
        {
            animator.SetBool(
                state.ToString(),
                state == currentState
            );
        }
    }

    private void HandleIdle()
    {
        AnimationStateSetter(EnemyState.Idle);
    }

    private void HandleMoving()
    {
        AnimationStateSetter(EnemyState.Moving);
    }
}