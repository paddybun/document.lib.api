using document.lib.bl.shared;
using document.lib.bl.shared.Folders;
using document.lib.data.entities;
using document.lib.data.models.Folders;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace document.lib.bl.tests;

public class SaveFolderUseCaseTests: UnitTestBase
{
    public SaveFolderUseCaseTests()
    {
        InitTestData();
    }

    [Fact]
    public async Task GivenValidParameters_ExecuteAsync_ShouldUpdateExistingFolder()
    {
        var expected = new FolderSaveModel(false)
        {
            DescriptionGroup = "default",
            MaxDocumentsFolder = 1337,
            MaxDocumentsRegister = 1337,
            DisplayName = "Test successful",
            Id = GetFolderId(1)
        };
        
        using var uow = new UnitOfWork(Context);
        var sut = new SaveFolderUseCase(NullLogger<SaveFolderUseCase>.Instance, new FolderQuery(NullLogger<FolderQuery>.Instance));
        var actual = await sut.ExecuteAsync(uow, new() { Folder = expected });
        
        Assert.True(actual.IsSuccess);
        Assert.NotNull(actual.Value);
        Assert.Equal(expected.MaxDocumentsFolder, actual.Value.MaxDocumentsFolder);
        Assert.Equal(expected.MaxDocumentsRegister, actual.Value.MaxDocumentsRegister);
        Assert.Equal(expected.DisplayName, actual.Value.DisplayName);
    }

    [Fact]
    public async Task GivenValidParameters_ExecuteAsync_ShouldCreateFolder()
    {
        var expected = new FolderSaveModel(true)
        {
            DescriptionGroup = "default",
            MaxDocumentsFolder = 1337,
            MaxDocumentsRegister = 1337,
            DisplayName = "Test successful",
            Id = GetFolderId(1)
        };
        
        using var uow = new UnitOfWork(Context);
        var sut = new SaveFolderUseCase(NullLogger<SaveFolderUseCase>.Instance, new FolderQuery(NullLogger<FolderQuery>.Instance));
        var actual = await sut.ExecuteAsync(uow, new() { Folder = expected });
        
        Assert.True(actual.IsSuccess);
        Assert.NotNull(actual.Value);
        Assert.NotEqual(default, actual.Value.Id);
        Assert.Equal(expected.MaxDocumentsFolder, actual.Value.MaxDocumentsFolder);
        Assert.Equal(expected.MaxDocumentsRegister, actual.Value.MaxDocumentsRegister);
        Assert.Equal(expected.DisplayName, actual.Value.DisplayName);
    }

    private void InitTestData()
    {
        Context.Folders.Add(new Folder { Id = GetFolderId(1), Name = "1", MaxDocumentsFolder = 10, MaxDocumentsRegister = 10, DescriptionGroup = "default" });
        Context.SaveChanges();
    }
}