using Microsoft.Data.SqlClient;

using Tolitech.Exceptions.Database;

namespace Tolitech.ConstraintValidator.SqlServer;

/// <summary>
/// Validator for SQL Server database constraint violations.
/// This class validates exceptions caused by SQL Server database constraint violations
/// such as primary key, foreign key, and not null constraint violations.
/// </summary>
public class SqlServerConstraintValidator : IConstraintValidator
{
    private static readonly Dictionary<int, Func<SqlException, Exception>> ExceptionMap = new()
    {
        [2627] = ex => new PrimaryKeyViolationException(ex.Message, ex),
        [547] = ex => new ForeignKeyViolationException(ex.Message, ex),
        [515] = ex => new NotNullConstraintViolationException(ex.Message, ex),
    };

    /// <summary>
    /// Validates the provided <see cref="Exception"/> to check for SQL Server constraint violations.
    /// If a violation is found, a <see cref="DatabaseConstraintViolationException"/> is returned.
    /// </summary>
    /// <param name="exception">The <see cref="Exception"/> to validate.</param>
    /// <returns>
    /// A <see cref="DatabaseConstraintViolationException"/> if the exception is caused by a SQL Server constraint violation.
    /// Otherwise, the original exception is returned if no matching constraint violation is detected.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when the <paramref name="exception"/> parameter is null.</exception>
    public Exception HandleConstraintViolation(Exception exception)
    {
        ArgumentNullException.ThrowIfNull(exception);

        if (exception.GetBaseException() is SqlException sqlEx)
        {
            return ExceptionMap.TryGetValue(sqlEx.Number, out var factory)
                ? factory(sqlEx)
                : sqlEx;
        }

        return exception; // Not handled by this validator
    }
}