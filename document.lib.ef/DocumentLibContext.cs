﻿using document.lib.data.entities;
using Microsoft.EntityFrameworkCore;

namespace document.lib.ef;

public class DocumentLibContext : DbContext
{
    public DbSet<Category> Categories { get; set; }
    public DbSet<Document> Documents { get; set; }
    public DbSet<Folder> Folders { get; set; }
    public DbSet<Register> Registers { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<TagAssignment> TagAssignments { get; set; }

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
        
        modelBuilder
            .Entity<Category>()
            .HasIndex(x => x.Name)
            .IsUnique();
        
        // remove cascade delete
        foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
        {
            relationship.DeleteBehavior = DeleteBehavior.Restrict;
        }
        
        // enable cascade delete for tag assignments
        modelBuilder.Entity<TagAssignment>()
            .HasOne(x => x.Document)
            .WithMany(x => x.Tags)
            .OnDelete(DeleteBehavior.Cascade);
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
            
            if (insertedEntry is BaseFields bf)
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
            if (modifiedEntry is BaseFields bf)
            {
                bf.DateModified = DateTimeOffset.Now;
            }
        }
    }
}