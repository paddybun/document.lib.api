using document.lib.data.context;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace document.lib.bl.tests;

public class UnitTests
{
    public UnitTests()
    {
        // init inmemory database or mock services here
        // e.g., using Microsoft.EntityFrameworkCore.InMemory;
        
        var options = new DbContextOptionsBuilder<DatabaseContext>()
            .
            .Options;
            
    }
    
    
    [Fact]
    public async Task Test1()
    {
        
    }
}