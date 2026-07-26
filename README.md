# Megaraz.ResultPattern.Infrastructure

Provider-neutral infrastructure error extensions for
[`Megaraz.ResultPattern`](https://github.com/Megaraz/Megaraz.ResultPattern).
The initial package provides database error contracts without tying consumers
to Entity Framework Core, another persistence provider, a logging framework,
or a storage destination.

## Supported frameworks

- .NET 8
- .NET 9
- .NET 10

## Installation

```bash
dotnet add package Megaraz.ResultPattern.Infrastructure
```

This package requires `Megaraz.ResultPattern` 0.2.2 or later.

## Database errors

`DatabaseError` is an `ErrorType.External` error with a database-specific
`DatabaseErrorType`. Its error codes are stable, explicitly database-prefixed
identifiers such as `Update.Account.DatabaseConcurrencyFailure`.

```csharp
var context = new ErrorContext(OperationType.Update, "Account");

try
{
    await repository.SaveChangesAsync(cancellationToken);
    return Result.Success();
}
catch (Exception exception) when (IsConcurrencyFailure(exception))
{
    return Result.Failure(
        DatabaseError.ConcurrencyFailure(context, exception));
}
```

The package does not inspect provider exception types. Applications decide how
to classify exceptions from Entity Framework Core, Dapper, ADO.NET, or another
store, then use the appropriate factory.

Available classifications are:

- `SaveChangesFailure`
- `ConcurrencyFailure`
- `QueryFailure`
- `UnexpectedFailure`

Use `DatabaseError.Custom` for a deliberate application-specific database
classification. When an application owns its error-code contract, pass an
`ErrorCode` to preserve the exact stable code rather than adopting a package
convention:

```csharp
var error = DatabaseError.Custom(
    ErrorCode.From("storage.replica.unavailable"),
    "The selected replica is unavailable.",
    userMessage: "Storage is temporarily unavailable.");
```

The caller-owned code is a compatibility-sensitive application contract. Keep
its meaning stable after publishing it to clients or integrations.

### Safe messages and diagnostics

Technical descriptions do not include exception messages by default, and
`UserMessage` is empty unless the application supplies a vetted value:

```csharp
var error = DatabaseError.SaveChangesFailure(
    context,
    exception,
    userMessage: "We could not save your changes. Please try again.");
```

The original exception is retained in `Error.Exception` for the application's
own logging and diagnostics. Do not expose database exception text, SQL,
connection information, file paths, or stack traces to clients.

## Logging and persistence policy

This package intentionally does not provide a logger, file sink, retention
policy, or log-reader API. Applications should use their existing logging and
observability stack and choose their own storage, retention, redaction, and
access policies:

```csharp
logger.LogError(
    error.Exception,
    "Operation failed with {ErrorCode}: {ErrorDescription}",
    error.Code,
    error.Description);
```

## Versioning

The package follows semantic versioning. Public types, factory behavior,
`DatabaseErrorType` values, and documented error codes are compatibility-
sensitive contracts. Breaking changes require a major version.

## License

This project is licensed under the [MIT License](LICENSE).
