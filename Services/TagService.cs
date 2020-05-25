using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace document.lib.api.Services
{
    public class TagService
    {
        private readonly DocumentlibContext _dbContext;

        public TagService(DocumentlibContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task DeleteTag(Guid id)
        {
            var toDelete = await _dbContext.Tags
                .Include(tag => tag.Documents)
                .ThenInclude(docs => docs.LibDocument)
                .SingleOrDefaultAsync(tag => tag.Id == id);

            toDelete.Documents.Clear();
            _dbContext.Tags.Remove(toDelete);
            await _dbContext.SaveChangesAsync();
        }
    }
}