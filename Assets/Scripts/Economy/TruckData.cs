using UnityEngine;

namespace TrotroNation.Economy
{
    /// <summary>
    /// Designer-authored attributes of a purchasable truck model.
    /// Immutable at runtime: owned instances should copy these values into their own state.
    /// </summary>
    [CreateAssetMenu(fileName = "NewTruckData", menuName = "TrotroNation/Economy/Truck Data")]
    public sealed class TruckData : ScriptableObject
    {
        [Tooltip("Unique identifier used by save data and the garage. Must not be empty.")]
        [SerializeField] private string _vehicleID = "truck_000";

        [Tooltip("Display name shown in the showroom.")]
        [SerializeField] private string _name = "Tata Mini";

        [Tooltip("Purchase price in Ghana cedis (GHS).")]
        [Min(0)]
        [SerializeField] private int _price = 50000;

        [Tooltip("Engine power in horsepower.")]
        [Min(0)]
        [SerializeField] private int _horsepower = 150;

        [Tooltip("Fuel tank capacity in litres.")]
        [Min(0f)]
        [SerializeField] private float _fuelCapacity = 300f;

        [SerializeField] private HandlingType _handlingType = HandlingType.Balanced;

        public string VehicleID => _vehicleID;
        public string Name => _name;
        public int Price => _price;
        public int Horsepower => _horsepower;
        public float FuelCapacity => _fuelCapacity;
        public HandlingType HandlingType => _handlingType;

        private void OnValidate()
        {
            if (string.IsNullOrWhiteSpace(_vehicleID))
            {
                Debug.LogWarning($"[TruckData] '{name}' has an empty VehicleID.", this);
            }
        }
    }
}
