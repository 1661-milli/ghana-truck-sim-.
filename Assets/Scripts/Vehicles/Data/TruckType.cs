namespace TrotroNation.Vehicles
{
    /// <summary>Body and drivetrain class of a truck. Determines licence and trailer compatibility.</summary>
    public enum TruckType
    {
        /// <summary>Small rigid truck for short urban and regional routes.</summary>
        LightRigid = 0,

        /// <summary>Tractor unit pulling a semi-trailer. Heaviest class.</summary>
        HeavyArticulated = 1,

        /// <summary>Rigid truck with a hydraulic tipping body for bulk materials.</summary>
        Tipper = 2
    }
}
