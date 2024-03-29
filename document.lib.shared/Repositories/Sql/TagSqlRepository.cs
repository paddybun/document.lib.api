﻿using document.lib.ef;
using document.lib.ef.Entities;
using document.lib.shared.Exceptions;
using document.lib.shared.Interfaces;
using document.lib.shared.Models.QueryDtos;
using document.lib.shared.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace document.lib.shared.Repositories.Sql;

// Scoped injection 
public class TagSqlRepository : ITagRepository
{
    private readonly DocumentLibContext _context;

    public TagSqlRepository(DocumentLibContext context)
    {
        _context = context;
    }

    public async Task<TagModel> GetTagAsync(TagQueryParameters queryParameters)
    {
        if (queryParameters == null) throw new ArgumentNullException(nameof(queryParameters));
        if (!queryParameters.IsValid()) throw new InvalidQueryParameterException(queryParameters.GetType());

        EfTag efTag;
        if (queryParameters.Id.HasValue)
        {
            efTag = await _context.Tags
                .SingleOrDefaultAsync(x => x.Id == queryParameters.Id.Value);    
        }
        else
        {
            efTag = await _context.Tags
                .SingleOrDefaultAsync(x => x.Name == queryParameters.Name);
        }
        return Map(efTag);
    }

    public async Task<List<TagModel>> GetTagsAsync()
    {
        var tags = await _context.Tags.ToListAsync();
        return tags.Select(Map).ToList();
    }

    public async Task<TagModel> CreateTagAsync(string tagName)
    {
        var tag = new EfTag {Name = tagName, DisplayName = tagName};
        await _context.AddAsync(tag);
        await _context.SaveChangesAsync();
        return Map(tag);
    }

    private TagModel Map(EfTag efTag)
    {
        if (efTag == null) return null;
        return new TagModel
        {
            Id = efTag.Id.ToString(),
            Name = efTag.Name,
            DisplayName = efTag.DisplayName
        };
    }
}