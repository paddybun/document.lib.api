using document.lib.bl.shared;
using document.lib.bl.shared.Folders;
using document.lib.data.entities;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace document.lib.bl.tests.Folders;

public class DeleteFolderUseCaseTests : UnitTestBase
{
    public DeleteFolderUseCaseTests()
    {
        InitTestData();
    }

    [Fact]
    public async Task ProvidedValidInputs_ExecuteAsync_DeletesAnEmptyFolder()
    {
        var uow = new UnitOfWork(Context);
        var sut = new DeleteFolderUseCase(NullLogger<DeleteFolderUseCase>.Instance, new FolderQuery(NullLogger<FolderQuery>.Instance));
        var result = await sut.ExecuteAsync(uow, new() { FolderId = GetFolderId(1) });
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task ProvidedValidInputs_ExecuteAsync_CreatesNotEmptyWarning()
    {
        var uow = new UnitOfWork(Context);
        var sut = new DeleteFolderUseCase(NullLogger<DeleteFolderUseCase>.Instance, new FolderQuery(NullLogger<FolderQuery>.Instance));
        var result = await sut.ExecuteAsync(uow, new() { FolderId = GetFolderId(2) });
        Assert.False(result.IsSuccess);
        Assert.True(result.HasWarning);
    }

    [Fact]
    public async Task ProvidedInvalidInputs_ExecuteAsync_CreatesNotFoundWarning()
    {
        var uow = new UnitOfWork(Context);
        var sut = new DeleteFolderUseCase(NullLogger<DeleteFolderUseCase>.Instance, new FolderQuery(NullLogger<FolderQuery>.Instance));
        var result = await sut.ExecuteAsync(uow, new() { FolderId = GetFolderId(3) });
        Assert.False(result.IsSuccess);
        Assert.True(result.HasWarning);
    }
    
    private void InitTestData()
    {
        // Folder 1
        Context.Folders.Add(new Folder { Id = GetFolderId(1), Name = "UnitTest", MaxDocumentsRegister = 2, MaxDocumentsFolder = 10, DescriptionGroup = "default" });
        
        // Folder 2 with registers
        Context.Folders.Add(new Folder { Id = GetFolderId(2), Name = "UnitTest", MaxDocumentsRegister = 2, MaxDocumentsFolder = 10, DescriptionGroup = "default" });
        Context.Registers.Add(new Register {Id = 1, FolderId = GetFolderId(2), Name = "1", DisplayName = "1" });
        Context.Documents.Add(new Document { RegisterId = 1, BlobLocation = "", Name = "", PhysicalName = ""});
        
        Context.SaveChanges();
    }

}