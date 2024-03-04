using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Instances;

namespace MiniBank.Mappings
{
    internal class CustomPrimaryKeyConvention : IIdConvention
    {
        public void Apply(IIdentityInstance instance)
        {
            instance.Column("ID");
        }
    }
}
