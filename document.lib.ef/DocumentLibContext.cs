using document.lib.ef.Entities;
using Microsoft.EntityFrameworkCore;

namespace document.lib.ef;

public class DocumentLibContext : DbContext
{
    public DbSet<Category> Categories { get; set; }
    public DbSet<DocLibDocument> Documents { get; set; }
    public DbSet<Folder> Folders { get; set; }
    public DbSet<Register> Registers { get; set; }
    public DbSet<Tag> Tags { get; set; }

    public DocumentLibContext()
    {
    }

    public DocumentLibContext(DbContextOptions<DocumentLibContext> options)
    : base(options)
    {
    }

    public override int SaveChanges()
    {
        UpdateNewEntities();
        UpdateExistingEntites();
        return base.SaveChanges();
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        UpdateNewEntities();
        UpdateExistingEntites();
        return await base.SaveChangesAsync(cancellationToken);
    }

    private void UpdateNewEntities()
    {
        var insertedEntries = ChangeTracker.Entries()
            .Where(x => x.State == EntityState.Added)
            .Select(x => x.Entity);

        foreach (var insertedEntry in insertedEntries)
        {
            //If the inserted object is an BaseField. 
            if (insertedEntry is BaseFields bf)
            {
                bf.DateCreated = DateTimeOffset.UtcNow;
            }
        }
    }
    private void UpdateExistingEntites()
    {
        var modifiedEntries = ChangeTracker.Entries()
            .Where(x => x.State == EntityState.Modified)
            .Select(x => x.Entity);

        foreach (var modifiedEntry in modifiedEntries)
        {
            //If the inserted object is an BaseField. 
            if (modifiedEntry is BaseFields bf)
            {
                bf.DateModified = DateTimeOffset.UtcNow;
            }
        }
    }
}