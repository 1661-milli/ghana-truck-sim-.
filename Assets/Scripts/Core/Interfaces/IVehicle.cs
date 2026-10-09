namespace TrotroNation.Core.Interfaces
{
    /// <summary>
    /// Core behaviour contract for any drivable vehicle (trucks, trotros, etc.).
    /// Implementations own their physics/simulation; consumers read state and send controls.
    /// </summary>
    public interface IVehicle : IInteractable
    {
        /// <summary>Current engine speed in revolutions per minute.</summary>
        float EngineRPM { get; }

        /// <summary>Current ground speed in kilometres per hour.</summary>
        float SpeedKmh { get; }

        /// <summary>Fuel level as a normalized value in the range [0, 1].</summary>
        float FuelNormalized { get; }

        /// <summary>True when the engine is running.</summary>
        bool IsEngineOn { get; }

        /// <summary>True when a player is currently seated at the controls.</summary>
        bool IsOccupied { get; }

        /// <summary>Starts the engine if fuel and integrity allow it.</summary>
        /// <returns>True if the engine is running after the call.</returns>
        bool StartEngine();

        /// <summary>Shuts the engine down.</summary>
        void StopEngine();

        /// <summary>Applies driver controls for the current frame.</summary>
        void SetControls(VehicleControls controls);

        /// <summary>Adds fuel, clamped to capacity.</summary>
        /// <param name="litres">Amount of fuel to add, in litres. Negative values are ignored.</param>
        /// <returns>The amount actually added, in litres.</returns>
        float Refuel(float litres);

        /// <summary>Sets whether a player currently occupies the driver's seat.</summary>
        void SetOccupied(bool occupied);
    }
}
