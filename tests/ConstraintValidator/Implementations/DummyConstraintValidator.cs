namespace Tolitech.ConstraintValidator.UnitTests.Implementations;

/// <summary>
/// Provides validation for constraints by handling specific exceptions.
/// </summary>
/// <remarks>This validator processes exceptions to determine if they are of type <see cref="DummyException"/>. If
/// the exception matches, it wraps the original exception in a new <see cref="DummyException"/>  with the same message
/// and inner exception. Otherwise, the original exception is returned unmodified.</remarks>
public class DummyConstraintValidator : IConstraintValidator
{
    /// <summary>
    /// Handles constraint violation exceptions by transforming specific exception types.
    /// </summary>
    /// <param name="exception">The exception to be processed. Cannot be <see langword="null"/>.</param>
    /// <returns>A transformed exception if the input exception is of type <see cref="DummyException"/>; otherwise, the original
    /// exception.</returns>
    public Exception HandleConstraintViolation(Exception exception)
    {
        ArgumentNullException.ThrowIfNull(exception);

        if (exception.GetBaseException() is ApplicationException ex)
        {
            return new DummyException(ex.Message, ex);
        }

        return exception; // Not handled by this validator
    }
}