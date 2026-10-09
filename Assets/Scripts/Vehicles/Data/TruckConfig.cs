using TrotroNation.Economy;
using UnityEngine;

namespace TrotroNation.Vehicles
{
    /// <summary>
    /// Designer-authored definition of a truck model: identity, price, performance and audio.
    /// Read-only at runtime. Owned vehicles copy what they need into their own state.
    /// </summary>
    [CreateAssetMenu(fileName = "NewTruckConfig", menuName = "TrotroNation/Vehicles/Truck Config")]
    public sealed class TruckConfig : ScriptableObject
    {
        [Tooltip("Unique identifier used by save data and the garage. Must not be empty.")]
        [SerializeField] private string _vehicleID = "truck_light_001";

        [Tooltip("Name shown to the player in the showroom and garage.")]
        [SerializeField] private string _modelDisplayName = "Light Rigid";

        [Tooltip("Body and drivetrain class.")]
        [SerializeField] private TruckType _truckType = TruckType.LightRigid;

        [Tooltip("Purchase price in pesewas (1 GHS = 100). 5000000 = GHS 50,000.")]
        [Min(0)]
        [SerializeField] private int _basePricePesewas = 5000000;

        [Tooltip("Engine power in horsepower.")]
        [Min(0)]
        [SerializeField] private int _horsepower = 180;

        [Tooltip("Fuel tank capacity in litres.")]
        [Min(0f)]
        [SerializeField] private float _fuelCapacityLiters = 300f;

        [Tooltip("Physics tuning profile used by the vehicle controller.")]
        [SerializeField] private HandlingMode _handlingMode = HandlingMode.Arcade;

        [Tooltip("Key of the engine audio preset to load (matches the audio bank). Leave empty for silent.")]
        [SerializeField] private string _engineAudioPreset = "engine_default";

        /// <summary>Unique identifier. Never null; may be empty only if the asset is misconfigured.</summary>
        public string VehicleID => _vehicleID ?? string.Empty;

        /// <summary>Name shown to the player.</summary>
        public string ModelDisplayName => _modelDisplayName ?? string.Empty;

        /// <summary>Body and drivetrain class.</summary>
        public TruckType TruckType => _truckType;

        /// <summary>Purchase price in cedis, rounded to pesewa precision.</summary>
        public decimal BasePrice => Money.FromPesewas(_basePricePesewas);

        /// <summary>Engine power in horsepower. Never negative.</summary>
        public int Horsepower => Mathf.Max(0, _horsepower);

        /// <summary>Fuel tank capacity in litres. Never negative.</summary>
        public float FuelCapacityLiters => Mathf.Max(0f, _fuelCapacityLiters);

        /// <summary>Physics tuning profile.</summary>
        public HandlingMode HandlingMode => _handlingMode;

        /// <summary>Engine audio preset key. Never null.</summary>
        public string EngineAudioPreset => _engineAudioPreset ?? string.Empty;

        private void OnValidate()
        {
            if (string.IsNullOrWhiteSpace(_vehicleID))
            {
                Debug.LogWarning($"[TruckConfig] '{name}' has an empty VehicleID.", this);
            }
        }
    }
}
