using document.lib.core;
using document.lib.data.context;
using Microsoft.EntityFrameworkCore;

namespace document.lib.bl.shared;

public class UnitOfWork(DatabaseContext context): IUnitOfWork<DatabaseContext>
{
    public DatabaseContext Connection { get; } = context;

    public static UnitOfWork Create(IDbContextFactory<DatabaseContext> context)
    {
        return new UnitOfWork(context.CreateDbContext());
    }

    public static async Task<UnitOfWork> CreateAsync(IDbContextFactory<DatabaseContext> context)
    {
        var ctx = await context.CreateDbContextAsync();
        return new UnitOfWork(ctx);
    }

    public async Task BeginTransactionAsync()
    {
        await Connection.Database.BeginTransactionAsync();
    }

    public async Task CommitTransactionAsync()
    {
        await Connection.SaveChangesAsync();
        await Connection.Database.CommitTransactionAsync();
    }

    public async Task RollbackTransactionAsync()
    {
        await Connection.Database.RollbackTransactionAsync();
    }

    public void Dispose()
    {
        Connection.Dispose();
    }
}