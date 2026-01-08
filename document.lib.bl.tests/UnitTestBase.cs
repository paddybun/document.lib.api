using document.lib.data.context;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace document.lib.bl.tests;

public abstract class UnitTestBase
{
    private readonly int _folderOffset;
    private readonly int _registerOffset;
    protected readonly DatabaseContext Context;

    protected UnitTestBase()
    {
        var name = GetType().Name + Guid.NewGuid();
        UnitTestContext.RegisterTest(name);
        _folderOffset = UnitTestContext.GetFolderOffset(name);
        _registerOffset = UnitTestContext.GetRegisterOffset(name);
        
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();
        
        var options = new DbContextOptionsBuilder<DatabaseContext>()
            .UseSqlite(connection)
            .ConfigureWarnings(b => b.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        Context = new DatabaseContext(options);
        // Context.Database.EnsureDeleted();
        Context.Database.EnsureCreated();
    }
    protected int GetFolderId(int id)
    {
        return _folderOffset + id;
    }
    protected int GetRegisterId(int id)
    {
        return _registerOffset + id;
    }
}