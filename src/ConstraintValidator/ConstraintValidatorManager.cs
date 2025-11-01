using Tolitech.Application.Exceptions.Database;

namespace Tolitech.ConstraintValidator;

/// <summary>
/// Provides static methods for managing and validating database constraint exceptions.
/// </summary>
public static class ConstraintValidatorManager
{
    private static readonly List<IConstraintValidator> Validators = [];
    private static readonly Lock _lock = new();

    /// <summary>
    /// Handles the exception by checking for any database constraint violations.
    /// If a violation is found, returns a custom exception. Otherwise, returns the original exception.
    /// </summary>
    /// <param name="exception">The exception thrown by EF Core.</param>
    /// <returns>
    /// A <see cref="DatabaseConstraintViolationException"/> if a constraint violation is detected,
    /// or the original exception if no constraint violation is found.
    /// </returns>
    public static Exception Handle(Exception exception)
    {
        foreach (var validator in Validators)
        {
            var result = validator.HandleConstraintViolation(exception);

            if (result != null)
            {
                return result;
            }
        }

        return exception;
    }

    /// <summary>
    /// Adds a validator to the collection of registered validators.
    /// </summary>
    /// <param name="validator">The validator implementation.</param>
    public static void AddValidator(IConstraintValidator validator)
    {
        lock (_lock)
        {
            Validators.Add(validator);
        }
    }

    /// <summary>
    /// Removes the specified validator from the collection of active validators.
    /// </summary>
    /// <remarks>This method is thread-safe. It uses a lock to ensure that the removal operation is
    /// atomic.</remarks>
    /// <param name="validator">The validator to be removed. Must not be <see langword="null"/>.</param>
    public static void RemoveValidator(IConstraintValidator validator)
    {
        lock (_lock)
        {
            Validators.Remove(validator);
        }
    }

    /// <summary>
    /// Clears all registered validators from the collection.
    /// </summary>
    /// <remarks>This method removes all validators, effectively resetting the collection to an empty state.
    /// It is thread-safe and can be called concurrently from multiple threads.</remarks>
    public static void ClearValidators()
    {
        lock (_lock)
        {
            Validators.Clear();
        }
    }
}