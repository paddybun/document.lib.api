using document.lib.shared.Helper;
using Xunit;

namespace document.lib.test;

public class PropertyCheckerTests
{
    [Fact]
    public void Any_OneValue_ReturnsTrue()
    {
        object toTest = new { };
        var mockObject = new { toTest };
        
        var actual = PropertyValidator.Any(mockObject, x => x.toTest);
        var expected = true;
        Assert.Equal(expected, actual);
    }
    
    [Fact]
    public void Any_OneNullable_ReturnsFalse()
    {
        object? toTest = default;
        var mockObject = new { toTest };
        
        var actual = PropertyValidator.Any(mockObject, x => x.toTest);
        var expected = false;
        Assert.Equal(expected, actual);
    }
    
    [Fact]
    public void Any_MultipleNullable_ReturnsFalse()
    {
        object? toTest = null;
        object? toTest2 = null;
        var mockObject = new { toTest, toTest2 };
        
        var actual = PropertyValidator.Any(mockObject, x => x.toTest, x => x.toTest2);
        var expected = false;
        Assert.Equal(expected, actual);
    }
    
    [Fact]
    public void Any_MultipleMixedValue_ReturnsTrue()
    {
        object toTest = new { };
        object? toTest2 = null;
        var mockObject = new { toTest, toTest2 };
        
        var actual = PropertyValidator.Any(mockObject, x => x.toTest, x => x.toTest2);
        var expected = true;
        Assert.Equal(expected, actual);
    }
    
    [Fact]
    public void Any_StringValue_ReturnsTrue()
    {
        string toTest = "test";
        var mockObject = new { toTest };
        
        var actual = PropertyValidator.Any(mockObject, x => x.toTest);
        var expected = true;
        Assert.Equal(expected, actual);
    }
    
    [Fact]
    public void Any_EmptyString_ReturnsFalse()
    {
        string toTest = "";
        var mockObject = new { toTest };
        
        var actual = PropertyValidator.Any(mockObject, x => x.toTest);
        var expected = false;
        Assert.Equal(expected, actual);
    }
    
    [Fact]
    public void Any_EmptyStringAndValue_ReturnsTrue()
    {
        string toTest = "";
        object toTest2 = new { };
        var mockObject = new { toTest, toTest2 };
        
        var actual = PropertyValidator.Any(mockObject, x => x.toTest, x => x.toTest2);
        var expected = true;
        Assert.Equal(expected, actual);
    }
    
    [Fact]
    public void All_OneValue_ReturnsTrue()
    {
        object toTest = new { };
        var mockObject = new { toTest };
        
        var actual = PropertyValidator.All(mockObject, x => x.toTest);
        var expected = true;
        Assert.Equal(expected, actual);
    }
    
    [Fact]
    public void All_OneNullable_ReturnsFalse()
    {
        object? toTest = default;
        var mockObject = new { toTest };
        
        var actual = PropertyValidator.All(mockObject, x => x.toTest);
        var expected = false;
        Assert.Equal(expected, actual);
    }
    
    [Fact]
    public void All_MultipleNullable_ReturnsFalse()
    {
        object? toTest = null;
        object? toTest2 = null;
        var mockObject = new { toTest, toTest2 };
        
        var actual = PropertyValidator.All(mockObject, x => x.toTest, x => x.toTest2);
        var expected = false;
        Assert.Equal(expected, actual);
    }
    
    [Fact]
    public void All_MultipleMixedValue_ReturnsFalse()
    {
        object toTest = new { };
        object? toTest2 = null;
        var mockObject = new { toTest, toTest2 };
        
        var actual = PropertyValidator.All(mockObject, x => x.toTest, x => x.toTest2);
        var expected = false;
        Assert.Equal(expected, actual);
    }
    
    [Fact]
    public void All_MultipleValue_ReturnsTrue()
    {
        object toTest = new { };
        object? toTest2 = new { };
        var mockObject = new { toTest, toTest2 };
        
        var actual = PropertyValidator.All(mockObject, x => x.toTest, x => x.toTest2);
        var expected = true;
        Assert.Equal(expected, actual);
    }
    
    [Fact]
    public void All_StringValue_ReturnsTrue()
    {
        string toTest = "test";
        var mockObject = new { toTest };
        
        var actual = PropertyValidator.All(mockObject, x => x.toTest);
        var expected = true;
        Assert.Equal(expected, actual);
    }
    
    [Fact]
    public void All_EmptyString_ReturnsFalse()
    {
        string toTest = "";
        var mockObject = new { toTest };
        
        var actual = PropertyValidator.All(mockObject, x => x.toTest);
        var expected = false;
        Assert.Equal(expected, actual);
    }
    
    [Fact]
    public void All_EmptyStringAndValue_ReturnsFalse()
    {
        string toTest = "";
        object toTest2 = new { };
        var mockObject = new { toTest, toTest2 };
        
        var actual = PropertyValidator.All(mockObject, x => x.toTest, x => x.toTest2);
        var expected = false;
        Assert.Equal(expected, actual);
    }
    
    [Fact]
    public void All_EmptyStringAndValueAndNullable_ReturnsFalse()
    {
        string toTest = "";
        object toTest2 = new { };
        object? toTest3 = null;
        var mockObject = new { toTest, toTest2, toTest3 };
        
        var actual = PropertyValidator.All(mockObject, x => x.toTest, x => x.toTest2, x => x.toTest3);
        var expected = false;
        Assert.Equal(expected, actual);
    }
}