using System;
using System.Globalization;
using TrotroNation.Core.Interfaces;
using TrotroNation.Core.Managers;
using UnityEngine;

namespace TrotroNation.Economy
{
    /// <summary>
    /// Single source of truth for the player's money.
    ///
    /// Model:
    ///   - Wallet: cash on hand. Job payouts are credited here; repairs and fines are debited here first.
    ///   - Bank: savings. Funds move between wallet and bank via <see cref="DepositToBank"/> and <see cref="WithdrawFromBank"/>.
    ///   - Deductions (repairs, fines) draw from the wallet first, then the bank. They are all-or-nothing:
    ///     if the total available funds are insufficient, nothing is taken and <c>false</c> is returned.
    ///
    /// Every change raises <see cref="OnBalanceChanged"/>. Completed jobs also raise <see cref="OnJobCompleted"/>.
    /// The manager implements <see cref="ISaveable"/> so it persists through <see cref="SaveManager"/>.
    /// </summary>
    public sealed class EconomyManager : MonoSingleton<EconomyManager>, ISaveable
    {
        /// <summary>Save key under which economy state is stored.</summary>
        public const string EconomySaveKey = "economy";

        /// <summary>Current schema version of the economy save payload.</summary>
        public const int EconomySaveVersion = 1;

        [Tooltip("Cash the player starts with, in pesewas (1 GHS = 100).")]
        [Min(0)]
        [SerializeField] private int _startingWalletPesewas = 20000;

        [Tooltip("Bank balance the player starts with, in pesewas.")]
        [Min(0)]
        [SerializeField] private int _startingBankPesewas = 0;

        private decimal _walletBalance;
        private decimal _bankBalance;

        /// <summary>Cash on hand, in cedis. Never negative.</summary>
        public decimal WalletBalance => _walletBalance;

        /// <summary>Bank balance, in cedis. Never negative.</summary>
        public decimal BankBalance => _bankBalance;

        /// <summary>Wallet plus bank, in cedis. Used for affordability checks.</summary>
        public decimal TotalFunds => _walletBalance + _bankBalance;

        /// <summary>Raised after every change to the wallet or bank account.</summary>
        public event Action<BalanceChange> OnBalanceChanged;

        /// <summary>Raised after a delivery job has been settled.</summary>
        public event Action<JobResult> OnJobCompleted;

        /// <inheritdoc/>
        public string SaveKey => EconomySaveKey;

        /// <inheritdoc/>
        public int SaveVersion => EconomySaveVersion;

        protected override void Awake()
        {
            base.Awake();
            if (Instance != this) return;

            _walletBalance = Money.FromPesewas(_startingWalletPesewas);
            _bankBalance = Money.FromPesewas(_startingBankPesewas);
        }

        private void Start()
        {
            if (HasInstance && Instance == this && SaveManager.HasInstance)
            {
                SaveManager.Instance.Register(this);
            }
        }

        protected override void OnDestroy()
        {
            if (SaveManager.HasInstance)
            {
                SaveManager.Instance.Unregister(this);
            }
            base.OnDestroy();
        }

        /// <summary>
        /// Settles a delivery job: calculates the payout, credits the wallet and raises events.
        /// </summary>
        /// <param name="job">The completed job.</param>
        /// <param name="damagePercent">Damage sustained, in percent (clamped to [0, 100]).</param>
        /// <returns>The breakdown of gross, penalty and net payout.</returns>
        public JobResult CompleteJob(JobConfig job, float damagePercent)
        {
            if (job == null) throw new ArgumentNullException(nameof(job));

            JobResult result = JobPayoutCalculator.Calculate(job, damagePercent);
            Credit(BalanceAccount.Wallet, result.NetPayout, TransactionReason.JobPayout);
            OnJobCompleted?.Invoke(result);
            return result;
        }

        /// <summary>Attempts to deduct a repair bill from the player's funds.</summary>
        /// <returns>True if the full amount was taken; false if funds were insufficient (nothing taken).</returns>
        public bool TryDeductRepairCost(decimal amount)
        {
            return TryDebit(amount, TransactionReason.RepairDeduction);
        }

        /// <summary>Attempts to pay a police fine from the player's funds.</summary>
        /// <returns>True if the full amount was paid; false if funds were insufficient (nothing taken).</returns>
        public bool TryPayPoliceFine(decimal amount)
        {
            return TryDebit(amount, TransactionReason.PoliceFine);
        }

        /// <summary>Moves cash from the wallet into the bank.</summary>
        /// <returns>True on success; false if the wallet cannot cover the amount.</returns>
        public bool DepositToBank(decimal amount)
        {
            if (!TryNormalize(amount, out decimal value)) return false;
            if (value == 0m) return true;
            if (_walletBalance < value) return false;

            _walletBalance -= value;
            _bankBalance += value;
            RaiseChange(BalanceAccount.Wallet, -value, _walletBalance, TransactionReason.BankTransfer);
            RaiseChange(BalanceAccount.Bank, value, _bankBalance, TransactionReason.BankTransfer);
            return true;
        }

        /// <summary>Moves cash from the bank into the wallet.</summary>
        /// <returns>True on success; false if the bank cannot cover the amount.</returns>
        public bool WithdrawFromBank(decimal amount)
        {
            if (!TryNormalize(amount, out decimal value)) return false;
            if (value == 0m) return true;
            if (_bankBalance < value) return false;

            _bankBalance -= value;
            _walletBalance += value;
            RaiseChange(BalanceAccount.Bank, -value, _bankBalance, TransactionReason.BankTransfer);
            RaiseChange(BalanceAccount.Wallet, value, _walletBalance, TransactionReason.BankTransfer);
            return true;
        }

        /// <summary>Returns true if the player can cover the given amount from wallet plus bank.</summary>
        public bool CanAfford(decimal amount)
        {
            return TryNormalize(amount, out decimal value) && TotalFunds >= value;
        }

        /// <inheritdoc/>
        public string CaptureState()
        {
            var data = new EconomySaveData
            {
                walletBalance = _walletBalance.ToString(CultureInfo.InvariantCulture),
                bankBalance = _bankBalance.ToString(CultureInfo.InvariantCulture)
            };
            return JsonUtility.ToJson(data);
        }

        /// <inheritdoc/>
        public bool RestoreState(string json, int savedVersion)
        {
            if (string.IsNullOrEmpty(json) || savedVersion > EconomySaveVersion) return false;

            EconomySaveData data;
            try
            {
                data = JsonUtility.FromJson<EconomySaveData>(json);
            }
            catch (ArgumentException)
            {
                return false;
            }

            if (data == null) return false;
            if (!decimal.TryParse(data.walletBalance, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal wallet)
                || !decimal.TryParse(data.bankBalance, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal bank)
                || wallet < 0m || bank < 0m)
            {
                return false;
            }

            _walletBalance = Money.Round(wallet);
            _bankBalance = Money.Round(bank);
            return true;
        }

        private bool TryDebit(decimal amount, TransactionReason reason)
        {
            if (!TryNormalize(amount, out decimal value)) return false;
            if (value == 0m) return true;
            if (TotalFunds < value) return false;

            decimal fromWallet = Math.Min(_walletBalance, value);
            decimal fromBank = value - fromWallet;

            if (fromWallet > 0m)
            {
                _walletBalance -= fromWallet;
                RaiseChange(BalanceAccount.Wallet, -fromWallet, _walletBalance, reason);
            }

            if (fromBank > 0m)
            {
                _bankBalance -= fromBank;
                RaiseChange(BalanceAccount.Bank, -fromBank, _bankBalance, reason);
            }

            return true;
        }

        private void Credit(BalanceAccount account, decimal amount, TransactionReason reason)
        {
            if (!TryNormalize(amount, out decimal value) || value == 0m) return;

            if (account == BalanceAccount.Wallet)
            {
                _walletBalance += value;
                RaiseChange(account, value, _walletBalance, reason);
            }
            else
            {
                _bankBalance += value;
                RaiseChange(account, value, _bankBalance, reason);
            }
        }

        private void RaiseChange(BalanceAccount account, decimal delta, decimal newBalance, TransactionReason reason)
        {
            OnBalanceChanged?.Invoke(new BalanceChange(account, delta, newBalance, reason));
        }

        /// <summary>Validates and rounds an amount. Rejects negatives; amounts are rounded to pesewas.</summary>
        private static bool TryNormalize(decimal amount, out decimal value)
        {
            value = Money.Round(amount);
            return amount >= 0m;
        }

        [Serializable]
        public sealed class EconomySaveData
        {
            public string walletBalance;
            public string bankBalance;
        }
    }
}
