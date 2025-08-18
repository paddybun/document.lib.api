using document.lib.bl.shared;
using document.lib.bl.shared.Folders;
using document.lib.data.context;
using document.lib.data.entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace document.lib.bl.tests;

public class GetRegisterUseCaseTests
{
    private readonly DatabaseContext _context;

    public GetRegisterUseCaseTests()
    {
        var options = new DbContextOptionsBuilder<DatabaseContext>()
            .UseInMemoryDatabase("DatabaseContext")
            .ConfigureWarnings(b => b.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;
        _context = new DatabaseContext(options);
        _context.Database.EnsureDeleted();
        _context.Database.EnsureCreated();
        InitTestData();
    }

    [Fact]
    public async Task ProvidedValidInputs_ExecuteAsync_ReturnsNextRegister()
    {
        var uow = new UnitOfWork(_context);
        var sut = new GetRegisterUseCase(NullLogger<GetRegisterUseCase>.Instance, new NextDescriptionQuery(NullLogger<NextDescriptionQuery>.Instance));
        var result = await sut.ExecuteAsync(uow, new(1));
        var expectedName = "2";
        var expectedDisplayName = "2";
        Assert.Equal(expectedName, result.Value!.Name);
        Assert.Equal(expectedDisplayName, result.Value!.DisplayName);
    }

    [Fact]
    public async Task ProvidedValidInputs_ExecuteAsync_ReturnsNewRegisterAsNextRegister()
    {
        var uow = new UnitOfWork(_context);
        var sut = new GetRegisterUseCase(NullLogger<GetRegisterUseCase>.Instance,
            new NextDescriptionQuery(NullLogger<NextDescriptionQuery>.Instance));
        var result = await sut.ExecuteAsync(uow, new(2));
        var expectedName = "3";
        var expectedDisplayName = "3";
        Assert.Equal(expectedName, result.Value!.Name);
        Assert.Equal(expectedDisplayName, result.Value!.DisplayName);
    }

    [Fact]
    public async Task ProvidedValidInputs_ExecuteAsync_CreatesFirstRegisterForFreshFolder()
    {
        var uow = new UnitOfWork(_context);
        var sut = new GetRegisterUseCase(NullLogger<GetRegisterUseCase>.Instance,
            new NextDescriptionQuery(NullLogger<NextDescriptionQuery>.Instance));
        var result = await sut.ExecuteAsync(uow, new(3));
        var expectedName = "1";
        var expectedDisplayName = "Bar";
        Assert.Equal(expectedName, result.Value!.Name);
        Assert.Equal(expectedDisplayName, result.Value!.DisplayName);
    }

    private void InitTestData()
    {
        // Global data
        _context.RegisterDescriptions.Add(new RegisterDescription { Group = "default", Name = "1", DisplayName = "1", Order = 1 });
        _context.RegisterDescriptions.Add(new RegisterDescription { Group = "default", Name = "2", DisplayName = "2", Order = 2 });
        _context.RegisterDescriptions.Add(new RegisterDescription { Group = "default", Name = "3", DisplayName = "3", Order = 3 });
        _context.RegisterDescriptions.Add(new RegisterDescription { Group = "default", Name = "4", DisplayName = "4", Order = 4 });
        _context.RegisterDescriptions.Add(new RegisterDescription { Group = "default", Name = "5", DisplayName = "5", Order = 5 });
        
        _context.RegisterDescriptions.Add(new RegisterDescription { Group = "foo", Name = "1", DisplayName = "Bar", Order = 1 });
        _context.RegisterDescriptions.Add(new RegisterDescription { Group = "foo", Name = "2", DisplayName = "Baz", Order = 2 });
        
        // Folder 1
        _context.Folders.Add(new Folder { Id = 1, Name = "UnitTest", MaxDocumentsRegister = 2, MaxDocumentsFolder = 10, DescriptionGroup = "default" });
        _context.Registers.Add(new (){ Name = "1", DocumentCount = 2, FolderId = 1, DescriptionId = 1 });
        _context.Registers.Add(new (){ Name = "2", DocumentCount = 1, FolderId = 1, DescriptionId = 2 });
        
        // Folder 2
        _context.Folders.Add(new Folder { Id = 2, Name = "UnitTest", MaxDocumentsRegister = 2, MaxDocumentsFolder = 10, DescriptionGroup = "default" });
        _context.Registers.Add(new (){ Name = "1", DocumentCount = 2, FolderId = 2, DescriptionId = 1 });
        _context.Registers.Add(new (){ Name = "2", DocumentCount = 2, FolderId = 2, DescriptionId = 2 });
        
        // Folder 3
        _context.Folders.Add(new Folder { Id = 3, Name = "UnitTest", MaxDocumentsRegister = 2, MaxDocumentsFolder = 10, DescriptionGroup = "foo" });
        
        _context.SaveChanges();
    }
}