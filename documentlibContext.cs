using System;
using System.Threading;
using System.Threading.Tasks;
using document.lib.api.Models;
using Microsoft.EntityFrameworkCore;

namespace document.lib.api
{
    public partial class DocumentlibContext : DbContext
    {
        public DbSet<Category> Categories { get; set; }
        public DbSet<DocumentTag> DocumentTags { get; set; }
        public DbSet<Folder> Folders { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<LibDocument> LibDocuments { get; set; }
        public DbSet<Register> Registers { get; set; }

        public DocumentlibContext()
        {
        }

        public DocumentlibContext(DbContextOptions<DocumentlibContext> options)
            : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }

        public override int SaveChanges()
        {
            OnBeforeSaving();
            return base.SaveChanges();
        }


        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            OnBeforeSaving();
            return base.SaveChangesAsync(cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "2.2.4-servicing-10062");

            // Primary Keys
            modelBuilder.Entity<Category>()
                .HasKey(c => c.Id);
            modelBuilder.Entity<DocumentTag>()
                .HasKey(dt => dt.Id);
            modelBuilder.Entity<Folder>()
                .HasKey(f => f.Id);
            modelBuilder.Entity<LibDocument>()
                .HasKey(ld => ld.Id);
            modelBuilder.Entity<Register>()
                .HasKey(t => t.Id);
            modelBuilder.Entity<Tag>()
                .HasKey(t => t.Id);

            // Default Values
            modelBuilder.Entity<Category>()
                .Property(c => c.Id)
                .HasDefaultValueSql("newsequentialid()");
            modelBuilder.Entity<DocumentTag>()
                .Property(dt => dt.Id)
                .HasDefaultValueSql("newsequentialid()");
            modelBuilder.Entity<Folder>()
                .Property(f => f.Id)
                .HasDefaultValueSql("newsequentialid()");
            modelBuilder.Entity<LibDocument>()
                .Property(ld => ld.Id)
                .HasDefaultValueSql("newsequentialid()");
            modelBuilder.Entity<Tag>()
                .Property(t => t.Id)
                .HasDefaultValueSql("newsequentialid()");
            modelBuilder.Entity<Register>()
                .Property(t => t.Id)
                .HasDefaultValueSql("newsequentialid()");
            modelBuilder.Entity<LibDocument>()
                .Property(ld => ld.Date)
                .HasDefaultValue(DateTimeOffset.Now);

            // N to M mapping
            modelBuilder.Entity<DocumentTag>()
                .HasOne(dt => dt.LibDocument)
                .WithMany(dt => dt.Tags)
                .HasForeignKey(dt => dt.TagId);

            modelBuilder.Entity<DocumentTag>()
                .HasOne(dt => dt.Tag)
                .WithMany(dt => dt.Documents)
                .HasForeignKey(dt => dt.LibDocumentId);

            // Relations
            modelBuilder.Entity<Register>()
                .HasOne(reg => reg.Folder)
                .WithMany(folder => folder.Registers)
                .HasForeignKey(fk => fk.FolderId);

            modelBuilder.Entity<Register>()
                .HasMany(reg => reg.Documents)
                .WithOne(doc => doc.Register)
                .HasForeignKey(fk => fk.RegisterId);

            modelBuilder.Entity<Folder>()
                .HasMany(f => f.Registers)
                .WithOne(reg => reg.Folder);

            modelBuilder.Entity<LibDocument>()
                .HasOne(doc => doc.Register)
                .WithMany(reg => reg.Documents)
                .HasForeignKey(fk => fk.RegisterId);

            // Hidden Properties
            modelBuilder.Entity<Category>().Property<DateTimeOffset>("CreatedAt");
            modelBuilder.Entity<Category>().Property<DateTimeOffset>("LastUpdatedAt");
            modelBuilder.Entity<DocumentTag>().Property<DateTimeOffset>("CreatedAt");
            modelBuilder.Entity<DocumentTag>().Property<DateTimeOffset>("LastUpdatedAt");
            modelBuilder.Entity<Folder>().Property<DateTimeOffset>("CreatedAt");
            modelBuilder.Entity<Folder>().Property<DateTimeOffset>("LastUpdatedAt");
            modelBuilder.Entity<LibDocument>().Property<DateTimeOffset>("CreatedAt");
            modelBuilder.Entity<LibDocument>().Property<DateTimeOffset>("LastUpdatedAt");
            modelBuilder.Entity<Tag>().Property<DateTimeOffset>("CreatedAt");
            modelBuilder.Entity<Tag>().Property<DateTimeOffset>("LastUpdatedAt");
            modelBuilder.Entity<Register>().Property<DateTimeOffset>("CreatedAt");
            modelBuilder.Entity<Register>().Property<DateTimeOffset>("LastUpdatedAt");
        }

        private void OnBeforeSaving()
        {
            var entries = ChangeTracker.Entries();
            foreach (var entry in entries)
            {
                var now = DateTimeOffset.Now;
                switch (entry.State)
                {
                    case EntityState.Modified:
                        entry.CurrentValues["LastUpdatedAt"] = now;
                        break;

                    case EntityState.Added:
                        entry.CurrentValues["CreatedAt"] = now;
                        entry.CurrentValues["LastUpdatedAt"] = now;
                        break;
                    case EntityState.Detached:
                    case EntityState.Unchanged:
                    case EntityState.Deleted:
                    default:
                        break;
                }
            }
        }
    }
}
