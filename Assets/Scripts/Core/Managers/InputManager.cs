using System;
using TrotroNation.Core.Interfaces;
using UnityEngine;

namespace TrotroNation.Core.Managers
{
    /// <summary>
    /// Centralised input hub. Polls an <see cref="IInputSource"/> once per frame,
    /// publishes the normalized <see cref="VehicleControls"/>, and raises discrete events
    /// (camera toggle, horn, pause). Concrete device handling lives in the input source,
    /// so this class has no direct dependency on any input API.
    /// </summary>
    public sealed class InputManager : MonoSingleton<InputManager>
    {
        [SerializeField] private float _steeringDeadZone = 0.05f;

        private IInputSource _source;

        /// <summary>Controls sampled this frame (neutral when no source is set).</summary>
        public VehicleControls CurrentControls { get; private set; } = VehicleControls.Neutral;

        /// <summary>Raw snapshot from the source for this frame.</summary>
        public RawInputState CurrentRawState { get; private set; } = RawInputState.Idle;

        public event Action CameraToggleRequested;
        public event Action HornPressed;
        public event Action PauseRequested;

        /// <summary>
        /// Replaces the active input backend. Pass null to disable input.
        /// Useful for tests, remapping, or switching between touch and controller.
        /// </summary>
        public void SetInputSource(IInputSource source)
        {
            _source = source;
        }

        protected override void Awake()
        {
            base.Awake();
            if (Instance != this) return;

            if (_source == null)
            {
                _source = CreateDefaultSource();
            }
        }

        private void Update()
        {
            if (_source == null) return;

            CurrentRawState = _source.Poll();
            CurrentControls = BuildControls(CurrentRawState);

            if (CurrentRawState.CameraTogglePressed) CameraToggleRequested?.Invoke();
            if (CurrentRawState.HornPressed) HornPressed?.Invoke();
            if (CurrentRawState.PausePressed) PauseRequested?.Invoke();
        }

        private VehicleControls BuildControls(RawInputState raw)
        {
            float steering = Mathf.Abs(raw.Steering) < _steeringDeadZone ? 0f : raw.Steering;
            return new VehicleControls(raw.Throttle, steering, raw.Brake, raw.HornPressed);
        }

        private static IInputSource CreateDefaultSource()
        {
#if ENABLE_INPUT_SYSTEM
            return new InputSystemInputSource();
#else
            return null;
#endif
        }
    }
}
