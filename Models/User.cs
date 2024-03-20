namespace MiniBank.Models
{
    internal class User
    {
        public virtual string Username { get; set; }
        public virtual string Password { get; set; }
        public virtual IList<Account> Accounts { get; set; }

        public User(string username, string password)
        {
            Accounts = [];
            Username = username;
            Password = password;
        }

        public User()
        {
            
        }
    }
}