using TrotroNation.Vehicles;
using UnityEngine;

namespace TrotroNation.Economy
{
    /// <summary>
    /// Designer-authored delivery job: route, pay, time limit, and trailer requirement.
    /// Money is authored in pesewas and exposed as decimal cedis (see <see cref="Money"/>).
    /// </summary>
    [CreateAssetMenu(fileName = "NewJobConfig", menuName = "TrotroNation/Economy/Job Config")]
    public sealed class JobConfig : ScriptableObject
    {
        [Tooltip("Unique identifier used by save data and job boards. Must not be empty.")]
        [SerializeField] private string _jobID = "job_accra_kumasi_001";

        [Tooltip("Title shown on the job board.")]
        [SerializeField] private string _title = "Accra to Kumasi Cocoa Run";

        [Tooltip("Pickup location, e.g. 'Tema Port'.")]
        [SerializeField] private string _departureLocation = "Tema Port";

        [Tooltip("Drop-off location, e.g. 'Kumasi Central Market'.")]
        [SerializeField] private string _destinationLocation = "Kumasi Central Market";

        [Tooltip("Gross payout before damage penalties, in pesewas (1 GHS = 100).")]
        [Min(0)]
        [SerializeField] private int _basePayoutPesewas = 450000;

        [Tooltip("Hours before the job is withdrawn from the board. 0 = never expires.")]
        [Min(0f)]
        [SerializeField] private float _expirationTimeHours = 24f;

        [Tooltip("Trailer specialisation the driver must bring.")]
        [SerializeField] private TrailerCargoType _requiredTrailerType = TrailerCargoType.Cocoa;

        [Tooltip("Fragile cargo is subject to the damage penalty below.")]
        [SerializeField] private bool _isFragile = false;

        [Tooltip("Percent of the base payout deducted per 1% of cargo/vehicle damage, in hundredths of a percent " +
                 "(150 = 1.50%). Only applies when IsFragile is true.")]
        [Min(0)]
        [SerializeField] private int _penaltyPerDamagePercentHundredths = 150;

        /// <summary>Unique identifier. Never null.</summary>
        public string JobID => _jobID ?? string.Empty;

        /// <summary>Title shown on the job board.</summary>
        public string Title => _title ?? string.Empty;

        /// <summary>Pickup location name.</summary>
        public string DepartureLocation => _departureLocation ?? string.Empty;

        /// <summary>Drop-off location name.</summary>
        public string DestinationLocation => _destinationLocation ?? string.Empty;

        /// <summary>Gross payout in cedis, before any damage penalty.</summary>
        public decimal BasePayout => Money.FromPesewas(_basePayoutPesewas);

        /// <summary>Hours until expiry. Zero means the job never expires. Never negative.</summary>
        public float ExpirationTimeHours => Mathf.Max(0f, _expirationTimeHours);

        /// <summary>Trailer specialisation required to accept the job.</summary>
        public TrailerCargoType RequiredTrailerType => _requiredTrailerType;

        /// <summary>True if cargo damage reduces the payout.</summary>
        public bool IsFragile => _isFragile;

        /// <summary>
        /// Percent of the base payout lost per 1% of damage, as a decimal percentage.
        /// For example, a value of 1.50 means 15% of the payout is lost at 10% damage.
        /// </summary>
        public decimal PenaltyPerDamagePercent => _penaltyPerDamagePercentHundredths / 100m;

        private void OnValidate()
        {
            if (string.IsNullOrWhiteSpace(_jobID))
            {
                Debug.LogWarning($"[JobConfig] '{name}' has an empty JobID.", this);
            }
        }
    }
}
