namespace TrotroNation.Economy
{
    /// <summary>
    /// Payload for <see cref="EconomyManager.OnBalanceChanged"/>. Describes one account movement.
    /// </summary>
    public readonly struct BalanceChange
    {
        /// <summary>The account that changed.</summary>
        public readonly BalanceAccount Account;

        /// <summary>Signed amount in cedis. Positive = credit, negative = debit.</summary>
        public readonly decimal Delta;

        /// <summary>The account balance after the change, in cedis.</summary>
        public readonly decimal NewBalance;

        /// <summary>The business reason for the change.</summary>
        public readonly TransactionReason Reason;

        public BalanceChange(BalanceAccount account, decimal delta, decimal newBalance, TransactionReason reason)
        {
            Account = account;
            Delta = delta;
            NewBalance = newBalance;
            Reason = reason;
        }
    }
}
