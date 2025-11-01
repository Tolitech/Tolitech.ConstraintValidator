using Npgsql;

using Tolitech.Exceptions.Database;

namespace Tolitech.ConstraintValidator.PostgreSql;

/// <summary>
/// Validator for PostgreSQL database constraint violations.
/// This class validates exceptions caused by PostgreSQL database constraint violations
/// such as primary key, foreign key, and not null constraint violations.
/// </summary>
public class PostgreSqlConstraintValidator : IConstraintValidator
{
    private static readonly Dictionary<string, Func<PostgresException, Exception>> ExceptionMap = new()
    {
        ["23505"] = ex => new PrimaryKeyViolationException(ex.Message, ex),
        ["23503"] = ex => new ForeignKeyViolationException(ex.Message, ex),
        ["23514"] = ex => new CheckConstraintViolationException(ex.Message, ex),
        ["23502"] = ex => new NotNullConstraintViolationException(ex.Message, ex),
    };

    /// <summary>
    /// Validates the provided <see cref="Exception"/> to check for PostgreSQL constraint violations.
    /// If a violation is found, a <see cref="DatabaseConstraintViolationException"/> is returned.
    /// </summary>
    /// <param name="exception">The <see cref="Exception"/> to validate.</param>
    /// <returns>
    /// A <see cref="DatabaseConstraintViolationException"/> if the exception is caused by a PostgreSQL constraint violation.
    /// Otherwise, the original exception is returned if no matching constraint violation is detected.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when the <paramref name="exception"/> parameter is null.</exception>
    public Exception HandleConstraintViolation(Exception exception)
    {
        ArgumentNullException.ThrowIfNull(exception);

        if (exception.GetBaseException() is PostgresException pgEx)
        {
            return ExceptionMap.TryGetValue(pgEx.SqlState, out var factory)
                ? factory(pgEx)
                : pgEx;
        }

        return exception; // Not handled by this validator
    }
}