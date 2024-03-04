using FluentNHibernate.Mapping;
using MiniBank.Models;

namespace MiniBank.Mappings
{
    internal class SimpleAccountMap : SubclassMap<SimpleAccount>
    {
        public SimpleAccountMap()
        {
            DiscriminatorValue(1);
        }
    }
}