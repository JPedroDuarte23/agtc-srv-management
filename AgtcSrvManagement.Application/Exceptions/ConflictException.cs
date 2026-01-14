using System.Diagnostics.CodeAnalysis;

namespace AgtcSrvManagement.Application.Exceptions;

[ExcludeFromCodeCoverage]
public class ConflictException : HttpException
{
    public ConflictException(string message)
     : base(409, "Conflict", message) { }
}
