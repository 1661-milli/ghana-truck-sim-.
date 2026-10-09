using System;

namespace TrotroNation.Economy
{
    /// <summary>
    /// Money helpers. All currency is GHS (Ghana cedis) with two decimal places (pesewas).
    /// Unity cannot serialize <see cref="decimal"/>, so designer-authored amounts are stored
    /// as integer pesewas and converted here.
    /// </summary>
    public static class Money
    {
        /// <summary>Number of pesewas in one cedi.</summary>
        public const int PesewasPerCedi = 100;

        /// <summary>Converts an integer pesewa amount to cedis. Negative inputs are clamped to zero.</summary>
        public static decimal FromPesewas(int pesewas)
        {
            return pesewas < 0 ? 0m : pesewas / (decimal)PesewasPerCedi;
        }

        /// <summary>Rounds a cedi amount to pesewa precision (half away from zero).</summary>
        public static decimal Round(decimal amount)
        {
            return Math.Round(amount, 2, MidpointRounding.AwayFromZero);
        }
    }
}
