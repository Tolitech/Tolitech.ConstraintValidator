using System.Reflection;

using Npgsql;

namespace Tolitech.ConstraintValidator.PostgreSql.UnitTests;

/// <summary>
/// Factory to simulate <see cref="PostgresException"/> in unit tests.
/// </summary>
public static class PostgresExceptionFactory
{
    /// <summary>
    /// Creates a new instance of the <see cref="PostgresException"/> class with the specified SQL state and optional
    /// message.
    /// </summary>
    /// <param name="sqlState">The SQL state code associated with the exception, such as "23505".</param>
    /// <param name="message">An optional message that describes the error. If not provided, a default message is used.</param>
    /// <returns>A <see cref="PostgresException"/> initialized with the specified SQL state and message.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the necessary constructor for creating the internal error message or the <see
    /// cref="PostgresException"/> itself cannot be found.</exception>
    public static PostgresException Create(string sqlState, string? message = null)
    {
        var npgsqlAssembly = typeof(PostgresException).Assembly;

        // Find the internal type 'ErrorOrNoticeMessage'
        var errorMessageType = npgsqlAssembly.GetType("Npgsql.BackendMessages.ErrorOrNoticeMessage", throwOnError: true)!;

        // Find the constructor for 'ErrorOrNoticeMessage' with the correct parameters
        var errorMessageCtor = errorMessageType.GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic)
            .FirstOrDefault(c => c.GetParameters().Length == 18)
            ?? throw new InvalidOperationException("Could not find a constructor for ErrorOrNoticeMessage.");

        // Build the arguments for ErrorOrNoticeMessage
        object[] errorMessageArgs =
        [
            "ERROR",                                 // severity
            "ERROR",                                 // invariantSeverity
            sqlState,                                // sqlState
            message ?? "Simulated Postgres error",   // messageText
            string.Empty, string.Empty,              // detail, hint
            0, 0,                                     // position, internalPosition
            string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty // internalQuery, where, schemaName, tableName, columnName, dataTypeName, constraintName, file, line, routine
        ];

        // Adjust the number of parameters in case the version changes
        object[] paddedArgs = new object[errorMessageCtor.GetParameters().Length];
        for (int i = 0; i < paddedArgs.Length; i++)
        {
            paddedArgs[i] = i < errorMessageArgs.Length ? errorMessageArgs[i] : GetDefault(errorMessageCtor.GetParameters()[i].ParameterType)!;
        }

        object errorMessage = errorMessageCtor.Invoke(paddedArgs);

        // Now instantiate the PostgresException with this ErrorOrNoticeMessage
        var pgExceptionCtor = typeof(PostgresException)
            .GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic)
            .FirstOrDefault(c =>
            {
                var p = c.GetParameters();
                return p.Length == 1 && p[0].ParameterType == errorMessageType;
            }) ?? throw new InvalidOperationException("Could not find PostgresException constructor.");

        return (PostgresException)pgExceptionCtor.Invoke([errorMessage]);
    }

    private static object? GetDefault(Type type)
    {
        return type.IsValueType ? Activator.CreateInstance(type) : null;
    }
}