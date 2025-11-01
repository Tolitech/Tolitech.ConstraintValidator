# Tolitech.ConstraintValidator.SqlServer

A static library for managing and validating database constraint exceptions specifically for SQL Server.

## Overview

This package provides a SQL Server-specific implementation of the constraint validator interface, allowing you to handle integrity exceptions (primary key, foreign key, not null) thrown by SQL Server and convert them into custom exceptions for your application.

## Installation

```bash
dotnet add package Tolitech.ConstraintValidator.SqlServer
```

## Usage

### 1. Register the SQL Server validator

```csharp
using Tolitech.ConstraintValidator;
using Tolitech.ConstraintValidator.SqlServer;

ConstraintValidatorManager.AddValidator(new SqlServerConstraintValidator());
```

### 2. Handle SQL Server exceptions

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
}
```

## Supported SQL Server Constraint Violations
- Primary key violation: `Number = 2627`
- Foreign key violation: `Number = 547`
- Not null violation: `Number = 515`

## Advanced

You can remove or clear validators:
```csharp
ConstraintValidatorManager.RemoveValidator(yourValidator);
ConstraintValidatorManager.ClearValidators();
```

## Example: Entity Framework Core
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

---

> **Tolitech.ConstraintValidator.SqlServer** makes handling SQL Server constraint exceptions simple and robust.
