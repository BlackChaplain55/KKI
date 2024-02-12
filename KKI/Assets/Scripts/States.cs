using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine
{
    public event StateHandler OnStateChanged;
    public delegate void StateHandler(IState state, IState oldState);

    private IState _currentState;
    private IState _oldState;

    public IState CurrentState => _currentState;
    public IState OldState => _oldState;

    public void ChangeState(IState state)
    {
        _oldState = _currentState;
        _currentState = state;
        _oldState.Exit();
        state.Enter();
        OnStateChanged?.Invoke(CurrentState, OldState);
    }

    public void Update()
    {
        _currentState.Update();
    }

    public void Initialize(IState startState)
    {
        _currentState = startState;
        startState.Enter();
    }
}



public interface IState
{
    void Enter();

    void Exit();

    void Update();

}
