using FluentNHibernate.Automapping;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using Microsoft.Extensions.Configuration;
using MiniBank.Mappings;
using MiniBank.Models;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniBank.Utils
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
                .Conventions.Add<PrimaryKeyConvention>();

            SessionFactory = Fluently.Configure()
                .Database(MsSqlConfiguration.MsSql2012.ConnectionString(configuration.GetConnectionString("DB")).ShowSql())
                .Mappings(m => m.AutoMappings.Add(persistenceModel))
                .BuildSessionFactory();
        }

        internal static NhibernateConfig GetInstance()
        {
            if ( instance == null )
            {
                instance = new NhibernateConfig();
            }

            return instance;
        }
    }
}
