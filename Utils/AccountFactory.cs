using MiniBank.Enums;
using MiniBank.Models;

namespace MiniBank.Utils
{
    internal class AccountFactory
    {
        public Dictionary<int, (Func<Account> creator, string name)> AccountCreators { get; set; }

        public AccountFactory()
        {
            AccountCreators = new Dictionary<int, (Func<Account>, string)>
            {
                {(int)AccountType.Simple, (() => new SimpleAccount(), "Simple") },
                {(int)AccountType.Vip, (() => new VipAccount(), "VIP") },
            };
        }

        public Account Create(int type) => AccountCreators.TryGetValue(type, out var creatorInfo)
            ? creatorInfo.creator() 
            : throw new ArgumentException("Invalid account type.");
    }
}