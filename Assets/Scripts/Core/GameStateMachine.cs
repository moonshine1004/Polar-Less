using System;
using UnityEngine; // 디버그용

public enum GameState
{
    Ready,
    Playing,
    GameOver
}

public interface IGameStateSubscriber // 구독용
{
    event Action<GameState> StateEntered;
}

public interface IGameStatePublisher // 발행용
{
    void Change(GameState nextState);
}

public class GameStateMachine : IGameStateSubscriber, IGameStatePublisher
{
    private GameState _currentState = GameState.Ready;

    public event Action<GameState> StateEntered;

    public void Change(GameState nextState)
    {
        // 같은 상태 전환 무시
        if (!CanTransition(_currentState, nextState))
            return;

        Debug.Log($"상태 전환: {_currentState} -> {nextState}");
        _currentState = nextState;
        StateEntered?.Invoke(nextState);
    }

    private bool CanTransition(GameState from, GameState to) => from != to;
}
