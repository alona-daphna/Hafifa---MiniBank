namespace MiniBank.Models
{
    internal abstract class Account
    {
        internal string ID { get; set; }
        internal decimal Balance { get; set; }
        internal string OwnerID { get; set; }
        private decimal MaxBalance { get; set; } = 100000000000000;

        public void Deposit(decimal amount) 
        {
            EnsureAmountPositive(amount);
            PreventOverflow(amount + Balance);
            Balance += amount;
        }
        public virtual void Withdraw(decimal amount) 
        { 
            EnsureAmountPositive(amount);
            PreventOverflow(amount + Balance);
            Balance -= amount;
        }

        private void PreventOverflow(decimal amount)
        {
            if (amount > MaxBalance) throw new ArgumentException("Amount exceeds maximal account balance.");
        }

        private void EnsureAmountPositive(decimal amount)
        {
            if (amount <= 0)
            {
                throw new ArgumentException("Amount must be positive.");
            }
        }

        internal void EnsureOwnership(string ownerID)
        {
            if (ownerID != OwnerID)
            {
                throw new UnauthorizedAccessException("You don't own this account, operation is not authorized.");
            }
        }
    }
}