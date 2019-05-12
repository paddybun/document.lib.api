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

            // N to M mapping
            modelBuilder.Entity<DocumentTag>()
                .HasOne(dt => dt.LibDocument)
                .WithMany(dt => dt.Tags)
                .HasForeignKey(dt => dt.LibDocumentId);

            modelBuilder.Entity<DocumentTag>()
                .HasOne(dt => dt.Tag)
                .WithMany(dt => dt.Documents)
                .HasForeignKey(dt => dt.LibDocumentId);
        }
    }
}
