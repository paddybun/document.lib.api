﻿using document.lib.ef;
using document.lib.ef.Entities;
using document.lib.shared.Exceptions;
using document.lib.shared.Interfaces;
using document.lib.shared.Models.QueryDtos;
using document.lib.shared.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace document.lib.shared.Repositories.Sql;

// Scoped injection 
public sealed class TagSqlRepository(DocumentLibContext context) : ITagRepository
{
    public async Task<TagModel?> GetTagAsync(TagQueryParameters queryParameters)
    {
        if (queryParameters == null) throw new ArgumentNullException(nameof(queryParameters));
        if (!queryParameters.IsValid()) throw new InvalidQueryParameterException(queryParameters.GetType());

        EfTag? efTag;
        if (queryParameters.Id.HasValue)
        {
            efTag = await context.Tags
                .SingleOrDefaultAsync(x => x.Id == queryParameters.Id.Value);    
        }
        else
        {
            efTag = await context.Tags
                .SingleOrDefaultAsync(x => x.Name == queryParameters.Name);
        }

        return efTag == null ? null : Map(efTag);
    }

    public async Task<List<TagModel>> GetTagsAsync()
    {
        var tags = await context.Tags.ToListAsync();
        return tags.Select(Map).ToList();
    }

    public async Task<TagModel> CreateTagAsync(TagModel tagModel)
    {
        var tag = new EfTag {Name = tagModel.Name, DisplayName = tagModel.DisplayName};
        await context.AddAsync(tag);
        await context.SaveChangesAsync();
        return Map(tag);
    }

    private static TagModel Map(EfTag efTag)
    {
        return new TagModel
        {
            Id = efTag.Id.ToString(),
            Name = efTag.Name,
            DisplayName = efTag.DisplayName
        };
    }
}