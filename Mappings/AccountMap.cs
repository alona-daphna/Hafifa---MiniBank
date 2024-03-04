using FluentNHibernate.Mapping;
using MiniBank.Models;

namespace MiniBank.Mappings
{
    internal class AccountMap : ClassMap<Account>
    {
        public AccountMap()
        {
            Table("Accounts");

            DiscriminateSubClassesOnColumn("Type");

            Id(x => x.ID);
            Map(x => x.OwnerID);
            Map(x => x.Balance);
        }
    }
}