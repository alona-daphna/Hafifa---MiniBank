using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniBank.Models
{
    internal class User(string name)
    {
        private string ID { get; set; } = Guid.NewGuid().ToString();
        private string Name { get; set; } = name;
    }
}
