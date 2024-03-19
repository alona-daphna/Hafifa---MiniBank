using MiniBank.Models;
using NHibernate.Mapping.ByCode.Impl;

namespace MiniBank.Utils
{
    internal class AccountFactory
    {
        public Dictionary<int, (Func<User, Account> creator, string name)> AccountCreators { get; set; }

        public AccountFactory()
        {
            AccountCreators = new Dictionary<int, (Func<User, Account>, string)>
            {
                {1, ((owner) => new SimpleAccount(owner), "Simple") },
                {2, ((owner) => new VipAccount(owner), "VIP") },
            };
        }


        public Account Create(int type, User owner) => AccountCreators.TryGetValue(type, out var creatorInfo)
            ? creatorInfo.creator(owner) 
            : throw new ArgumentException("Invalid account type.");
    }
}