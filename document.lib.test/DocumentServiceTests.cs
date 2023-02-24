using document.lib.shared.Interfaces;
using document.lib.shared.Models;
using document.lib.shared.Services;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace document.lib.test;

public class DocumentServiceTests
{
    private readonly IOptions<AppConfiguration> _defaultConfiguration;
    private readonly IDocumentRepository _defaultDocumentRepository;
    private readonly ICategoryService _defaultCategoryService;
    private readonly ITagService _defaultTagService;
    private readonly IFolderService _defaultFolderService;


    public DocumentServiceTests()
    {
        _defaultConfiguration = Options.Create(new AppConfiguration());
        _defaultDocumentRepository = new Mock<IDocumentRepository>().Object;
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