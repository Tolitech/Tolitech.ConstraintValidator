# Tolitech.ConstraintValidator.SqlServer

Biblioteca estática para gerenciar e validar exceções de restrições de banco de dados especificamente para SQL Server.

## Visão Geral

Este pacote fornece uma implementação específica para SQL Server do validador de restrições, permitindo tratar exceções de integridade (chave primária, chave estrangeira, not null) lançadas pelo SQL Server e convertê-las em exceções customizadas para sua aplicação.

## Instalação

```bash
dotnet add package Tolitech.ConstraintValidator.SqlServer
```

## Uso

### 1. Registrar o validador SQL Server

```csharp
using Tolitech.ConstraintValidator;
using Tolitech.ConstraintValidator.SqlServer;

ConstraintValidatorManager.AddValidator(new SqlServerConstraintValidator());
```

### 2. Tratar exceções do SQL Server

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

## Restrições SQL Server Suportadas
- Violação de chave primária: `Number = 2627`
- Violação de chave estrangeira: `Number = 547`
- Violação de not null: `Number = 515`

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

> **Tolitech.ConstraintValidator.SqlServer** simplifica o tratamento de exceções de restrição no SQL Server.
