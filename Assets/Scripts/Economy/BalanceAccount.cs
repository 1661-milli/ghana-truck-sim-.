namespace TrotroNation.Economy
{
    /// <summary>The player account a balance change applied to.</summary>
    public enum BalanceAccount
    {
        /// <summary>Cash the player carries. Used for day-to-day spending.</summary>
        Wallet = 0,

        /// <summary>Bank balance. Safe storage, not directly spent.</summary>
        Bank = 1
    }
}
