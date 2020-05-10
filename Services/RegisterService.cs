using System;
using System.Threading.Tasks;
using document.lib.api.Models;
using Microsoft.EntityFrameworkCore;

namespace document.lib.api.Services
{
    public interface IRegisterService
    {
        Task<Register> UpdateRegisterAsync(Guid registerId, string displayName, bool? isActive);
    }

    public class RegisterService : IRegisterService
    {
        private readonly DocumentlibContext _dbContext;

        public RegisterService(DocumentlibContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Register> UpdateRegisterAsync(Guid registerId, string displayName, bool? isActive)
        {
            var reg = await _dbContext
                .Registers
                .Include(x => x.Documents)
                .SingleOrDefaultAsync(x => x.Id == registerId);
            var registers = (await _dbContext
                .Folders
                .Include(x => x.Registers)
                .SingleOrDefaultAsync(x => x.Id == reg.FolderId)).Registers;

            // Display name
            if (!string.IsNullOrEmpty(displayName))
            {
                reg.DisplayName = displayName;
            }

            // Active state
            if (isActive != null)
            {
                if (isActive.Value)
                {
                    foreach (var register in registers)
                    {
                        register.IsActive = false;
                    }
                }

                reg.IsActive = isActive.Value;
            }

            _dbContext.UpdateRange(registers);
            await _dbContext.SaveChangesAsync();
            return reg;
        }
    }
}