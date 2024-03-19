namespace MiniBank.Models
{
    internal class User
    {
        public virtual string ID { get; set; }
        public virtual string Username { get; set; }
        public virtual string Password { get; set; }
        public virtual List<Account> Accounts { get; set; }

        public User(string username, string password)
        {
            Accounts = [];
            Username = username;
            Password = password;
            ID = Guid.NewGuid().ToString();
        }
    }
}