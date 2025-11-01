using Tolitech.ConstraintValidator.UnitTests.Implementations;

namespace Tolitech.ConstraintValidator.UnitTests;

/// <summary>
/// Provides unit tests for the <see cref="ConstraintValidatorManager"/> class, focusing on the behavior of its <see
/// cref="ConstraintValidatorManager.Handle(Exception)"/> method under various conditions.
/// </summary>
/// <remarks>This test class verifies the functionality of the <see cref="ConstraintValidatorManager"/> by
/// ensuring that: <list type="bullet"> <item> <description> The <see
/// cref="ConstraintValidatorManager.Handle(Exception)"/> method correctly returns a custom exception when a registered
/// validator transforms the input exception. </description> </item> <item> <description> The <see
/// cref="ConstraintValidatorManager.Handle(Exception)"/> method returns the original exception when no registered
/// validator can handle the provided exception. </description> </item> </list></remarks>
public class ConstraintValidationManagerTests
{
    /// <summary>
    /// Tests that the <see cref="ConstraintValidatorManager.Handle"/> method returns a custom exception when the
    /// registered validator produces a custom exception for the given input exception.
    /// </summary>
    /// <remarks>This test verifies the behavior of the <see cref="ConstraintValidatorManager.Handle"/> method
    /// when a validator registered with <see cref="ConstraintValidatorManager"/> is capable of transforming the input
    /// exception into a custom exception. The test ensures that the custom exception is returned as expected.</remarks>
    [Fact]
    public void Handle_ReturnsCustomException_WhenValidatorReturnsCustomException()
    {
        // Arrange
        ConstraintValidatorManager.AddValidator(new DummyConstraintValidator());
        Exception inputException = new ApplicationException();

        // Act
        var result = ConstraintValidatorManager.Handle(inputException);

        // Assert
        Assert.IsType<DummyException>(result);
    }

    /// <summary>
    /// Tests that the <see cref="ConstraintValidatorManager.Handle(Exception)"/> method  returns the original exception
    /// when no registered validator can handle the provided exception.
    /// </summary>
    /// <remarks>This test ensures that the <see cref="ConstraintValidatorManager.Handle(Exception)"/> method
    /// does not modify or replace the exception if no validator is capable of handling it.</remarks>
    [Fact]
    public void Handle_ReturnsOriginalException_WhenNoValidatorHandlesException()
    {
        // Arrange
        ConstraintValidatorManager.AddValidator(new DummyConstraintValidator());
        Exception inputException = new();

        // Act
        var result = ConstraintValidatorManager.Handle(inputException);

        // Assert
        Assert.IsNotType<DummyException>(result);
    }

    /// <summary>
    /// Tests that the <see cref="ConstraintValidatorManager.Handle(Exception)"/> method returns the original exception
    /// when no validator is registered for the provided exception type.
    /// </summary>
    /// <remarks>This test ensures that the <see cref="ConstraintValidatorManager.Handle(Exception)"/> method
    /// does not modify or  replace the exception if no applicable validator is available. The result should be the same
    /// instance as the input exception.</remarks>
    [Fact]
    public void Handle_ReturnsOriginalException_WhenNoValidatorIsRegistered()
    {
        // Arrange
        Exception inputException = new();

        // Act
        var result = ConstraintValidatorManager.Handle(inputException);

        // Assert
        Assert.IsNotType<DummyException>(result);
    }

    /// <summary>
    /// Tests that the <see cref="ConstraintValidatorManager.ClearValidators"/> method removes all validators.
    /// </summary>
    /// <remarks>This test ensures that after calling <see
    /// cref="ConstraintValidatorManager.ClearValidators"/>, no validators remain in the <see
    /// cref="ConstraintValidatorManager"/>, and exceptions are not handled by any previously added
    /// validators.</remarks>
    [Fact]
    public void ClearValidators_RemovesAllValidators()
    {
        // Arrange
        ConstraintValidatorManager.AddValidator(new DummyConstraintValidator());

        // Act
        ConstraintValidatorManager.ClearValidators();
        Exception inputException = new DummyException("Test message");
        var result = ConstraintValidatorManager.Handle(inputException);

        // Assert
        Assert.IsType<DummyException>(result);
    }

    /// <summary>
    /// Tests that a specific validator is removed from the <see cref="ConstraintValidatorManager"/>.
    /// </summary>
    /// <remarks>This test verifies that after removing a specific validator, the <see
    /// cref="ConstraintValidatorManager"/> no longer applies that validator to exceptions, ensuring that the removed
    /// validator does not affect subsequent exception handling.</remarks>
    [Fact]
    public void RemoveValidator_RemovesSpecificValidator()
    {
        // Arrange
        DummyConstraintValidator validator = new();
        ConstraintValidatorManager.AddValidator(validator);

        // Act
        ConstraintValidatorManager.RemoveValidator(validator);
        Exception inputException = new DummyException();
        var result = ConstraintValidatorManager.Handle(inputException);

        // Assert
        Assert.IsType<DummyException>(result);
    }
}