using HomeBanking.Models;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;
using System.Linq;
using System;
using Microsoft.EntityFrameworkCore;
using HomeBanking.Repositories.Interfaces;

namespace HomeBanking.Repositories
{
    // Interfaz que define métodos de operaciones de acceso a datos genéricas
    public abstract class RepositoryBase<T> : IRepositoryBase<T> where T : class
    {
        // Contexto de la base de datos
        protected HomeBankingContext RepositoryContext { get; set; }

        // Constructor que inyecta el contexto de la base de datos
        public RepositoryBase(HomeBankingContext repositoryContext)
        {
            this.RepositoryContext = repositoryContext;
        }

        //Retorna una colección IQueryable de objetos. No recibe ningún parámetro y devuelve todos los objetos sin filtro.
        public IQueryable<T> FindAll()
        {
            return this.RepositoryContext.Set<T>().AsNoTrackingWithIdentityResolution();
        }

        //Puede recibir un IQueryable con una condición de filtro.
        //IIncludableQueryable para usar las relaciones
        public IQueryable<T> FindAll(Func<IQueryable<T>, IIncludableQueryable<T, object>> includes = null)
        {
            IQueryable<T> queryable = this.RepositoryContext.Set<T>();

            if (includes != null)
            {
                queryable = includes(queryable);
            }
            return queryable.AsNoTrackingWithIdentityResolution();
        }

        public IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression)
        {
            return this.RepositoryContext.Set<T>().Where(expression).AsNoTrackingWithIdentityResolution();
        }

        public void Create(T entity)
        {
            this.RepositoryContext.Set<T>().Add(entity);
        }

        public void Update(T entity)
        {
            this.RepositoryContext.Set<T>().Update(entity);
        }

        public void Delete(T entity)
        {
            this.RepositoryContext.Set<T>().Remove(entity);
        }

        public void SaveChanges()
        {
            this.RepositoryContext.SaveChanges();
        }
    }
}
