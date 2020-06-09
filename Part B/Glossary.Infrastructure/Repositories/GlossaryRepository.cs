using Glossary.Core.Abstract.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Glossary.Infrastructure.Repositories
{
    public class GlossaryRepository : EntityFrameworkRepository<Core.Models.Glossary>, IGlossaryRepository
    {
        public GlossaryRepository(GlossaryDbContext context): base(context)
        {
        }

        public async Task<IEnumerable<Core.Models.Glossary>> GetAllAsync() =>
            await Context.Set<Core.Models.Glossary>().OrderBy(x => x.Term).ToListAsync();
    }
}
