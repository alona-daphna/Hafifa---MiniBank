using FluentNHibernate.Conventions.Inspections;
using MiniBank.Nhibernate;
using NHibernate.Linq;
using System.Linq.Expressions;

namespace MiniBank.Controllers
{
    internal class BaseController
    {
        private NhibernateConfig NhibernateConfig { get; set; } = NhibernateConfig.GetInstance();

        public List<T> GetAll<T>()
        {
            using var session = NhibernateConfig.SessionFactory.OpenSession();
            return session.Query<T>().ToList();
        }

        public T GetEntityByIdentifier<T>(string identifier)
        {
            using var session = NhibernateConfig.SessionFactory.OpenSession();

            return session.Get<T>(identifier);
        }

        public List<T>? GetByPredicateEagerlyLoad<T>(Func<T, bool> predicate, params Expression<Func<T, object>>[] fetchExpressions)
        {
            using var session = NhibernateConfig.SessionFactory.OpenSession();

            var query = session.Query<T>();

            foreach (var expression in fetchExpressions)
            {
                query = query.Fetch(expression);
            }

            return query.Where(predicate).ToList();
        }

        public void SaveEntity<T>(T entity)
        {
            using var session = NhibernateConfig.SessionFactory.OpenSession();
            using var transaction = session.BeginTransaction();
            session.Save(entity);
            transaction.Commit();
        }

        public void UpdateEntity<T>(T entity)
        {
            using var session = NhibernateConfig.SessionFactory.OpenSession();
            using var transaction = session.BeginTransaction();
            session.Update(entity);
            transaction.Commit();
        }

        public void DeleteEntity<T>(string identifier)
        {
            using var session = NhibernateConfig.SessionFactory.OpenSession();
            using var transaction = session.BeginTransaction();
            var entity = session.Get<T>(identifier);
            session.Delete(entity);
            transaction.Commit();
        }
    }
}