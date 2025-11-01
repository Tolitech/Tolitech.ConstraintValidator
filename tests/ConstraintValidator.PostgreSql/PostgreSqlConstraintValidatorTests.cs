using Tolitech.Exceptions.Database;

namespace Tolitech.ConstraintValidator.PostgreSql.UnitTests;

/// <summary>
/// Contains unit tests for the <see cref="PostgreSqlConstraintValidator"/> class, specifically testing the <see
/// cref="PostgreSqlConstraintValidator.HandleConstraintViolation"/> method for various SQL constraint violations.
/// </summary>
/// <remarks>This test class verifies that the <see cref="PostgreSqlConstraintValidator"/> correctly identifies
/// and handles different types of SQL constraint violations by returning the appropriate exception types. It includes
/// tests for primary key, foreign key, check constraint, and not-null constraint violations, as well as tests for
/// handling unknown SQL states and non-Postgres exceptions.</remarks>
public class PostgreSqlConstraintValidatorTests
{
    /// <summary>
    /// Tests that the <see cref="PostgreSqlConstraintValidator.HandleConstraintViolation"/> method returns a <see
    /// cref="PrimaryKeyViolationException"/> when a primary key violation occurs.
    /// </summary>
    /// <remarks>This test verifies that the <see cref="PostgreSqlConstraintValidator"/> correctly identifies
    /// a primary key violation from a <see cref="Npgsql.PostgresException"/> with SQL state "23505" and returns the
    /// appropriate exception type.</remarks>
    [Fact]
    public void HandleConstraintViolation_PrimaryKeyViolation_ReturnsPrimaryKeyViolationException()
    {
        // Arrange
        PostgreSqlConstraintValidator validator = new();
        var pgEx = PostgresExceptionFactory.Create("23505", "Primary key violation");

        // Act
        var result = validator.HandleConstraintViolation(pgEx);

        // Assert
        Assert.IsType<PrimaryKeyViolationException>(result);
    }

    /// <summary>
    /// Tests that the <see cref="PostgreSqlConstraintValidator.HandleConstraintViolation"/> method returns a <see
    /// cref="ForeignKeyViolationException"/> when a foreign key violation occurs.
    /// </summary>
    /// <remarks>This test verifies that the <see cref="PostgreSqlConstraintValidator"/> correctly identifies
    /// a foreign key violation from a <see cref="Npgsql.PostgresException"/> with SQL state "23503" and returns the
    /// appropriate exception type.</remarks>
    [Fact]
    public void HandleConstraintViolation_ForeignKeyViolation_ReturnsForeignKeyViolationException()
    {
        // Arrange
        PostgreSqlConstraintValidator validator = new();
        var pgEx = PostgresExceptionFactory.Create("23503", "Foreign key violation");

        // Act
        var result = validator.HandleConstraintViolation(pgEx);

        // Assert
        Assert.IsType<ForeignKeyViolationException>(result);
    }

    /// <summary>
    /// Tests that the <see cref="PostgreSqlConstraintValidator.HandleConstraintViolation"/> method returns a <see
    /// cref="CheckConstraintViolationException"/> when a check constraint violation occurs.
    /// </summary>
    /// <remarks>This test verifies that the <see cref="PostgreSqlConstraintValidator"/> correctly identifies
    /// and handles a check constraint violation by returning the appropriate exception type.</remarks>
    [Fact]
    public void HandleConstraintViolation_CheckConstraintViolation_ReturnsCheckConstraintViolationException()
    {
        // Arrange
        PostgreSqlConstraintValidator validator = new();
        var pgEx = PostgresExceptionFactory.Create("23514", "Check constraint violation");

        // Act
        var result = validator.HandleConstraintViolation(pgEx);

        // Assert
        Assert.IsType<CheckConstraintViolationException>(result);
    }

    /// <summary>
    /// Tests that the <see cref="PostgreSqlConstraintValidator.HandleConstraintViolation"/> method returns a <see
    /// cref="NotNullConstraintViolationException"/> when a not-null constraint violation occurs.
    /// </summary>
    /// <remarks>This test verifies that the <see cref="PostgreSqlConstraintValidator"/> correctly identifies
    /// a not-null constraint violation from a PostgreSQL exception and returns the appropriate exception
    /// type.</remarks>
    [Fact]
    public void HandleConstraintViolation_NotNullConstraintViolation_ReturnsNotNullConstraintViolationException()
    {
        // Arrange
        PostgreSqlConstraintValidator validator = new();
        var pgEx = PostgresExceptionFactory.Create("23502", "Not null constraint violation");

        // Act
        var result = validator.HandleConstraintViolation(pgEx);

        // Assert
        Assert.IsType<NotNullConstraintViolationException>(result);
    }

    /// <summary>
    /// Verifies that the <see cref="PostgreSqlConstraintValidator.HandleConstraintViolation"/> method returns the
    /// original <see cref="Npgsql.PostgresException"/> when an unknown SQL state is encountered.
    /// </summary>
    /// <remarks>This test ensures that the method does not alter or wrap the exception when the SQL state is
    /// not recognized, maintaining the original exception for further handling or logging.</remarks>
    [Fact]
    public void HandleConstraintViolation_UnknownSqlState_ReturnsOriginalPostgresException()
    {
        // Arrange
        PostgreSqlConstraintValidator validator = new();
        var pgEx = PostgresExceptionFactory.Create("99999", "Unknown error");

        // Act
        var result = validator.HandleConstraintViolation(pgEx);

        // Assert
        Assert.Same(pgEx, result);
    }

    /// <summary>
    /// Verifies that the <see cref="PostgreSqlConstraintValidator.HandleConstraintViolation"/> method returns the
    /// original exception when a non-Postgres exception is provided.
    /// </summary>
    /// <remarks>This test ensures that the <see cref="PostgreSqlConstraintValidator"/> does not alter
    /// exceptions that are not related to Postgres constraint violations, maintaining the original exception
    /// instance.</remarks>
    [Fact]
    public void HandleConstraintViolation_NonPostgresException_ReturnsOriginalException()
    {
        // Arrange
        PostgreSqlConstraintValidator validator = new();
        InvalidOperationException ex = new("Not a Postgres exception");

        // Act
        var result = validator.HandleConstraintViolation(ex);

        // Assert
        Assert.Same(ex, result);
    }
}