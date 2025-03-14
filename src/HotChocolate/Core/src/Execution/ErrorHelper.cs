using System;
using System.Collections.Generic;
using HotChocolate.Execution.Processing;
using HotChocolate.Language;
using static HotChocolate.Execution.Properties.Resources;

namespace HotChocolate.Execution;

internal static class ErrorHelper
{
    public static IError ArgumentNonNullError(
        ArgumentNode argument,
        string responseName,
        ArgumentNonNullValidator.ValidationResult validationResult)
    {
        return ErrorBuilder.New()
            .SetMessage(
                ErrorHelper_ArgumentNonNullError_Message,
                argument.Name.Value)
            .AddLocation(argument)
            .SetExtension("responseName", responseName)
            .SetExtension("errorPath", validationResult.Path)
            .Build();
    }

    public static IError ArgumentValueIsInvalid(
        ArgumentNode argument,
        string responseName,
        GraphQLException exception)
    {
        return ErrorBuilder.FromError(exception.Errors[0])
            .AddLocation(argument)
            .SetExtension("responseName", responseName)
            .Build();
    }

    public static IError ArgumentDefaultValueIsInvalid(
        string responseName,
        GraphQLException exception)
    {
        return ErrorBuilder.FromError(exception.Errors[0])
            .SetExtension("responseName", responseName)
            .Build();
    }

    public static IError InvalidLeafValue(
        GraphQLException exception,
        FieldNode field,
        Path path)
    {
        return ErrorBuilder.FromError(exception.Errors[0])
            .AddLocation(field)
            .SetPath(path)
            .SetCode(ErrorCodes.Execution.CannotSerializeLeafValue)
            .Build();
    }

    public static IError UnexpectedLeafValueSerializationError(
        Exception exception,
        IErrorHandler errorHandler,
        FieldNode field,
        Path path)
    {
        return errorHandler
            .CreateUnexpectedError(exception)
            .AddLocation(field)
            .SetPath(path)
            .SetCode(ErrorCodes.Execution.CannotSerializeLeafValue)
            .Build();
    }

    public static IError UnableToResolveTheAbstractType(
        string typeName,
        FieldNode field,
        Path path)
    {
        return ErrorBuilder.New()
            .SetMessage(ErrorHelper_UnableToResolveTheAbstractType_Message, typeName)
            .AddLocation(field)
            .SetPath(path)
            .SetCode(ErrorCodes.Execution.CannotResolveAbstractType)
            .Build();
    }

    public static IError UnexpectedErrorWhileResolvingAbstractType(
        Exception exception,
        string typeName,
        FieldNode field,
        Path path)
    {
        return ErrorBuilder.New()
            .SetMessage(ErrorHelper_UnableToResolveTheAbstractType_Message, typeName)
            .AddLocation(field)
            .SetPath(path)
            .SetCode(ErrorCodes.Execution.CannotResolveAbstractType)
            .SetException(exception)
            .Build();
    }

    public static IError ListValueIsNotSupported(
        Type listType,
        FieldNode field,
        Path path)
    {
        return ErrorBuilder.New()
            .SetMessage(ErrorHelper_ListValueIsNotSupported_Message, listType.FullName!)
            .AddLocation(field)
            .SetPath(path)
            .SetCode(ErrorCodes.Execution.ListTypeNotSupported)
            .Build();
    }

    public static IError UnexpectedValueCompletionError(
        FieldNode field,
        Path path)
    {
        return ErrorBuilder.New()
            .SetMessage(ErrorHelper_UnexpectedValueCompletionError_Message)
            .AddLocation(field)
            .SetPath(path)
            .SetCode(ErrorCodes.Execution.ListTypeNotSupported)
            .Build();
    }

    public static IQueryResult RootTypeNotFound(OperationType operationType) =>
        QueryResultBuilder.CreateError(
            ErrorBuilder.New()
                .SetMessage(ErrorHelper_RootTypeNotFound_Message, operationType)
                .Build());

    public static IQueryResult StateInvalidForOperationResolver() =>
        QueryResultBuilder.CreateError(
            ErrorBuilder.New()
                .SetMessage(ErrorHelper_StateInvalidForOperationResolver_Message)
                .Build());

    public static IQueryResult StateInvalidForOperationVariableCoercion() =>
        QueryResultBuilder.CreateError(
            ErrorBuilder.New()
                .SetMessage(ErrorHelper_StateInvalidForOperationVariableCoercion_Message)
                .Build());

    public static IQueryResult StateInvalidForOperationExecution() =>
        QueryResultBuilder.CreateError(
            ErrorBuilder.New()
                .SetMessage(ErrorHelper_StateInvalidForOperationExecution_Message)
                .Build());

    public static IError ValueCompletion_CouldNotResolveAbstractType(
        FieldNode field,
        Path path,
        object result) =>
        ErrorBuilder.New()
            .SetMessage(
                ErrorHelper_ValueCompletion_CouldNotResolveAbstractType_Message,
                result.GetType().FullName ?? result.GetType().Name,
                field.Name)
            .SetPath(path)
            .AddLocation(field)
            .Build();

    public static IQueryResult StateInvalidForDocumentValidation() =>
        QueryResultBuilder.CreateError(
            ErrorBuilder.New()
                .SetMessage(ErrorHelper_StateInvalidForDocumentValidation_Message)
                .SetCode(ErrorCodes.Execution.QueryNotFound)
                .Build());

    public static IQueryResult OperationKindNotAllowed() =>
        QueryResultBuilder.CreateError(
            ErrorBuilder.New()
                .SetMessage("The specified operation kind is not allowed.")
                .Build(),
            new Dictionary<string, object?>
            {
                    { WellKnownContextData.OperationNotAllowed, null }
            });

    public static IQueryResult RequestTimeout(TimeSpan timeout) =>
        QueryResultBuilder.CreateError(
            new Error(
                string.Format(ErrorHelper_RequestTimeout, timeout),
                ErrorCodes.Execution.Timeout));

    public static IQueryResult OperationCanceled() =>
        QueryResultBuilder.CreateError(
            new Error(
                ErrorHelper_OperationCanceled_Message,
                ErrorCodes.Execution.Canceled));

    public static IQueryResult MaxComplexityReached(
        int complexity,
        int allowedComplexity) =>
        QueryResultBuilder.CreateError(
            new Error(
                ErrorHelper_MaxComplexityReached,
                ErrorCodes.Execution.ComplexityExceeded,
                extensions: new Dictionary<string, object?>
                {
                    { nameof(complexity), complexity },
                    { nameof(allowedComplexity), allowedComplexity }
                }),
            contextData: new Dictionary<string, object?>
            {
                { WellKnownContextData.ValidationErrors, true }
            });

    public static IQueryResult StateInvalidForComplexityAnalyzer() =>
        QueryResultBuilder.CreateError(
            ErrorBuilder.New()
                .SetMessage(ErrorHelper_StateInvalidForComplexityAnalyzer_Message)
                .SetCode(ErrorCodes.Execution.ComplexityStateInvalid)
                .Build());

    public static IError NonNullOutputFieldViolation(Path? path, FieldNode selection)
        => ErrorBuilder.New()
            .SetMessage("Cannot return null for non-nullable field.")
            .SetCode(ErrorCodes.Execution.NonNullViolation)
            .SetPath(path)
            .AddLocation(selection)
            .Build();

    public static IError PersistedQueryNotFound(string requestedKey)
        => ErrorBuilder.New()
            .SetMessage(ErrorHelper_PersistedQueryNotFound)
            .SetCode(ErrorCodes.Execution.PersistedQueryNotFound)
            .SetExtension(nameof(requestedKey), requestedKey)
            .Build();

    public static IError OnlyPersistedQueriesAreAllowed()
        => ErrorBuilder.New()
            .SetMessage(ErrorHelper_OnlyPersistedQueriesAreAllowed)
            .SetCode(ErrorCodes.Execution.OnlyPersistedQueriesAllowed)
            .Build();

    public static IError ReadPersistedQueryMiddleware_PersistedQueryNotFound()
        => ErrorBuilder.New()
            // this string is defined in the APQ spec!
            .SetMessage("PersistedQueryNotFound")
            .SetCode(ErrorCodes.Execution.PersistedQueryNotFound)
            .Build();
}
