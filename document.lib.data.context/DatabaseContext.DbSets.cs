using document.lib.data.entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace document.lib.data.context;

public partial class DatabaseContext
{
    public DbSet<Category> Categories { get; set; }
    public DbSet<Document> Documents { get; set; }
    public DbSet<Folder> Folders { get; set; }
    public DbSet<Register> Registers { get; set; }
    public DbSet<RegisterDescription> RegisterDescriptions { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<TagAssignment> TagAssignments { get; set; }
}