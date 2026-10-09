using UnityEngine;

namespace TrotroNation.Vehicles
{
    /// <summary>
    /// Designer-authored definition of a trailer: cargo specialisation, load limits,
    /// centre-of-mass offset and the licence needed to tow it.
    /// </summary>
    [CreateAssetMenu(fileName = "NewTrailerConfig", menuName = "TrotroNation/Vehicles/Trailer Config")]
    public sealed class TrailerConfig : ScriptableObject
    {
        [Tooltip("Unique identifier used by save data and job requirements. Must not be empty.")]
        [SerializeField] private string _trailerID = "trailer_container_001";

        [Tooltip("Cargo category this trailer is fitted out for.")]
        [SerializeField] private TrailerCargoType _cargoType = TrailerCargoType.Container;

        [Tooltip("Maximum payload in metric tonnes.")]
        [Min(0f)]
        [SerializeField] private float _maxWeightTons = 30f;

        [Tooltip("Local-space offset of the trailer's centre of mass from its pivot, in metres. " +
                 "Used by the physics controller for load balance.")]
        [SerializeField] private Vector3 _massCenterOffset = Vector3.zero;

        [Tooltip("Licence class required to tow this trailer, e.g. 'Class C'. Leave empty if none.")]
        [SerializeField] private string _licenseRequired = "Class C";

        /// <summary>Unique identifier. Never null.</summary>
        public string TrailerID => _trailerID ?? string.Empty;

        /// <summary>Cargo category this trailer carries.</summary>
        public TrailerCargoType CargoType => _cargoType;

        /// <summary>Maximum payload in tonnes. Never negative.</summary>
        public float MaxWeightTons => Mathf.Max(0f, _maxWeightTons);

        /// <summary>Centre-of-mass offset in local metres.</summary>
        public Vector3 MassCenterOffset => _massCenterOffset;

        /// <summary>Licence class required to tow this trailer. Never null.</summary>
        public string LicenseRequired => _licenseRequired ?? string.Empty;

        private void OnValidate()
        {
            if (string.IsNullOrWhiteSpace(_trailerID))
            {
                Debug.LogWarning($"[TrailerConfig] '{name}' has an empty TrailerID.", this);
            }
        }
    }
}
