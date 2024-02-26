using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniBank.Models
{
    internal class SimpleAccount(string ownerID) : Account(ownerID)
    {
        public override void Withdraw(float amount)
        {
            if (Amount - amount < 0)
            {
                throw new ArgumentException("This account cannot be in overdraft");
            }

            base.Withdraw(amount);
        }
    }
}
