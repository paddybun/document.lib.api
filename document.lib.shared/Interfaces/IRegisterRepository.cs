using document.lib.shared.Models.Models;

namespace document.lib.shared.Interfaces;

public interface IRegisterRepository
{
    Task<RegisterModel> GetRegistersAsync();
    Task<RegisterModel> CreateRegistersAsync(RegisterModel model);
}