using System;
using System.Collections.Generic;

public class StateMachine
{
    private readonly Dictionary<Type, IExitableState> _states;
    private IExitableState _activeState;

    public StateMachine(GameBootstrapper gameBootstrapper)
    {
        _states = new Dictionary<Type, IExitableState>()
        {
            [typeof(GameLoopState)] = new GameLoopState(gameBootstrapper, this),
            [typeof(InitialState)] = new InitialState(gameBootstrapper, this),
            [typeof(DiagramViewState)] = new DiagramViewState(gameBootstrapper, this),
            [typeof(MainMenuState)] = new MainMenuState(gameBootstrapper, this),
        };
    }

    public void Enter<TState>() where TState : class, IState
    {
        IState state = ChangeState<TState>();
        state.Enter();
    }

    private TState ChangeState<TState>() where TState : class, IExitableState
    {
        _activeState?.Exit();
        TState state = GetState<TState>();
        _activeState = state;
        return state;
    }

    public void Enter<TState, TPayload>(TPayload payload) where TState : class, IPayloadState<TPayload>
    {
        TState state = ChangeState<TState>();
        state.Enter(payload);
    }

    private TState GetState<TState>() where TState : class, IExitableState => _states[typeof(TState)] as TState;
}