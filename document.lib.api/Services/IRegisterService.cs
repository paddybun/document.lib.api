using System;
using System.Threading.Tasks;
using document.lib.api.Models;

namespace document.lib.api.Services
{
    public interface IRegisterService
    {
        Task<Register> UpdateRegisterAsync(Guid registerId, string displayName, bool? isActive);
    }
}