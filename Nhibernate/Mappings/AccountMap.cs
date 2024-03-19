using FluentNHibernate.Mapping;
using MiniBank.Models;

namespace MiniBank.Nhibernate.Mappings
{
    internal class AccountMap : ClassMap<Account>
    {
        public AccountMap()
        {
            Table("Accounts");

            DiscriminateSubClassesOnColumn("Type");

            Id(x => x.ID);
            Map(x => x.Balance);
            References(x => x.Owner, "UserID");
        }
    }
}