namespace TrotroNation.Core.Interfaces
{
    /// <summary>
    /// Snapshot of driver inputs for one frame. Values are normalized:
    /// Throttle and Brake in [0, 1], Steering in [-1, 1] (left negative).
    /// Out-of-range values are clamped on construction.
    /// </summary>
    public readonly struct VehicleControls
    {
        public readonly float Throttle;
        public readonly float Steering;
        public readonly float Brake;
        public readonly bool HornPressed;

        public VehicleControls(float throttle, float steering, float brake, bool hornPressed = false)
        {
            Throttle = Clamp(throttle, 0f, 1f);
            Steering = Clamp(steering, -1f, 1f);
            Brake = Clamp(brake, 0f, 1f);
            HornPressed = hornPressed;
        }

        public static VehicleControls Neutral => new VehicleControls(0f, 0f, 0f);

        private static float Clamp(float value, float min, float max)
        {
            if (value < min) return min;
            if (value > max) return max;
            return value;
        }
    }
}
