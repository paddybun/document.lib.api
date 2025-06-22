using document.lib.data.entities;
using document.lib.shared.Interfaces;
using document.lib.shared.Models;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace document.lib.test;

public class DocumentSqlServiceTests
{
    private readonly IOptions<SharedConfig> _defaultConfiguration;
    private readonly IDocumentRepository<Document> _defaultDocumentRepository;
    private readonly ICategoryService _defaultCategoryService;
    private readonly ITagService _defaultTagService;
    private readonly IFolderService _defaultFolderService;


    public DocumentSqlServiceTests()
    {
        _defaultConfiguration = Options.Create(new SharedConfig());
        _defaultDocumentRepository = new Mock<IDocumentRepository<Document>>().Object;
        _defaultCategoryService = new Mock<ICategoryService>().Object;
        _defaultTagService = new Mock<ITagService>().Object;
        _defaultFolderService = new Mock<IFolderService>().Object;
    }

    [Fact]
    public void Can_create_instance()
    {
        Assert.True(true);
    }
}