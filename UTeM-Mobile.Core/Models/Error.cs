namespace UTeM_Mobile.Core.Models
{
    public class Error
    {
        public string error { get; set; }
        public string Message { get; set; }
        public Dictionary<string, List<string>> Errors { get; set; } = new Dictionary<string, List<string>>();
    }
}
