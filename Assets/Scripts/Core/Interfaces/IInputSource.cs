namespace TrotroNation.Core.Interfaces
{
    /// <summary>
    /// Abstraction over a physical input backend (Unity Input System, touch, gamepad, tests).
    /// Lets InputManager stay independent of any particular device API.
    /// </summary>
    public interface IInputSource
    {
        /// <summary>Samples the device state for the current frame.</summary>
        RawInputState Poll();
    }

    /// <summary>Device-agnostic input snapshot for one frame.</summary>
    public readonly struct RawInputState
    {
        /// <summary>Steering in [-1, 1], left negative.</summary>
        public readonly float Steering;

        /// <summary>Throttle in [0, 1].</summary>
        public readonly float Throttle;

        /// <summary>Brake in [0, 1].</summary>
        public readonly float Brake;

        /// <summary>True on the frame the camera toggle was pressed.</summary>
        public readonly bool CameraTogglePressed;

        /// <summary>True on the frame the horn was pressed.</summary>
        public readonly bool HornPressed;

        /// <summary>True on the frame the pause/menu button was pressed.</summary>
        public readonly bool PausePressed;

        public RawInputState(float steering, float throttle, float brake,
                             bool cameraTogglePressed, bool hornPressed, bool pausePressed)
        {
            Steering = steering;
            Throttle = throttle;
            Brake = brake;
            CameraTogglePressed = cameraTogglePressed;
            HornPressed = hornPressed;
            PausePressed = pausePressed;
        }

        /// <summary>An input snapshot with no input pressed.</summary>
        public static RawInputState Idle => new RawInputState(0f, 0f, 0f, false, false, false);
    }
}
