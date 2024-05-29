using document.lib.shared.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace document.lib.shared.Repositories;

public abstract class SqlRepositoryBase(DbContext context) : IRepository
{
    public virtual async Task SaveAsync()
    {
        await context.SaveChangesAsync();
    }
}