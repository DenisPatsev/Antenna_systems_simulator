public class InitialState : IState
{
    private GameBootstrapper _gameBootstrapper;
    private StateMachine _stateMachine;

    public InitialState(GameBootstrapper gameBootstrapper, StateMachine stateMachine)
    {
        _gameBootstrapper = gameBootstrapper;
        _stateMachine = stateMachine;
    }
   
   
    public void Enter()
    {
        _gameBootstrapper.SceneLoader.LoadScene(Constants.MainMenuSceneName, OnLoaded);
    }

    private void OnLoaded()
    {
        _stateMachine.Enter<MainMenuState>();
    }

    public void Subscribe()
    {
    }

    public void Unsubscribe()
    {
    }
    public void Exit()
    {
    }
}