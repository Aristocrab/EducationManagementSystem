namespace EducationManagementSystem.Core.Exceptions;

public class WrongPasswordException : Exception
{
    public WrongPasswordException(string message) : base(message) { }
}