using MiniBank.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniBank.Controllers
{
    internal class AccountController
    {
        private DBConnection DBConnection { get; set; } = new DBConnection();
        
        internal List<Account> GetByOwnerId(string id) { return new List<Account>(); }

        internal void Delete(string ownerId, string accountId) { }

        internal string Create(string ownerId) { return "generated account ID"; }

        internal void Deposit(string ownerId, string accountId) { }

        internal void Withdraw(string ownerId, string accountId) { }
    }
}
