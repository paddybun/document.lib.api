using document.lib.bl.shared;
using document.lib.bl.shared.RegisterDescriptions;
using document.lib.data.entities;
using document.lib.data.models.RegisterDescriptions;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace document.lib.bl.tests.RegisterDescriptions;

public class RegisterDescriptionSaveUseCaseTests: UnitTestBase
{
    public RegisterDescriptionSaveUseCaseTests()
    {
        InitTestData();
    }

    [Fact]
    public async Task ProvidedValidInputs_ExecuteAsync_SavesRegisterDescription()
    {
        var uow = new UnitOfWork(Context);
        var registerDescriptionQuery = new RegisterDescriptionQuery(NullLogger<RegisterDescriptionQuery>.Instance);
        var addCommand = new RegisterDescriptionAddCommand(NullLogger<RegisterDescriptionAddCommand>.Instance);
        var renameCommand =
            new RegisterDescriptionRenameGroupCommand(NullLogger<RegisterDescriptionRenameGroupCommand>.Instance);
        var updateCommand = new RegisterDescriptionUpdateCommand(NullLogger<RegisterDescriptionUpdateCommand>.Instance);
        var sut = new RegisterDescriptionSaveUseCase(NullLogger<RegisterDescriptionSaveUseCase>.Instance,
            registerDescriptionQuery, addCommand, renameCommand, updateCommand);

        var saveModel = new RegisterDescriptionSaveModel
        {
            GroupName = "default",
            Entries = new List<RegisterDescriptionEntryModel>
            {
                new() { Id = GetRegisterId(1), DisplayName = "Foo", Order = 2 },
                new() { Id = GetRegisterId(2), DisplayName = "Bar", Order = 1 }
            }
        };

        var actual = await sut.ExecuteAsync(uow, new() { SaveModel = saveModel });
        
        Assert.True(actual.IsSuccess);
        Assert.NotNull(actual.Value);
    }

    private void InitTestData()
    {
        // Global data
        Context.RegisterDescriptions.Add(new RegisterDescription { Id = GetRegisterId(1), Group = "default", Name = "1", DisplayName = "1", Order = 1 });
        Context.RegisterDescriptions.Add(new RegisterDescription { Id = GetRegisterId(2), Group = "default", Name = "2", DisplayName = "2", Order = 2 });
        
        // Folder 1
        Context.Folders.Add(new Folder { Id = GetFolderId(1), Name = "UnitTest", MaxDocumentsRegister = 2, MaxDocumentsFolder = 10, DescriptionGroup = "default" });
        Context.Registers.Add(new (){ Name = "1", DocumentCount = 2, FolderId = GetFolderId(1), DescriptionId = 1 });
        Context.Registers.Add(new (){ Name = "2", DisplayName = "anyExistingString", DocumentCount = 1, FolderId = GetFolderId(1), DescriptionId = 2 });

        Context.SaveChanges();
    }
}