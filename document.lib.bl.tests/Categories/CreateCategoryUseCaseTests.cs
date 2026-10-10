using document.lib.bl.shared;
using document.lib.bl.shared.Categories.Commands;
using document.lib.bl.shared.Categories.UseCases;
using document.lib.data.entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace document.lib.bl.tests.Categories;

public class CreateCategoryUseCaseTests : UnitTestBase
{
    [Fact]
    public async Task CreatesCategoryWithTrimmedDisplayNameAndPersistedId()
    {
        var result = await Create("  Insurance  ");

        Assert.True(result.IsSuccess);
        Assert.True(result.Value > 0);
        var category = await Context.Categories.AsNoTracking().SingleAsync(x => x.Id == result.Value);
        Assert.Equal("Insurance", category.DisplayName);
        Assert.False(string.IsNullOrWhiteSpace(category.Name));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(" UnCategorized ")]
    public async Task RejectsInvalidDisplayNames(string displayName)
    {
        var result = await Create(displayName);

        Assert.True(result.HasWarning);
        Assert.Empty(await Context.Categories.ToListAsync());
    }

    [Fact]
    public async Task RejectsOverlongDisplayName()
    {
        var result = await Create(new string('a', 501));
        Assert.True(result.HasWarning);
        Assert.Empty(await Context.Categories.ToListAsync());
    }

    [Fact]
    public async Task RejectsDuplicateDisplayNameIgnoringCaseAndWhitespace()
    {
        Context.Categories.Add(new Category { Name = "insurance", DisplayName = "Insurance" });
        await Context.SaveChangesAsync();

        var result = await Create(" INSURANCE ");

        Assert.True(result.HasWarning);
        Assert.Single(await Context.Categories.ToListAsync());
    }

    private Task<document.lib.core.Result<int>> Create(string displayName)
    {
        var useCase = new CreateCategoryUseCase(NullLogger<CreateCategoryUseCase>.Instance, new CreateCategoryCommand());
        return useCase.ExecuteAsync(new UnitOfWork(Context), new(displayName));
    }
}
