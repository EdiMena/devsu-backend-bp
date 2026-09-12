namespace Domain.Exceptions;

public abstract class AppException(string message) : Exception(message)
{
    public abstract int StatusCode { get; }
}

public sealed class NotFoundException(string message) : AppException(message)
{
    public override int StatusCode => 404;
}

public sealed class ValidationException(string message) : AppException(message)
{
    public override int StatusCode => 400;
}