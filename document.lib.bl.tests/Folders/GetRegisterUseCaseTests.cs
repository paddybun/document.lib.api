using document.lib.bl.shared;
using document.lib.bl.shared.DocumentHandling;
using document.lib.data.entities;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace document.lib.bl.tests.Folders;

public class GetRegisterUseCaseTests: UnitTestBase
{
    public GetRegisterUseCaseTests()
    {
        InitTestData();
    }

    [Fact]
    public async Task ProvidedValidInputs_ExecuteAsync_ReturnsNextRegister()
    {
        var uow = new UnitOfWork(Context);
        var sut = new GetRegisterUseCase(NullLogger<GetRegisterUseCase>.Instance, new NextDescriptionQuery(NullLogger<NextDescriptionQuery>.Instance));
        var result = await sut.ExecuteAsync(uow, new(GetFolderId(1)));
        var expectedName = "2";
        var expectedDisplayName = "anyExistingString";
        Assert.Equal(expectedName, result.Value!.Name);
        Assert.Equal(expectedDisplayName, result.Value!.DisplayName);
    }

    [Fact]
    public async Task ProvidedValidInputs_ExecuteAsync_ReturnsNewRegisterAsNextRegister()
    {
        var uow = new UnitOfWork(Context);
        var sut = new GetRegisterUseCase(NullLogger<GetRegisterUseCase>.Instance,
            new NextDescriptionQuery(NullLogger<NextDescriptionQuery>.Instance));
        var result = await sut.ExecuteAsync(uow, new(GetFolderId(2)));
        var expectedName = "3";
        var expectedDisplayName = "3";
        Assert.Equal(expectedName, result.Value!.Name);
        Assert.Equal(expectedDisplayName, result.Value!.DisplayName);
    }

    [Fact]
    public async Task ProvidedValidInputs_ExecuteAsync_CreatesFirstRegisterForFreshFolder()
    {
        var uow = new UnitOfWork(Context);
        var sut = new GetRegisterUseCase(NullLogger<GetRegisterUseCase>.Instance,
            new NextDescriptionQuery(NullLogger<NextDescriptionQuery>.Instance));
        var result = await sut.ExecuteAsync(uow, new(GetFolderId(3)));
        var expectedName = "1";
        var expectedDisplayName = "Bar";
        Assert.Equal(expectedName, result.Value!.Name);
        Assert.Equal(expectedDisplayName, result.Value!.DisplayName);
    }

    private void InitTestData()
    {
        // Global data
        Context.RegisterDescriptions.Add(new RegisterDescription { Group = "default", Name = "1", DisplayName = "1", Order = 1 });
        Context.RegisterDescriptions.Add(new RegisterDescription { Group = "default", Name = "2", DisplayName = "2", Order = 2 });
        Context.RegisterDescriptions.Add(new RegisterDescription { Group = "default", Name = "3", DisplayName = "3", Order = 3 });
        Context.RegisterDescriptions.Add(new RegisterDescription { Group = "default", Name = "4", DisplayName = "4", Order = 4 });
        Context.RegisterDescriptions.Add(new RegisterDescription { Group = "default", Name = "5", DisplayName = "5", Order = 5 });
        
        Context.RegisterDescriptions.Add(new RegisterDescription { Group = "foo", Name = "1", DisplayName = "Bar", Order = 1 });
        Context.RegisterDescriptions.Add(new RegisterDescription { Group = "foo", Name = "2", DisplayName = "Baz", Order = 2 });
        
        // Folder 1
        Context.Folders.Add(new Folder { Id = GetFolderId(1), Name = "UnitTest", MaxDocumentsRegister = 2, MaxDocumentsFolder = 10, DescriptionGroup = "default" });
        Context.Registers.Add(new (){ Name = "1", DocumentCount = 2, FolderId = GetFolderId(1), DescriptionId = 1 });
        Context.Registers.Add(new (){ Name = "2", DisplayName = "anyExistingString", DocumentCount = 1, FolderId = GetFolderId(1), DescriptionId = 2 });

        // Folder 2
        Context.Folders.Add(new Folder { Id = GetFolderId(2), Name = "UnitTest", MaxDocumentsRegister = 2, MaxDocumentsFolder = 10, DescriptionGroup = "default" });
        Context.Registers.Add(new (){ Name = "1", DocumentCount = 2, FolderId = GetFolderId(2), DescriptionId = 1 });
        Context.Registers.Add(new (){ Name = "2", DocumentCount = 2, FolderId = GetFolderId(2), DescriptionId = 2 });
        
        // Folder 3
        Context.Folders.Add(new Folder { Id = GetFolderId(3), Name = "UnitTest", MaxDocumentsRegister = 2, MaxDocumentsFolder = 10, DescriptionGroup = "foo" });
        
        Context.SaveChanges();
    }

    
}