using System.Reflection;
using System.Runtime.Serialization;

using Microsoft.Data.SqlClient;

namespace Tolitech.ConstraintValidator.SqlServer.UnitTests;

/// <summary>
/// Provides a factory for creating instances of <see cref="SqlException"/> for testing purposes.
/// </summary>
/// <remarks>This class creates a minimal, but valid, SqlException without any inner exception,
/// so GetBaseException() returns the SqlException itself.</remarks>
public static class SqlExceptionFactory
{
    /// <summary>
    /// Creates a new instance of <see cref="SqlException"/> with a specified error number and an optional message.
    /// </summary>
    /// <param name="errorNumber">The error number associated with the SQL exception.</param>
    /// <param name="message">An optional message describing the error. If not provided, a default message is used.</param>
    /// <returns>A <see cref="SqlException"/> instance representing the specified SQL error.</returns>
    /// <exception cref="InvalidOperationException">Thrown when a compatible constructor for <see cref="SqlError"/> cannot be found.</exception>
    public static SqlException Create(int errorNumber, string? message = null)
    {
        // Build SqlError dynamically (constructor is non-public and differs across versions)
        var sqlErrorType = typeof(SqlError);
        var errorCtor = sqlErrorType
            .GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic)
            .FirstOrDefault(c =>
            {
                var p = c.GetParameters();
                return p.Length >= 9 && p[0].ParameterType == typeof(int);
            }) ?? throw new InvalidOperationException("Could not find a compatible constructor for SqlError.");

        var ctorParams = errorCtor.GetParameters();
        object?[] errorArgs = new object?[ctorParams.Length];

        for (int i = 0; i < ctorParams.Length; i++)
        {
            var t = ctorParams[i].ParameterType;

            if (t == typeof(int))
            {
                errorArgs[i] = (i == 0) ? errorNumber : 1;
            }
            else if (t == typeof(byte))
            {
                errorArgs[i] = (byte)1;
            }
            else if (t == typeof(string))
            {
                errorArgs[i] = message ?? "Fake SQL exception for testing.";
            }
            else if (t == typeof(uint))
            {
                errorArgs[i] = 0u;
            }
            else
            {
                errorArgs[i] = null!;
            }
        }

        object sqlError = errorCtor.Invoke(errorArgs);

        // Create SqlErrorCollection and add the error
        SqlErrorCollection errorCollection = (SqlErrorCollection)Activator.CreateInstance(typeof(SqlErrorCollection), nonPublic: true)!;
        typeof(SqlErrorCollection)
            .GetMethod("Add", BindingFlags.Instance | BindingFlags.NonPublic)!
            .Invoke(errorCollection, [sqlError]);

        // Create an uninitialized SqlException (no constructor invoked, no inner exception)
#pragma warning disable SYSLIB0050 // Type or member is obsolete
        SqlException ex = (SqlException)FormatterServices.GetUninitializedObject(typeof(SqlException));
#pragma warning restore SYSLIB0050 // Type or member is obsolete

        // Set the private field that stores the errors
        var errorsField = typeof(SqlException)
            .GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
            .FirstOrDefault(f => f.FieldType == typeof(SqlErrorCollection))
            ?? throw new InvalidOperationException("Could not find SqlException error collection field.");

        errorsField.SetValue(ex, errorCollection);

        // Set HResult to the standard SqlException HRESULT (-2146232060 / 0x80131904)
        var hresultField = typeof(Exception).GetField("_HResult", BindingFlags.Instance | BindingFlags.NonPublic);
        hresultField?.SetValue(ex, unchecked((int)0x80131904));

        // Optionally set a friendly base message if supported by the current runtime layout
        // (SqlException.Message often formats from Errors; setting this is not strictly required)
        var messageField = typeof(Exception).GetField("_message", BindingFlags.Instance | BindingFlags.NonPublic);
        messageField?.SetValue(ex, message ?? "Fake SQL exception for testing.");

        return ex;
    }
}