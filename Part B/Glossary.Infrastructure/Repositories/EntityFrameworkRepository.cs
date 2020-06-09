using Glossary.Core.Abstract.Repositories;
using Glossary.Core.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Glossary.Infrastructure.Repositories
{
    public class EntityFrameworkRepository<TEntity> : IRepository<TEntity> where TEntity : EntityBase
    {
        protected DbContext Context { get; }

        public EntityFrameworkRepository(DbContext context)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
            Context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        public void Add(TEntity glossary) =>
            Context.Set<TEntity>().Add(glossary);

        public void Update(TEntity glossary)
        {
            Context.Entry(glossary).State = EntityState.Modified;
        }

        public async Task DeleteAsync(Guid glossaryId)
        {
            var glossaryEntity = await Context.Set<TEntity>().FindAsync(glossaryId);
            Context.Set<TEntity>().Remove(glossaryEntity);
        }

        public async Task<TEntity> GetAsync(Guid glossaryId) =>
            await Context.Set<TEntity>().FindAsync(glossaryId);

        public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            await Context.SaveChangesAsync(cancellationToken);
        }
    }
}
