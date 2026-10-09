using System;

namespace TrotroNation.Economy
{
    /// <summary>
    /// Pure payout rules for delivery jobs. Has no Unity or state dependencies, so it is trivially testable.
    /// </summary>
    public static class JobPayoutCalculator
    {
        /// <summary>
        /// Calculates the result of a finished job.
        /// Penalties apply only to fragile jobs: penalty fraction = PenaltyPerDamagePercent/100 * damagePercent,
        /// clamped to [0, 1] so a payout can never go negative.
        /// </summary>
        /// <param name="job">The job that was completed.</param>
        /// <param name="damagePercent">Damage sustained, in percent. Clamped to [0, 100]; NaN treated as 0.</param>
        public static JobResult Calculate(JobConfig job, float damagePercent)
        {
            if (job == null) throw new ArgumentNullException(nameof(job));

            float safeDamage = float.IsNaN(damagePercent) ? 0f : Clamp(damagePercent, 0f, 100f);

            decimal gross = Money.Round(job.BasePayout);
            decimal penalty = 0m;

            if (job.IsFragile && safeDamage > 0f)
            {
                decimal fraction = job.PenaltyPerDamagePercent / 100m * (decimal)safeDamage;
                fraction = Clamp(fraction, 0m, 1m);
                penalty = Money.Round(gross * fraction);
            }

            decimal net = gross - penalty;
            return new JobResult(job.JobID, safeDamage, gross, penalty, net);
        }

        private static float Clamp(float value, float min, float max)
        {
            if (value < min) return min;
            if (value > max) return max;
            return value;
        }

        private static decimal Clamp(decimal value, decimal min, decimal max)
        {
            if (value < min) return min;
            if (value > max) return max;
            return value;
        }
    }
}
