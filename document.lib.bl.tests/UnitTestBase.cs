using document.lib.data.context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace document.lib.bl.tests;

public abstract class UnitTestBase
{
    private readonly int _offset;
    protected readonly DatabaseContext Context;

    protected UnitTestBase()
    {
        var name = GetType().Name + Guid.NewGuid();
        UnitTestContext.RegisterTest(name);
        _offset = UnitTestContext.GetFolderOffset(name);
        
        var options = new DbContextOptionsBuilder<DatabaseContext>()
            .UseInMemoryDatabase(name)
            .ConfigureWarnings(b => b.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        Context = new DatabaseContext(options);
        // Context.Database.EnsureDeleted();
        Context.Database.EnsureCreated();
    }
    protected int GetFolderId(int id)
    {
        return _offset + id;
    }
}