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
                {1, (() => new SimpleAccount(), "Simple") },
                {2, (() => new VipAccount(), "VIP") },
            };
        }


        public Account Create(int type) => AccountCreators.TryGetValue(type, out var creatorInfo)
            ? creatorInfo.creator() 
            : throw new ArgumentException("Invalid account type.");
    }
}