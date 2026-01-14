using System.Diagnostics.CodeAnalysis;

namespace AgtcSrvManagement.Application.Exceptions;

[ExcludeFromCodeCoverage]
public class UnexpectedException : HttpException
{
    public UnexpectedException(Exception ex)
     : base(500, "Internal Server Error", ex) { }
}
