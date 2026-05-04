namespace UserService.Application.Exceptions;

public class ValidationException(string message) : Exception(message);
