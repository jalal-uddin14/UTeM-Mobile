namespace UTeM_Mobile.Data.Models
{
    public class ApplicationUser
    {
        public string Name { get; set; }
        public string Status { get; set; }
        public string Role { get; set; }
        public string? Position { get; set; }
        public string? SupervisorId { get; set; }
        public virtual ApplicationUser Supervisor { get; set; }
    }
}
