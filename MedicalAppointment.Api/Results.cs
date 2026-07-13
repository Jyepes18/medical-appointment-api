namespace MedicalAppointment.Api;

public class Results<TValue, TStatus>
{
    public TValue Value { get; }
    public string Error { get; }
    public bool IsSuccess { get; }
    public TStatus Status { get; }

    private Results(TValue value, string error, bool isSuccess, TStatus status)
    {
        Value = value;
        Error = error;
        IsSuccess = isSuccess;
        Status = status;
    }

    public static Results<TValue, TStatus> Success(TValue value, TStatus status) =>
        new Results<TValue, TStatus>(value, null, true, status);

    public static Results<TValue, TStatus> Failure(string error, TStatus status) =>
        new Results<TValue, TStatus>(default, error, false, status);
}