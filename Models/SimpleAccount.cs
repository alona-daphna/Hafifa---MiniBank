using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniBank.Models
{
    internal class SimpleAccount : Account
    {
        public override void Withdraw(float amount)
        {
            if (Balance - amount < 0)
            {
                throw new ArgumentException("This account cannot be in overdraft");
            }

            base.Withdraw(amount);
        }
    }
}
