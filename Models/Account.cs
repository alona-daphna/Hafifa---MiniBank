using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniBank.Models
{
    internal abstract class Account(string ownerID)
    {
        protected string ID { get; set; } = Guid.NewGuid().ToString();
        protected float Amount { get; set; } = 0;
        protected string OwnerID { get; set; } = ownerID;

        public void Deposit(float amount) 
        {
            EnsureAmountPositive(amount);
            Amount += amount;
        }
        public virtual void Withdraw(float amount) 
        { 
            EnsureAmountPositive(amount);
            Amount -= amount;
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
