namespace User.Gateway.DTO
{
    public class FLResponseModenaIdentity<T>
    {
        public int Status { get; set; }

        public string Message { get; set; }

        public T User { get; set; }
    }
}
