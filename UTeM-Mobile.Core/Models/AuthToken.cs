namespace UTeM_Mobile.Models
{
    public class AuthToken
    {
        public string UserId { get; set; }
        public string TokenType { get; set; }
        public DateTime ValidTo { get; set; }
        public string Token { get; set; }
        public string UserRole { get; set; }
        public bool IsRemember { get; set; }
    }
}
