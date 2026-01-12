public interface IExitableState
{
    void Exit();
}

public interface IState : IExitableState
{
    void Enter();
    void Subscribe();
    void Unsubscribe();
}

public interface IPayloadState<TPayload> : IState
{
    void Enter(TPayload payload);
}