namespace User.Gateway.DTO
{
    public class JWTDto
    {
        public string SigningSecret { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public int ExpiryDuration { get; set; }
        public int RefreshExpiryDuration { get; set; }
    }
}
