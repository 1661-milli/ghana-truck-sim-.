namespace TrotroNation.Economy
{
    /// <summary>Why a balance changed. Used for UI, analytics and audit logs.</summary>
    public enum TransactionReason
    {
        JobPayout = 0,
        RepairDeduction = 1,
        PoliceFine = 2,
        BankTransfer = 3,
        Adjustment = 4
    }
}
