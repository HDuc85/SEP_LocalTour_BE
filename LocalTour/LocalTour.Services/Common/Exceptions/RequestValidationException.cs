using Microsoft.IdentityModel.Tokens;

namespace Service.Common.Exceptions;

public class RequestValidationException : Exception
{
    public List<ValidationFailure>? Errors { get; init; }

    public RequestValidationException(List<ValidationFailure>? errors) : base("User input validation failed!")
    {
        Errors = errors;
    }
}