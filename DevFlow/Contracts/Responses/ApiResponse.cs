namespace DevFlow.Api.Contracts.Responses
{
    public class ApiResponse<T>
    {
        public bool Success {  get; set; }
        public String Message { get; set; } = null!;
        public T? Data {  get; set; }
    public static ApiResponse<T>Ok(T ?data, String message)
        {
            return new ApiResponse<T>
            {
                Success= true,
                Message= message,
                Data= data

            };
        }
        public static ApiResponse<T> Fail(T? data ,String message)
        {
            return new ApiResponse<T>
            {
                Success = false,
                Message = message,
                Data = data
            };

        }



    }
}
