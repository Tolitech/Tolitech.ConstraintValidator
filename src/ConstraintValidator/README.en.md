# Tolitech.ConstraintValidator

A static library for managing and validating database constraint exceptions in a database-agnostic way, with specific implementations for PostgreSQL and SQL Server.

## Overview

This library helps you identify and handle integrity constraint exceptions (primary key, foreign key, not null, check) thrown by databases, converting them into custom exceptions for easier handling in your application.

- **Tolitech.ConstraintValidator**: Core and interface for validators.
- **Tolitech.ConstraintValidator.PostgreSql**: PostgreSQL-specific validator.
- **Tolitech.ConstraintValidator.SqlServer**: SQL Server-specific validator.

## Installation

Add the desired package to your project:

```bash
dotnet add package Tolitech.ConstraintValidator

# For PostgreSQL:
dotnet add package Tolitech.ConstraintValidator.PostgreSql

# For SQL Server:
dotnet add package Tolitech.ConstraintValidator.SqlServer
```

## Basic Usage

### 1. Register the validator

At application startup (e.g., Startup):

```csharp
using Tolitech.ConstraintValidator;
using Tolitech.ConstraintValidator.PostgreSql;
using Tolitech.ConstraintValidator.SqlServer;

// For PostgreSQL
ConstraintValidatorManager.AddValidator(new PostgreSqlConstraintValidator());

// For SQL Server
ConstraintValidatorManager.AddValidator(new SqlServerConstraintValidator());
```

### 2. Handle database exceptions

When catching exceptions from database operations:

```csharp
try
{
    // Database operation
}
catch (Exception ex)
{
    Exception handled = ConstraintValidatorManager.Handle(ex);
    if (handled is DatabaseConstraintViolationException)
    {
        // Custom logic for constraint violations
    }
    else
    {
        // Other handling
    }
}
```

## Examples of Handled Exceptions

### PostgreSQL
- Primary key violation: `SqlState = "23505"`
- Foreign key violation: `SqlState = "23503"`
- Check constraint violation: `SqlState = "23514"`
- Not null violation: `SqlState = "23502"`

### SQL Server
- Primary key violation: `Number = 2627`
- Foreign key violation: `Number = 547`
- Not null violation: `Number = 515`

## Advanced Integration

### Remove or clear validators
```csharp
ConstraintValidatorManager.RemoveValidator(yourValidator);
ConstraintValidatorManager.ClearValidators();
```

### Custom implementation
Implement `IConstraintValidator` to create validators for other databases:

```csharp
public class MyDbConstraintValidator : IConstraintValidator
{
    public Exception HandleConstraintViolation(Exception exception)
    {
        // Your logic
    }
}
```

## Benefits
- Centralized constraint exception handling.
- Easy for internationalization and logging.
- Extensible for multiple databases.

## Modern Examples

### Usage with Entity Framework Core
```csharp
try
{
    await dbContext.SaveChangesAsync();
}
catch (Exception ex)
{
    var handled = ConstraintValidatorManager.Handle(ex);
    if (handled is PrimaryKeyViolationException)
        // Notify user about duplicate
}
```

### Usage in APIs
```csharp
[HttpPost]
public IActionResult Create([FromBody] MyObject obj)
{
    try
    {
        // ...
    }
    catch (Exception ex)
    {
        var handled = ConstraintValidatorManager.Handle(ex);
        if (handled is DatabaseConstraintViolationException)
            return BadRequest(handled.Message);
        throw;
    }
}
```

---

> **Tolitech.ConstraintValidator** makes constraint exception handling simple, clean, and ready for multiple databases.
