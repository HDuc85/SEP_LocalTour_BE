namespace LocalTour.Domain.Common
{
    public class JwtModel
    {
        public string accessToken { get; set; }
        public string refreshToken { get; set; }
        public string userId { get; set; }
        public DateTime accessTokenExpiredDate { get; set; }
        public DateTime refeshTokenExpiredDate { get; set; }
    }
}
