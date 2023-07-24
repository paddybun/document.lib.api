using System.Reflection;

namespace document.lib.shared.Exceptions;

public class InvalidQueryParameterException: Exception
{
    public InvalidQueryParameterException(): 
        base("Invalid query parameters")
    {
    }

    public InvalidQueryParameterException(MemberInfo queryParamType) :
        base($"Invalid query parameters for: {queryParamType.Name}")
    {
    }
}