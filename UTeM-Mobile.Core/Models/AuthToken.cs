namespace UTeM_Mobile.Models
{
    public class AuthToken
    {
        private int userId;
        private string tokenType;
        private double expiresInMinutes;
        private string token;

        public int UserId { get => userId; set => userId = value; }
        public string TokenType { get => tokenType; set => tokenType = value; }
        public double ExpiresInMinutes { get => expiresInMinutes; set => expiresInMinutes = value; }
        public string Token { get => token; set => token = value; }
    }
}
