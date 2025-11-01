using Microsoft.Data.SqlClient;

using Tolitech.Application.Exceptions.Database;

namespace Tolitech.ConstraintValidator.SqlServer.UnitTests;

/// <summary>
/// Contains unit tests for the <see cref="SqlServerConstraintValidator"/> class, specifically testing its ability to
/// handle SQL constraint violations and return appropriate exceptions.
/// </summary>
/// <remarks>This test class verifies the behavior of the <see cref="SqlServerConstraintValidator"/> when handling
/// various SQL Server constraint violations, such as primary key, foreign key, and not-null constraint violations. It
/// ensures that the correct exception types are returned for known SQL error codes and that the original exception is
/// returned for unknown or non-SQL exceptions.</remarks>
public class SqlServerConstraintValidatorTests
{
    /// <summary>
    /// Tests that the <see cref="SqlServerConstraintValidator.HandleConstraintViolation"/> method returns a <see
    /// cref="PrimaryKeyViolationException"/> when a primary key violation occurs.
    /// </summary>
    /// <remarks>This test verifies that the <see cref="SqlServerConstraintValidator"/> correctly identifies a
    /// SQL Server error with code 2627 as a primary key violation and returns the appropriate exception.</remarks>
    [Fact]
    public void HandleConstraintViolation_PrimaryKeyViolation_ReturnsPrimaryKeyViolationException()
    {
        // Arrange
        SqlServerConstraintValidator validator = new();
        var sqlEx = SqlExceptionFactory.Create(2627, "Primary key violation");

        // Act
        var result = validator.HandleConstraintViolation(sqlEx);

        // Assert
        Assert.IsType<PrimaryKeyViolationException>(result);
    }

    /// <summary>
    /// Tests that the <see cref="SqlServerConstraintValidator.HandleConstraintViolation"/> method returns a <see
    /// cref="ForeignKeyViolationException"/> when a foreign key violation occurs.
    /// </summary>
    /// <remarks>This test verifies that the <see cref="SqlServerConstraintValidator"/> correctly identifies a
    /// SQL Server error with code 547 as a foreign key violation and returns the appropriate exception.</remarks>
    [Fact]
    public void HandleConstraintViolation_ForeignKeyViolation_ReturnsForeignKeyViolationException()
    {
        // Arrange
        SqlServerConstraintValidator validator = new();
        var sqlEx = SqlExceptionFactory.Create(547, "Foreign key violation");

        // Act
        var result = validator.HandleConstraintViolation(sqlEx);

        // Assert
        Assert.IsType<ForeignKeyViolationException>(result);
    }

    /// <summary>
    /// Tests that the <see cref="SqlServerConstraintValidator.HandleConstraintViolation"/> method returns a <see
    /// cref="NotNullConstraintViolationException"/> when a SQL exception with a not-null constraint violation is
    /// encountered.
    /// </summary>
    /// <remarks>This test verifies the behavior of the <see cref="SqlServerConstraintValidator"/> when
    /// handling SQL exceptions that indicate a not-null constraint violation, ensuring that the appropriate exception
    /// type is returned.</remarks>
    [Fact]
    public void HandleConstraintViolation_NotNullViolation_ReturnsNotNullConstraintViolationException()
    {
        // Arrange
        SqlServerConstraintValidator validator = new();
        var sqlEx = SqlExceptionFactory.Create(515, "Not null violation");

        // Act
        var result = validator.HandleConstraintViolation(sqlEx);

        // Assert
        Assert.IsType<NotNullConstraintViolationException>(result);
    }

    /// <summary>
    /// Verifies that the <see cref="SqlServerConstraintValidator.HandleConstraintViolation"/> method returns the
    /// original <see cref="SqlException"/> when an unknown SQL error code is encountered.
    /// </summary>
    /// <remarks>This test ensures that the method does not alter or wrap the exception when the error code is
    /// not recognized as a constraint violation.</remarks>
    [Fact]
    public void HandleConstraintViolation_UnknownSqlException_ReturnsOriginalSqlException()
    {
        // Arrange
        SqlServerConstraintValidator validator = new();
        var sqlEx = SqlExceptionFactory.Create(9999, "Unknown error");

        // Act
        var result = validator.HandleConstraintViolation(sqlEx);

        // Assert
        Assert.Same(sqlEx, result);
    }

    /// <summary>
    /// Verifies that the <see cref="SqlServerConstraintValidator.HandleConstraintViolation"/> method returns the
    /// original exception when a non-SQL exception is provided.
    /// </summary>
    /// <remarks>This test ensures that the method does not alter exceptions that are not related to SQL
    /// constraint violations.</remarks>
    [Fact]
    public void HandleConstraintViolation_NonSqlException_ReturnsOriginalException()
    {
        // Arrange
        SqlServerConstraintValidator validator = new();
        InvalidOperationException ex = new("Not a SQL exception");

        // Act
        var result = validator.HandleConstraintViolation(ex);

        // Assert
        Assert.Same(ex, result);
    }
}