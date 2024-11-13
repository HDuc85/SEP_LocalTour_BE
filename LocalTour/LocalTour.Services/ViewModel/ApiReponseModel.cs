namespace LocalTour.Services.ViewModel;
public class ApiReponseModel<T>
{
    public T? data { get; set; }
    public bool success { get; set; } = true;
    public string message { get; set; } = string.Empty;

    public ApiReponseModel(T data)
    {
        data = data;
    }
    public ApiReponseModel(bool success, string message)
    {
        success = success;
        message = message;
    }
}
