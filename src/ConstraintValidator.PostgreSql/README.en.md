# Tolitech.ConstraintValidator.PostgreSql

A static library for managing and validating database constraint exceptions specifically for PostgreSQL.

## Overview

This package provides a PostgreSQL-specific implementation of the constraint validator interface, allowing you to handle integrity exceptions (primary key, foreign key, not null, check) thrown by PostgreSQL and convert them into custom exceptions for your application.

## Installation

```bash
dotnet add package Tolitech.ConstraintValidator.PostgreSql
```

## Usage

### 1. Register the PostgreSQL validator

```csharp
using Tolitech.ConstraintValidator;
using Tolitech.ConstraintValidator.PostgreSql;

ConstraintValidatorManager.AddValidator(new PostgreSqlConstraintValidator());
```

### 2. Handle PostgreSQL exceptions

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

## Supported PostgreSQL Constraint Violations
- Primary key violation: `SqlState = "23505"`
- Foreign key violation: `SqlState = "23503"`
- Check constraint violation: `SqlState = "23514"`
- Not null violation: `SqlState = "23502"`

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

> **Tolitech.ConstraintValidator.PostgreSql** makes handling PostgreSQL constraint exceptions simple and robust.
