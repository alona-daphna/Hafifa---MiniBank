using MiniBank.Exceptions;

namespace MiniBank.Models
{
    internal abstract class Account
    {
        public virtual string ID { get; set; }
        public virtual decimal Balance { get; set; }
        public virtual User Owner { get; set; }

        public Account(User owner)
        {
            ID = Guid.NewGuid().ToString();
            Balance = 0;
            Owner = owner;
        }

        protected Account()
        {
            
        }
    }
}