# Tolitech.ConstraintValidator.PostgreSql

Biblioteca estática para gerenciar e validar exceções de restrições de banco de dados especificamente para PostgreSQL.

## Visão Geral

Este pacote fornece uma implementação específica para PostgreSQL do validador de restrições, permitindo tratar exceções de integridade (chave primária, chave estrangeira, not null, check) lançadas pelo PostgreSQL e convertê-las em exceções customizadas para sua aplicação.

## Instalação

```bash
dotnet add package Tolitech.ConstraintValidator.PostgreSql
```

## Uso

### 1. Registrar o validador PostgreSQL

```csharp
using Tolitech.ConstraintValidator;
using Tolitech.ConstraintValidator.PostgreSql;

ConstraintValidatorManager.AddValidator(new PostgreSqlConstraintValidator());
```

### 2. Tratar exceções do PostgreSQL

```csharp
try
{
    // Operação de banco de dados
}
catch (Exception ex)
{
    Exception tratada = ConstraintValidatorManager.Handle(ex);
    if (tratada is DatabaseConstraintViolationException)
    {
        // Lógica customizada para restrições
    }
}
```

## Restrições PostgreSQL Suportadas
- Violação de chave primária: `SqlState = "23505"`
- Violação de chave estrangeira: `SqlState = "23503"`
- Violação de check: `SqlState = "23514"`
- Violação de not null: `SqlState = "23502"`

## Avançado

Você pode remover ou limpar validadores:
```csharp
ConstraintValidatorManager.RemoveValidator(seuValidador);
ConstraintValidatorManager.ClearValidators();
```

## Exemplo: Entity Framework Core
```csharp
try
{
    await dbContext.SaveChangesAsync();
}
catch (Exception ex)
{
    var tratada = ConstraintValidatorManager.Handle(ex);
    if (tratada is PrimaryKeyViolationException)
        // Notifique usuário sobre duplicidade
}
```

---

> **Tolitech.ConstraintValidator.PostgreSql** simplifica o tratamento de exceções de restrição no PostgreSQL.
