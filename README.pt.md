# Tolitech.ConstraintValidator

Biblioteca estática para gerenciar e validar exceções de restrições de banco de dados de forma agnóstica, com implementações específicas para PostgreSQL e SQL Server.

## Visão Geral

Esta biblioteca permite identificar e tratar exceções de restrições de integridade (chave primária, chave estrangeira, not null, check) lançadas por bancos de dados, convertendo-as em exceções customizadas para facilitar o tratamento na aplicação.

- **Tolitech.ConstraintValidator**: Núcleo agnóstico e interface para validadores.
- **Tolitech.ConstraintValidator.PostgreSql**: Validador específico para PostgreSQL.
- **Tolitech.ConstraintValidator.SqlServer**: Validador específico para SQL Server.

## Instalação

Adicione o pacote desejado ao seu projeto:

```bash
dotnet add package Tolitech.ConstraintValidator

# Para PostgreSQL:
dotnet add package Tolitech.ConstraintValidator.PostgreSql

# Para SQL Server:
dotnet add package Tolitech.ConstraintValidator.SqlServer
```

## Uso Básico

### 1. Registrar o validador

No início da aplicação (ex: Startup):

```csharp
using Tolitech.ConstraintValidator;
using Tolitech.ConstraintValidator.PostgreSql;
using Tolitech.ConstraintValidator.SqlServer;

// Para PostgreSQL
ConstraintValidatorManager.AddValidator(new PostgreSqlConstraintValidator());

// Para SQL Server
ConstraintValidatorManager.AddValidator(new SqlServerConstraintValidator());
```

### 2. Tratar exceções de banco de dados

Ao capturar exceções de operações de banco de dados:

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
    else
    {
        // Outro tratamento
    }
}
```

## Exemplos de Exceções Tratadas

### PostgreSQL
- Violação de chave primária: `SqlState = "23505"`
- Violação de chave estrangeira: `SqlState = "23503"`
- Violação de check: `SqlState = "23514"`
- Violação de not null: `SqlState = "23502"`

### SQL Server
- Violação de chave primária: `Number = 2627`
- Violação de chave estrangeira: `Number = 547`
- Violação de not null: `Number = 515`

## Integração Avançada

### Remover ou limpar validadores
```csharp
ConstraintValidatorManager.RemoveValidator(seuValidador);
ConstraintValidatorManager.ClearValidators();
```

### Implementação customizada
Implemente `IConstraintValidator` para criar validadores para outros bancos:

```csharp
public class MyDbConstraintValidator : IConstraintValidator
{
    public Exception HandleConstraintViolation(Exception exception)
    {
        // Sua lógica
    }
}
```

## Benefícios
- Centralização do tratamento de exceções de restrição.
- Facilidade para internacionalização e logging.
- Extensível para múltiplos bancos.

## Exemplos Modernos

### Uso com Entity Framework Core
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

### Uso em APIs
```csharp
[HttpPost]
public IActionResult Criar([FromBody] MeuObjeto obj)
{
    try
    {
        // ...
    }
    catch (Exception ex)
    {
        var tratada = ConstraintValidatorManager.Handle(ex);
        if (tratada is DatabaseConstraintViolationException)
            return BadRequest(tratada.Message);
        throw;
    }
}
```

---

> **Tolitech.ConstraintValidator** simplifica o tratamento de exceções de restrição, tornando seu código mais limpo, seguro e preparado para múltiplos bancos de dados.
