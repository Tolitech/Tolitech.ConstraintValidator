using System.Reflection;

using Microsoft.Data.SqlClient;

namespace Tolitech.ConstraintValidator.SqlServer.UnitTests;

/// <summary>
/// Provides a factory for creating instances of <see cref="SqlException"/> for testing purposes.
/// </summary>
/// <remarks>This class is designed to facilitate the creation of <see cref="SqlException"/> objects with
/// specified error numbers and optional messages. It is particularly useful in unit testing scenarios where simulating
/// SQL exceptions is necessary.</remarks>
public static class SqlExceptionFactory
{
    /// <summary>
    /// Creates a new instance of <see cref="SqlException"/> with a specified error number and an optional message.
    /// </summary>
    /// <remarks>This method is intended for testing purposes, allowing the creation of a <see
    /// cref="SqlException"/> with a specific error number and message. The method dynamically constructs the necessary
    /// internal objects to simulate a real SQL exception.</remarks>
    /// <param name="errorNumber">The error number associated with the SQL exception.</param>
    /// <param name="message">An optional message describing the error. If not provided, a default message is used.</param>
    /// <returns>A <see cref="SqlException"/> instance representing the specified SQL error.</returns>
    /// <exception cref="InvalidOperationException">Thrown if a compatible constructor for <see cref="SqlError"/> cannot be found.</exception>
    public static SqlException Create(int errorNumber, string? message = null)
    {
        // Find the SqlError type
        var sqlErrorType = typeof(SqlError);

        // Find the most suitable constructor, based on the minimum expected parameter count (== 9)
        var constructor = sqlErrorType
            .GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic)
            .FirstOrDefault(c =>
            {
                var parameters = c.GetParameters();
                return parameters.Length == 9 &&
                       parameters[0].ParameterType == typeof(int); // error number
            }) ?? throw new InvalidOperationException("Could not find a compatible constructor for SqlError.");

        // Dynamically create parameters based on the found constructor signature
        var ctorParams = constructor.GetParameters();
        object?[] args = new object?[ctorParams.Length];

        for (int i = 0; i < ctorParams.Length; i++)
        {
            var type = ctorParams[i].ParameterType;

            if (type == typeof(int))
            {
                args[i] = (i == 0) ? errorNumber : 1;
            }
            else if (type == typeof(byte))
            {
                args[i] = (byte)1;
            }
            else if (type == typeof(string))
            {
                args[i] = message ?? "Fake SQL exception for testing.";
            }
            else if (type == typeof(uint))
            {
                args[i] = 0u;
            }
            else
            {
                args[i] = null!;
            }
        }

        object sqlError = constructor.Invoke(args);

        // Create SqlErrorCollection and add the error
        SqlErrorCollection errorCollection = (SqlErrorCollection)Activator
            .CreateInstance(typeof(SqlErrorCollection), nonPublic: true)!;

        typeof(SqlErrorCollection)
            .GetMethod("Add", BindingFlags.Instance | BindingFlags.NonPublic)!
            .Invoke(errorCollection, [sqlError]);

        // Fix ambiguity: select the exact method with the correct parameters
        var createException = typeof(SqlException)
            .GetMethods(BindingFlags.Static | BindingFlags.NonPublic)
            .First(m =>
            {
                var parameters = m.GetParameters();
                return m.Name == "CreateException"
                       && parameters.Length == 2
                       && parameters[0].ParameterType == typeof(SqlErrorCollection)
                       && parameters[1].ParameterType == typeof(string);
            });

        return (SqlException)createException.Invoke(
            null,
            [
                errorCollection,
                "11.0.0",
            ])!;
    }
}