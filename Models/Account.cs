namespace MiniBank.Models
{
    internal abstract class Account
    {
        public virtual string ID { get; set; }
        public virtual decimal Balance { get; set; }
        public virtual string OwnerID { get; set; }
        private decimal MaxBalance { get; set; } = 100000000000000;

        public virtual void Deposit(decimal amount) 
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
            if (amount > MaxBalance) throw new ArgumentException("Amount exceeds limit");
        }

        private void EnsureAmountPositive(decimal amount)
        {
            if (amount <= 0)
            {
                throw new ArgumentException("Amount must be positive.");
            }
        }

        public virtual void EnsureOwnership(string ownerID)
        {
            if (ownerID != OwnerID)
            {
                throw new UnauthorizedAccessException("You don't own this account, operation is not authorized.");
            }
        }
    }
}