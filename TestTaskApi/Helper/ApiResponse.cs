namespace TestTaskApi.Helper
{
    public class ApiResponse<T>
    {
        public bool IsSuccess { get; set; }
        public T Data { get; set; }
        public string Message { get; set; }
        public int Status { get; set; }

        public ApiResponse(bool isSuccess, T data = default, string message = "", int status = 200)
        {
            IsSuccess = isSuccess;
            Data = data;
            Message = message;
            Status = status;
        }


    }
}
