using FluentNHibernate.Automapping;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using Microsoft.Extensions.Configuration;
using MiniBank.Models;
using MiniBank.Nhibernate.Mappings;
using NHibernate;

namespace MiniBank.Nhibernate
{
    internal class NhibernateConfig
    {
        private static NhibernateConfig instance;
        internal ISessionFactory SessionFactory { get; set; }


        private NhibernateConfig()
        {
            var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

            var persistenceModel = AutoMap.AssemblyOf<User>()
                .Where(t => t == typeof(User))
                .Conventions.Add<TableNameConvention>();



            SessionFactory = Fluently.Configure()
                .Database(MsSqlConfiguration.MsSql2012.ConnectionString(configuration.GetConnectionString("DB")))
                .Mappings(m =>
                    {
                        m.AutoMappings.Add(persistenceModel);
                        m.FluentMappings.AddFromAssemblyOf<AccountMap>();
                    })
                .BuildSessionFactory();
        }

        internal static NhibernateConfig GetInstance()
        {
            instance ??= new NhibernateConfig();

            return instance;
        }
    }
}