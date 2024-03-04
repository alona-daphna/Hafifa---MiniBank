using FluentNHibernate.Mapping;
using MiniBank.Models;

namespace MiniBank.Nhibernate.Mappings
{
    internal class VipAccountMap : SubclassMap<VipAccount>
    {
        public VipAccountMap()
        {
            DiscriminatorValue(2);
        }
    }
}