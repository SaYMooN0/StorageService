namespace StorageService.Domain.errs;

public class ErrOrNothing
{
    private Err? _err;
    private ErrOrNothing(Err? err) { _err = err; }
    public bool IsErr() => _err != null;
    public bool IsErr(out Err err) {
        if (_err != null) {
            err = _err;
            return true;
        }

        err = new Err("No error");
        return false;
    }
    public static implicit operator ErrOrNothing(Err err) => new(err);
    public static readonly ErrOrNothing Nothing = new ErrOrNothing(null);
}
