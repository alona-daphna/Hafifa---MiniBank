using MiniBank.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniBank.Controllers
{
    internal class UserController
    {
        private DBConnection DBConnection { get; set; } = new DBConnection();

        internal void Create() { }

        internal void Delete() { }

        internal List<User> GetAll() { return new List<User>(); }

        internal User GetById(string id) { return new User(); }
    }
}
