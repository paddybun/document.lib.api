using System.Linq;
using System.Threading.Tasks;
using document.lib.api.Controllers;
using document.lib.api.Models;
using Microsoft.EntityFrameworkCore;

namespace document.lib.api.Services
{
    public class SearchService
    {
        private readonly DocumentlibContext _dbContext;

        public SearchService(DocumentlibContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<LibDocument[]> Search(SearchController.SearchRequest searchRequest)
        {
            IQueryable<LibDocument> query = _dbContext.LibDocuments
                .Include(doc => doc.Tags)
                    .ThenInclude(tag => tag.Tag);

            if (!string.IsNullOrWhiteSpace(searchRequest.Term))
            {
                query = query.Where(x => x.Name.Contains(searchRequest.Term));
            }

            if (searchRequest.From.HasValue)
            {
                query = query.Where(doc => doc.Date >= searchRequest.From.Value);
            }

            if (searchRequest.To.HasValue)
            {
                query = query.Where(doc => doc.Date <= searchRequest.To.Value);
            }

            if (searchRequest.Page != null && searchRequest.PageSize != null)
            {
                var page = searchRequest.Page.Value;
                var count = searchRequest.PageSize.Value;
                query = query.Skip(page*count-1).Take(count);
            }
            var res = await query.ToListAsync();



            return res.ToArray();
        }
    }
}