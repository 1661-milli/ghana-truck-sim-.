using System;
using UnityEngine;

namespace TrotroNation.Core.Managers
{
    /// <summary>
    /// Owns the active <see cref="GameState"/> and broadcasts state changes.
    /// Gameplay systems subscribe to <see cref="StateChanged"/> instead of polling.
    /// </summary>
    public sealed class GameManager : MonoSingleton<GameManager>
    {
        [SerializeField] private GameState _startingState = GameState.MainMenu;

        private GameStateMachine _stateMachine;

        /// <summary>Raised after every successful state change: (previous, next).</summary>
        public event Action<GameState, GameState> StateChanged;

        public GameState CurrentState => _stateMachine != null ? _stateMachine.Current : _startingState;

        public GameState PreviousState => _stateMachine != null ? _stateMachine.Previous : _startingState;

        public bool IsPaused => CurrentState == GameState.Paused;

        protected override void Awake()
        {
            base.Awake();
            if (Instance != this) return;

            _stateMachine = new GameStateMachine(_startingState);
            _stateMachine.StateChanged += HandleStateChanged;
        }

        protected override void OnDestroy()
        {
            if (_stateMachine != null)
            {
                _stateMachine.StateChanged -= HandleStateChanged;
            }
            base.OnDestroy();
        }

        /// <summary>Requests a transition to <paramref name="target"/>. Illegal transitions are rejected.</summary>
        /// <returns>True if the state changed.</returns>
        public bool RequestState(GameState target)
        {
            if (_stateMachine == null)
            {
                Debug.LogWarning("[GameManager] RequestState called before initialisation.");
                return false;
            }

            if (_stateMachine.TryTransitionTo(target)) return true;

            Debug.LogWarning($"[GameManager] Rejected transition {CurrentState} -> {target}.");
            return false;
        }

        /// <summary>Pauses the game, remembering the current state for resume.</summary>
        public bool Pause() => RequestState(GameState.Paused);

        /// <summary>Resumes the state that was active before pausing or opening a dialog.</summary>
        public bool Resume() => _stateMachine != null && _stateMachine.TryResume();

        /// <summary>Toggles between paused and the previous state.</summary>
        public void TogglePause()
        {
            if (IsPaused) Resume();
            else Pause();
        }

        private void HandleStateChanged(GameState from, GameState to)
        {
            Time.timeScale = to == GameState.Paused ? 0f : 1f;
            StateChanged?.Invoke(from, to);
        }
    }
}
