namespace Msd.Services.AuthApi.Models
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; } = true;
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }

        public ApiResponse()
        { }

        public ApiResponse(string errorMessage) 
            : this()
        {
            Success = false;
            Message = errorMessage;
            Data = default;
        }

        public ApiResponse(T data)
        {
            Data = data;
        }
    }
}

