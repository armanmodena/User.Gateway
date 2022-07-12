namespace User.Gateway.DTO
{
    public class FLResponseDto<T>
    {
        public int Status { get; set; }

        public string Message { get; set; }

        public T Data { get; set; }

        public object? Errors { get; set; }
    }
}
