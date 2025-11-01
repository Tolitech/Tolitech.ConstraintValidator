namespace Tolitech.ConstraintValidator.UnitTests.Implementations;

/// <summary>
/// Represents errors that occur during application execution and are specific to the DummyException type.
/// </summary>
/// <remarks>This exception is intended to be used for scenarios where a specific error condition related to the
/// DummyException type needs to be communicated. It provides constructors for initializing the exception with an error
/// message and/or an inner exception, enabling detailed error reporting.</remarks>
public class DummyException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DummyException"/> class.
    /// </summary>
    public DummyException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DummyException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public DummyException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DummyException"/> class with a specified error message and a
    /// reference to the inner exception that caused this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception, or <see langword="null"/> if no inner exception is
    /// specified.</param>
    public DummyException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}