using StorageService.Api.extensions;
using StorageService.Domain.errs;

namespace ApiShared;

public class CustomResults
{
    public record class ErrorResponseObject(Err err);

    public static IResult ErrorResponse(Err e) =>
        Results.Json(e, statusCode: e.ToHttpStatusCode());

   

    public static IResult FromErrOrNothing(ErrOrNothing possibleErr, Func<IResult> successFunc) =>
        possibleErr.IsErr(out var err) ? ErrorResponse(err) : successFunc();

    public static IResult FromErrOr<T>(ErrOr<T> errOrValue, Func<T, IResult> successFunc) =>
        errOrValue.Match(successFunc, ErrorResponse);

    public static IResult AuthRequired() => ErrorResponse(ErrFactory.AuthRequired());

    public static IResult Created(object responseObject) =>
        Results.Json(responseObject, statusCode: StatusCodes.Status201Created);
}