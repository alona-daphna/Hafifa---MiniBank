namespace MiniBank.Models
{
    internal class SimpleAccount : Account
    {
        public override void Withdraw(decimal amount)
        {
            if (Balance - amount < 0)
            {
                throw new ArgumentException("This account cannot be in overdraft");
            }

            base.Withdraw(amount);
        }
    }
}