using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Glossary.Core.Abstract.Repositories
{
    public interface IRepository<T>
    {
        void Add(T entity);
        void Update(T entity);
        Task DeleteAsync(Guid entityId);
        Task<T> GetAsync(Guid entityId);
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}