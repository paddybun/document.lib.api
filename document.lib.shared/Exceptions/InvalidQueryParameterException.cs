using System.Reflection;

namespace document.lib.shared.Exceptions;

public class InvalidQueryParameterException: Exception
{
    public InvalidQueryParameterException(): 
        this("Invalid query parameters")
    {
    }

    public InvalidQueryParameterException(string message) :
        base(message)
    {
    }

    public InvalidQueryParameterException(MemberInfo queryParamType) :
        this($"Invalid query parameters for: {queryParamType.Name}")
    {
    }
}