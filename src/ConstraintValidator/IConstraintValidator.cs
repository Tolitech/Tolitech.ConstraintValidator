using Tolitech.Exceptions.Database;

namespace Tolitech.ConstraintValidator;

/// <summary>
/// Interface for validating constraint violations in database operations.
/// Implementations of this interface are responsible for checking
/// if a given <see cref="Exception"/> is caused by a database constraint violation
/// and returning an appropriate <see cref="DatabaseConstraintViolationException"/>.
/// </summary>
public interface IConstraintValidator
{
    /// <summary>
    /// Handles a constraint violation exception and returns a processed exception.
    /// </summary>
    /// <param name="exception">The exception to be processed. Must not be <see langword="null"/>.</param>
    /// <returns>An <see cref="Exception"/> that represents the processed result of the constraint violation.</returns>
    Exception HandleConstraintViolation(Exception exception);
}