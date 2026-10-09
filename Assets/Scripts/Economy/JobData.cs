using UnityEngine;

namespace TrotroNation.Economy
{
    /// <summary>
    /// Designer-authored definition of a delivery job shown on job boards.
    /// Immutable at runtime: create per-run state elsewhere.
    /// </summary>
    [CreateAssetMenu(fileName = "NewJobData", menuName = "TrotroNation/Economy/Job Data")]
    public sealed class JobData : ScriptableObject
    {
        [Tooltip("Unique identifier used by save data and job boards. Must not be empty.")]
        [SerializeField] private string _jobId = "job_000";

        [Tooltip("Display name shown to the player.")]
        [SerializeField] private string _title = "New Delivery";

        [Tooltip("Pickup location, e.g. 'Tema Port'.")]
        [SerializeField] private string _origin = "Accra";

        [Tooltip("Drop-off location, e.g. 'Kumasi Central Market'.")]
        [SerializeField] private string _destination = "Kumasi";

        [SerializeField] private CargoType _cargoType = CargoType.GeneralGoods;

        [Tooltip("Total cargo weight in kilograms.")]
        [Min(0f)]
        [SerializeField] private float _weightKg = 1000f;

        [Tooltip("Base payout in Ghana cedis (GHS) before bonuses or penalties.")]
        [Min(0)]
        [SerializeField] private int _basePayout = 500;

        [SerializeField] private FragilityLevel _fragility = FragilityLevel.Standard;

        public string JobId => _jobId;
        public string Title => _title;
        public string Origin => _origin;
        public string Destination => _destination;
        public CargoType CargoType => _cargoType;
        public float Weight => _weightKg;
        public int BasePayout => _basePayout;
        public FragilityLevel Fragility => _fragility;

        private void OnValidate()
        {
            if (string.IsNullOrWhiteSpace(_jobId))
            {
                Debug.LogWarning($"[JobData] '{name}' has an empty JobId.", this);
            }
        }
    }
}
