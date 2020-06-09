using System.Collections.Generic;
using System.Threading.Tasks;

namespace Glossary.Core.Abstract.Repositories
{
    public interface IGlossaryRepository: IRepository<Models.Glossary>
    {
        Task<IEnumerable<Models.Glossary>> GetAllAsync();
    }
}
