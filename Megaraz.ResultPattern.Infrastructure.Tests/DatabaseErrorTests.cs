
namespace Megaraz.ResultPattern.Infrastructure.Tests;

public class DatabaseErrorTests
{
    [Theory]
    [InlineData(DatabaseErrorType.Custom, "DatabaseCustom")]
    [InlineData(DatabaseErrorType.SaveChangesFailure, "DatabaseSaveChangesFailure")]
    [InlineData(DatabaseErrorType.ConcurrencyFailure, "DatabaseConcurrencyFailure")]
    [InlineData(DatabaseErrorType.QueryFailure, "DatabaseQueryFailure")]
    [InlineData(DatabaseErrorType.UnexpectedFailure, "DatabaseUnexpectedFailure")]
    public void FactoriesExposeExternalTypeAndStableCode(DatabaseErrorType type, string reason)
    {
        var error = type switch
        {
            DatabaseErrorType.Custom => DatabaseError.Custom(TestData.Context, "custom detail"),
            DatabaseErrorType.SaveChangesFailure => DatabaseError.SaveChangesFailure(TestData.Context),
            DatabaseErrorType.ConcurrencyFailure => DatabaseError.ConcurrencyFailure(TestData.Context),
            DatabaseErrorType.QueryFailure => DatabaseError.QueryFailure(TestData.Context),
            _ => DatabaseError.UnexpectedFailure(TestData.Context)
        };

        Assert.Equal(type, error.DatabaseErrorType);
        Assert.Equal($"Get.Account.{reason}", error.Code);
        Assert.Equal(ErrorType.External, error.Type);
        Assert.NotEmpty(error.Description);
        Assert.Equal(string.Empty, error.UserMessage);
    }

    [Fact]
    public void FactoryPreservesExceptionUserMessageAndCustomDescription()
    {
        var exception = new InvalidOperationException("Sensitive database detail");

        var error = DatabaseError.SaveChangesFailure(
            TestData.Context,
            exception,
            "Changes could not be saved.",
            context => $"Localized {context.EntityName} failure.");

        Assert.Same(exception, error.Exception);
        Assert.Equal("Changes could not be saved.", error.UserMessage);
        Assert.Equal("Localized Account failure.", error.Description);
        Assert.DoesNotContain(exception.Message, error.Description);
    }

    [Fact]
    public void CustomPreservesTechnicalDetailWithoutUsingItAsAUserMessage()
    {
        var error = DatabaseError.Custom(TestData.Context, "The selected store is read-only.");

        Assert.Contains("The selected store is read-only.", error.Description);
        Assert.Equal(string.Empty, error.UserMessage);
    }

    [Fact]
    public void CustomWithCallerOwnedCodePreservesTheExactStableCode()
    {
        var exception = new InvalidOperationException("Sensitive database detail");

        var error = DatabaseError.Custom(
            ErrorCode.From("storage.replica.unavailable"),
            "The selected replica is unavailable.",
            "Storage is temporarily unavailable.",
            exception);

        Assert.Equal("storage.replica.unavailable", error.Code);
        Assert.Equal(DatabaseErrorType.Custom, error.DatabaseErrorType);
        Assert.Equal(ErrorType.External, error.Type);
        Assert.Equal("The selected replica is unavailable.", error.Description);
        Assert.Equal("Storage is temporarily unavailable.", error.UserMessage);
        Assert.Same(exception, error.Exception);
    }

    [Fact]
    public void FactoriesRejectNullContexts()
    {
        ErrorContext nullContext = null!;
        ErrorCode nullCode = null!;

        Assert.Throws<ArgumentNullException>(() => DatabaseError.Custom(nullContext, "detail"));
        Assert.Throws<ArgumentNullException>(() => DatabaseError.Custom(nullCode, "detail"));
        Assert.Throws<ArgumentNullException>(() => DatabaseError.SaveChangesFailure(null!));
        Assert.Throws<ArgumentNullException>(() => DatabaseError.ConcurrencyFailure(null!));
        Assert.Throws<ArgumentNullException>(() => DatabaseError.QueryFailure(null!));
        Assert.Throws<ArgumentNullException>(() => DatabaseError.UnexpectedFailure(null!));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void CustomRejectsBlankDescriptions(string description)
    {
        Assert.Throws<ArgumentException>(() => DatabaseError.Custom(TestData.Context, description));
        Assert.Throws<ArgumentException>(() => DatabaseError.Custom(ErrorCode.From("storage.failure"), description));
    }

    [Fact]
    public void FactoriesRejectBlankMessageFactoryResults()
    {
        Assert.Throws<ArgumentException>(() =>
            DatabaseError.QueryFailure(TestData.Context, messageFactory: _ => " "));
    }
}
