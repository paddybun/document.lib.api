using document.lib.ef.Entities;

namespace document.lib.shared.Cqrs.Interfaces;

public interface ICategoryQuery
{
    Task<EfCategory?> ExecuteAsync(string categoryName);
}