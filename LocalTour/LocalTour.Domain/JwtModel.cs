namespace LocalTour.Domain
{
    public class JwtModel
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public string PhoneNumber { get; set; }
        public string FullName { get; set; }
        public DateTime AccessTokenExpiredDate { get; set; }
    }
}
