namespace StorageService.Domain.errs;

public class ErrOr<T>
{
    private readonly T? _success;
    private readonly Err? _error;
    private readonly bool _isSuccess;

    private ErrOr(T success) {
        _success = success;
        _error = null;
        _isSuccess = true;
    }

    private ErrOr(Err error) {
        _success = default;
        _error = error;
        _isSuccess = false;
    }

    public static ErrOr<T> Success(T value) => new(value);
    public static ErrOr<T> Error(Err err) => new(err);

    public bool IsSuccess() => _isSuccess;
    public bool IsErr() => !_isSuccess;

    public bool IsSuccess(out T value) {
        if (_isSuccess) {
            value = _success!;
            return true;
        }

        value = default!;
        return false;
    }

    public bool IsErr(out Err err) {
        if (!_isSuccess) {
            err = _error!;
            return true;
        }

        err = new Err("No error");
        return false;
    }

    public T AsSuccess() {
        if (_isSuccess) {
            return _success!;
        }

        throw new InvalidOperationException("No success value available");
    }

    public Err AsErr() {
        if (!_isSuccess) {
            return _error!;
        }

        return new Err("No error");
    }

    public TResult Match<TResult>(Func<T, TResult> successFunc, Func<Err, TResult> errorFunc) =>
        _isSuccess ? successFunc(_success!) : errorFunc(_error!);

    public static implicit operator ErrOr<T>(T value) => new(value);
    public static implicit operator ErrOr<T>(Err err) => new(err);
}