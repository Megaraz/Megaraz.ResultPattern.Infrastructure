namespace Megaraz.ResultPattern.Infrastructure;

/// <summary>Classifies an error produced while interacting with a database or persistence store.</summary>
public enum DatabaseErrorType
{
    /// <summary>A caller-defined database error.</summary>
    Custom = 0,

    /// <summary>Saving changes failed.</summary>
    SaveChangesFailure = 1,

    /// <summary>A concurrent update or delete prevented the operation from completing.</summary>
    ConcurrencyFailure = 2,

    /// <summary>Querying data failed.</summary>
    QueryFailure = 3,

    /// <summary>An unexpected database failure occurred.</summary>
    UnexpectedFailure = 4
}

/// <summary>
/// Represents a provider-neutral database error while retaining the core package's
/// <see cref="ErrorType.External"/> classification.
/// </summary>
public sealed record DatabaseError : Error
{
    private DatabaseError(
        string code,
        string description,
        DatabaseErrorType type,
        string userMessage,
        Exception? exception = null)
        : base(code, description, ErrorType.External, userMessage, exception)
    {
        DatabaseErrorType = type;
    }

    /// <summary>Gets the database-specific classification.</summary>
    public DatabaseErrorType DatabaseErrorType { get; }

    /// <summary>Creates a caller-defined database error.</summary>
    /// <param name="errorContext">The operation and entity context for the error code.</param>
    /// <param name="customDescription">Technical detail describing the failure.</param>
    /// <param name="userMessage">Optional application-owned message safe for clients.</param>
    /// <param name="messageFactory">Optional application-owned description factory.</param>
    /// <param name="exception">The underlying exception, when available.</param>
    /// <returns>A caller-defined database error.</returns>
    public static DatabaseError Custom(
        ErrorContext errorContext,
        string customDescription,
        string? userMessage = null,
        Func<ErrorContext, string>? messageFactory = null,
        Exception? exception = null) =>
        Create(errorContext, "DatabaseCustom", DatabaseErrorType.Custom, customDescription, userMessage,
            messageFactory, exception);

    /// <summary>Creates an error for a failure while saving changes.</summary>
    /// <param name="errorContext">The operation and entity context for the error code.</param>
    /// <param name="exception">The underlying persistence exception, when available.</param>
    /// <param name="userMessage">Optional application-owned message safe for clients.</param>
    /// <param name="messageFactory">Optional application-owned description factory.</param>
    /// <returns>A save-changes failure error.</returns>
    public static DatabaseError SaveChangesFailure(
        ErrorContext errorContext,
        Exception? exception = null,
        string? userMessage = null,
        Func<ErrorContext, string>? messageFactory = null) =>
        Create(errorContext, "DatabaseSaveChangesFailure", DatabaseErrorType.SaveChangesFailure,
            "A database failure occurred while saving changes.", userMessage, messageFactory, exception);

    /// <summary>Creates an error for a persistence concurrency conflict.</summary>
    /// <param name="errorContext">The operation and entity context for the error code.</param>
    /// <param name="exception">The underlying persistence exception, when available.</param>
    /// <param name="userMessage">Optional application-owned message safe for clients.</param>
    /// <param name="messageFactory">Optional application-owned description factory.</param>
    /// <returns>A concurrency failure error.</returns>
    public static DatabaseError ConcurrencyFailure(
        ErrorContext errorContext,
        Exception? exception = null,
        string? userMessage = null,
        Func<ErrorContext, string>? messageFactory = null) =>
        Create(errorContext, "DatabaseConcurrencyFailure", DatabaseErrorType.ConcurrencyFailure,
            "A database concurrency conflict occurred. The data may have been modified or deleted by another operation.",
            userMessage, messageFactory, exception);

    /// <summary>Creates an error for a failure while querying data.</summary>
    /// <param name="errorContext">The operation and entity context for the error code.</param>
    /// <param name="exception">The underlying persistence exception, when available.</param>
    /// <param name="userMessage">Optional application-owned message safe for clients.</param>
    /// <param name="messageFactory">Optional application-owned description factory.</param>
    /// <returns>A query failure error.</returns>
    public static DatabaseError QueryFailure(
        ErrorContext errorContext,
        Exception? exception = null,
        string? userMessage = null,
        Func<ErrorContext, string>? messageFactory = null) =>
        Create(errorContext, "DatabaseQueryFailure", DatabaseErrorType.QueryFailure,
            "A database failure occurred while querying data.", userMessage, messageFactory, exception);

    /// <summary>Creates an error for an unexpected database failure.</summary>
    /// <param name="errorContext">The operation and entity context for the error code.</param>
    /// <param name="exception">The underlying persistence exception, when available.</param>
    /// <param name="userMessage">Optional application-owned message safe for clients.</param>
    /// <param name="messageFactory">Optional application-owned description factory.</param>
    /// <returns>An unexpected database failure error.</returns>
    public static DatabaseError UnexpectedFailure(
        ErrorContext errorContext,
        Exception? exception = null,
        string? userMessage = null,
        Func<ErrorContext, string>? messageFactory = null) =>
        Create(errorContext, "DatabaseUnexpectedFailure", DatabaseErrorType.UnexpectedFailure,
            "An unexpected database failure occurred.", userMessage, messageFactory, exception);

    private static DatabaseError Create(
        ErrorContext context,
        string reason,
        DatabaseErrorType type,
        string description,
        string? userMessage,
        Func<ErrorContext, string>? messageFactory = null,
        Exception? exception = null)
    {
        ArgumentNullException.ThrowIfNull(context);
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("The description cannot be null, empty, or whitespace.", nameof(description));

        var code = ErrorCode.For(context, reason);
        var formattedDescription = $"{context.EntityName}: {description}";
        var resolvedDescription = messageFactory is null ? formattedDescription : messageFactory(context);
        if (string.IsNullOrWhiteSpace(resolvedDescription))
            throw new ArgumentException("The message factory must return a non-blank message.", nameof(messageFactory));

        return new DatabaseError(code.Code, resolvedDescription, type, userMessage ?? string.Empty, exception);
    }
}
