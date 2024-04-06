using document.lib.ef.Entities;
using Microsoft.EntityFrameworkCore;

namespace document.lib.ef;

public class DocumentLibContext : DbContext
{
    public DbSet<EfCategory> Categories { get; set; }
    public DbSet<EfDocument> Documents { get; set; }
    public DbSet<EfFolder> Folders { get; set; }
    public DbSet<EfRegister> Registers { get; set; }
    public DbSet<EfTag> Tags { get; set; }
    public DbSet<EfTagAssignment> TagAssignments { get; set; }

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

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        UpdateNewEntities();
        UpdateExistingEntites();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        UpdateNewEntities();
        UpdateExistingEntites();
        return await base.SaveChangesAsync(cancellationToken);
    }

    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = new CancellationToken())
    {
        UpdateNewEntities();
        UpdateExistingEntites();
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<EfCategory>().HasIndex(x => x.Name).IsUnique();
    }

    private void UpdateNewEntities()
    {
        var insertedEntries = ChangeTracker.Entries()
            .Where(x => x.State == EntityState.Added)
            .Select(x => x.Entity);

        var now = DateTimeOffset.Now;
        foreach (var insertedEntry in insertedEntries)
        {
            //If the inserted object is an BaseField. 
            
            if (insertedEntry is EfBaseFields bf)
            {
                bf.DateCreated = now;
                bf.DateModified = now;
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
            if (modifiedEntry is EfBaseFields bf)
            {
                bf.DateModified = DateTimeOffset.Now;
            }
        }
    }
}