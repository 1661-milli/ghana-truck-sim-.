using System;

namespace TrotroNation.Core.Managers
{
    /// <summary>
    /// Pure C# state machine holding the legal GameState transitions.
    /// Contains no Unity dependencies so it can be unit tested in isolation.
    /// </summary>
    public sealed class GameStateMachine
    {
        public GameState Current { get; private set; }

        /// <summary>The state that was active before the current one (used to resume from pause/dialog).</summary>
        public GameState Previous { get; private set; }

        public event Action<GameState, GameState> StateChanged;

        public GameStateMachine(GameState initialState = GameState.MainMenu)
        {
            Current = initialState;
            Previous = initialState;
        }

        /// <summary>Returns true if moving from <see cref="Current"/> to <paramref name="target"/> is allowed.</summary>
        public bool CanTransitionTo(GameState target)
        {
            if (target == Current) return false;

            switch (Current)
            {
                case GameState.MainMenu:
                    return target == GameState.OnFoot;

                case GameState.OnFoot:
                    return target == GameState.Driving
                        || target == GameState.Paused
                        || target == GameState.Dialog
                        || target == GameState.MainMenu;

                case GameState.Driving:
                    return target == GameState.OnFoot
                        || target == GameState.Paused
                        || target == GameState.Dialog
                        || target == GameState.MainMenu;

                case GameState.Dialog:
                    return target == GameState.OnFoot
                        || target == GameState.Driving
                        || target == GameState.Paused;

                case GameState.Paused:
                    return target == GameState.OnFoot
                        || target == GameState.Driving
                        || target == GameState.Dialog
                        || target == GameState.MainMenu;

                default:
                    return false;
            }
        }

        /// <summary>Attempts to transition to <paramref name="target"/>.</summary>
        /// <returns>True if the transition happened.</returns>
        public bool TryTransitionTo(GameState target)
        {
            if (!CanTransitionTo(target)) return false;
            Apply(target);
            return true;
        }

        /// <summary>Returns to the state that was active before pausing or opening a dialog.</summary>
        public bool TryResume()
        {
            if (Current != GameState.Paused && Current != GameState.Dialog) return false;
            return TryTransitionTo(Previous);
        }

        private void Apply(GameState target)
        {
            GameState from = Current;
            Previous = Current;
            Current = target;
            StateChanged?.Invoke(from, target);
        }
    }
}
