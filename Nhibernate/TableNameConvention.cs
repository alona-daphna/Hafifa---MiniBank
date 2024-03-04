using FluentNHibernate.Conventions;

namespace MiniBank.Nhibernate
{
    internal class TableNameConvention : IClassConvention
    {
        public void Apply(FluentNHibernate.Conventions.Instances.IClassInstance instance)
        {
            instance.Table(instance.EntityType.Name + "s");
        }
    }
}