namespace TrotroNation.Economy
{
    /// <summary>
    /// Outcome of a completed delivery job. Payload for <see cref="EconomyManager.OnJobCompleted"/>.
    /// </summary>
    public readonly struct JobResult
    {
        /// <summary>The <see cref="JobConfig.JobID"/> of the completed job.</summary>
        public readonly string JobID;

        /// <summary>Damage sustained during the run, as a percentage in [0, 100].</summary>
        public readonly float DamagePercent;

        /// <summary>Payout before penalties, in cedis.</summary>
        public readonly decimal GrossPayout;

        /// <summary>Amount deducted for damage, in cedis. Never negative.</summary>
        public readonly decimal DamagePenalty;

        /// <summary>Amount credited to the wallet (gross minus penalty), in cedis.</summary>
        public readonly decimal NetPayout;

        public JobResult(string jobID, float damagePercent, decimal grossPayout, decimal damagePenalty, decimal netPayout)
        {
            JobID = jobID ?? string.Empty;
            DamagePercent = damagePercent;
            GrossPayout = grossPayout;
            DamagePenalty = damagePenalty;
            NetPayout = netPayout;
        }
    }
}
