using document.lib.bl.contracts.Documents.Commands;
using document.lib.core;
using document.lib.data.entities;
using Microsoft.EntityFrameworkCore;

namespace document.lib.bl.shared.Documents.Commands;

public class UpdateDocumentCommand : IUpdateDocumentCommand<UnitOfWork>
{
    public async Task<Result<int>> ExecuteAsync(UnitOfWork uow, UpdateDocumentCommandParameters parameters)
    {
        var doc = await uow.Connection.Documents
            .Include(x => x.Tags)
            .ThenInclude(x => x.Tag)
            .SingleOrDefaultAsync(x => x.Id == parameters.DocumentId);

        if (doc == null)
            return Result<int>.Warning("Document not found");

        if (string.IsNullOrWhiteSpace(parameters.DisplayName))
            return Result<int>.Warning("Display name is required");

        var category = await uow.Connection.Categories.SingleOrDefaultAsync(x => x.Id == parameters.CategoryId);
        if (category == null)
            return Result<int>.Warning("Category not found");
        if (category.Name == Constants.UncategorizedName)
            return Result<int>.Warning("A real category is required");

        doc.DisplayName = parameters.DisplayName.Trim();
        doc.DateOfDocument = parameters.DateOfDocument;
        doc.Category = category;

        var wantedNames = parameters.TagNames
            .Select(x => x.Trim())
            .Where(x => x.Length > 0)
            .DistinctBy(x => x.ToLowerInvariant())
            .ToList();
        var wantedKeys = wantedNames.Select(x => x.ToLowerInvariant()).ToHashSet();

        foreach (var assignment in doc.Tags.Where(x => !wantedKeys.Contains(x.Tag.Name.ToLowerInvariant())).ToList())
        {
            doc.Tags.Remove(assignment);
            uow.Connection.Remove(assignment);
        }

        var assigned = doc.Tags.Select(x => x.Tag.Name.ToLowerInvariant()).ToHashSet();
        foreach (var name in wantedNames.Where(x => !assigned.Contains(x.ToLowerInvariant())))
        {
            var key = name.ToLowerInvariant();
            var tag = await uow.Connection.Tags.FirstOrDefaultAsync(x => x.Name == key)
                      ?? new Tag { Name = key, DisplayName = name };
            doc.Tags.Add(new TagAssignment { Document = doc, Tag = tag });
        }

        return Result<int>.Success(doc.Id);
    }
}
