using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateMachine : MonoBehaviour
{

    private Animator animator;

    public enum PlayerState
    {
        Idle,
        Walking,
        Sprinting,
        Jump,
        Fall,
        
    }

    private Dictionary<PlayerState, Action> stateActions;
    public PlayerState CurrentState { get; private set; }

    public void ChangeState(PlayerState newState)
    {
        if (CurrentState == newState)
            return;

        CurrentState = newState;

        Debug.Log(CurrentState);
    }

    private void Awake()
    {
       

        animator = GetComponent<Animator>();
        
        stateActions = new Dictionary<PlayerState, Action>
        {
            { PlayerState.Idle, HandleIdle },
            { PlayerState.Walking, HandleWalking },
            { PlayerState.Sprinting, HandleSprint },
            { PlayerState.Jump, HandleJump },
            { PlayerState.Fall, HandleFall }
            
        };
    }
    private void Start()
    {
        ChangeState(PlayerState.Idle);
    }

    private void Update()
    {

        stateActions[CurrentState].Invoke();
    }

    private void AnimationStateSetter(PlayerState currentState)
    {
        foreach (PlayerState state in Enum.GetValues(typeof(PlayerState)))
        {
            if(state == currentState)
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
        AnimationStateSetter(PlayerState.Idle);
    }

    private void HandleWalking()
    {
        AnimationStateSetter(PlayerState.Walking);
        
    }

    private void HandleSprint()
    {
       AnimationStateSetter(PlayerState.Sprinting);
    }

    private void HandleJump()
    {
       AnimationStateSetter(PlayerState.Jump);
    }

    private void HandleFall()
    {
        AnimationStateSetter(PlayerState.Fall);
    }
    
}

