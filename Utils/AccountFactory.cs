using MiniBank.Enums;
using MiniBank.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniBank.Utils
{
    internal class AccountFactory
    {
        public Dictionary<int, Func<Account>> AccountCreators { get; set; }

        public AccountFactory()
        {
            AccountCreators = new Dictionary<int, Func<Account>>
            {
                {(int)AccountType.Simple, () => new SimpleAccount() },
                {(int)AccountType.Vip, () => new VipAccount() },
            };
        }

        public Account Create(int type) => AccountCreators.TryGetValue(type, out Func<Account>? ctor) 
            ? ctor() 
            : throw new ArgumentException("Invalid account type.");
    }
}
