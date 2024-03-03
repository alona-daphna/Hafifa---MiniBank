namespace MiniBank.Models
{
    internal abstract class Account
    {
        internal string ID { get; set; }
        internal decimal Balance { get; set; }
        internal string OwnerID { get; set; }

        public void Deposit(decimal amount) 
        {
            EnsureAmountPositive(amount);
            Balance += amount;
        }
        public virtual void Withdraw(decimal amount) 
        { 
            EnsureAmountPositive(amount);
            Balance -= amount;
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