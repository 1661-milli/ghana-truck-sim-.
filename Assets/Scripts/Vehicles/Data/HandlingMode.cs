namespace TrotroNation.Vehicles
{
    /// <summary>Selects the vehicle physics tuning profile.</summary>
    public enum HandlingMode
    {
        /// <summary>Forgiving, responsive handling for casual play.</summary>
        Arcade = 0,

        /// <summary>Weight transfer, tyre slip and braking distance modelled more faithfully.</summary>
        Realistic = 1
    }
}
