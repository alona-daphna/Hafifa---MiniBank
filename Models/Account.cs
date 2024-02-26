using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniBank.Models
{
    internal abstract class Account()
    {
        internal string ID { get; set; }
        internal float Balance { get; set; }
        internal string OwnerID { get; set; }

        public void Deposit(float amount) 
        {
            EnsureAmountPositive(amount);
            Balance += amount;
        }
        public virtual void Withdraw(float amount) 
        { 
            EnsureAmountPositive(amount);
            Balance -= amount;
        }

        private void EnsureAmountPositive(float amount)
        {
            if (amount <= 0)
            {
                throw new ArgumentException("Amount must be positive.");
            }
        }
    }
}
