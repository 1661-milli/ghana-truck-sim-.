namespace TrotroNation.Vehicles
{
    /// <summary>
    /// Cargo category a trailer is fitted out for. Named distinctly from
    /// <c>TrotroNation.Economy.CargoType</c> to avoid ambiguity where both namespaces are in scope.
    /// </summary>
    public enum TrailerCargoType
    {
        Cocoa = 0,
        Timber = 1,
        Container = 2,
        Fuel = 3,
        Plantains = 4
    }
}
