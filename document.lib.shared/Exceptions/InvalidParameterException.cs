using System.Reflection;

namespace document.lib.shared.Exceptions;

public class InvalidParameterException: Exception
{
    public InvalidParameterException(): 
        this("Invalid query parameters")
    {
    }

    public InvalidParameterException(string message) :
        base(message)
    {
    }

    public InvalidParameterException(MemberInfo queryParamType) :
        this($"Invalid query parameters for: {queryParamType.Name}")
    {
    }
}