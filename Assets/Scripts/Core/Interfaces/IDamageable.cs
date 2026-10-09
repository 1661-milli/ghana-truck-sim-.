namespace TrotroNation.Core.Interfaces
{
    /// <summary>
    /// Contract for anything that can take damage: vehicle chassis, engine, or cargo.
    /// </summary>
    public interface IDamageable
    {
        /// <summary>Remaining integrity, in the range [0, MaxIntegrity].</summary>
        float CurrentIntegrity { get; }

        /// <summary>Maximum integrity.</summary>
        float MaxIntegrity { get; }

        /// <summary>True once integrity has reached zero.</summary>
        bool IsDestroyed { get; }

        /// <summary>Applies damage and reports the outcome.</summary>
        DamageResult ApplyDamage(DamageInfo damage);

        /// <summary>Restores integrity, clamped to MaxIntegrity.</summary>
        /// <returns>The amount actually restored.</returns>
        float Repair(float amount);
    }

    /// <summary>Categories of damage source.</summary>
    public enum DamageType
    {
        Impact = 0,
        Collision = 1,
        Rollover = 2,
        Fire = 3,
        Water = 4,
        Vibration = 5
    }

    /// <summary>Describes a single damage event.</summary>
    public readonly struct DamageInfo
    {
        public readonly float Amount;
        public readonly DamageType Type;

        public DamageInfo(float amount, DamageType type)
        {
            Amount = amount < 0f ? 0f : amount;
            Type = type;
        }
    }

    /// <summary>Outcome of an ApplyDamage call.</summary>
    public readonly struct DamageResult
    {
        public readonly float AppliedDamage;
        public readonly float RemainingIntegrity;
        public readonly bool WasDestroyed;

        public DamageResult(float appliedDamage, float remainingIntegrity, bool wasDestroyed)
        {
            AppliedDamage = appliedDamage;
            RemainingIntegrity = remainingIntegrity;
            WasDestroyed = wasDestroyed;
        }
    }
}
