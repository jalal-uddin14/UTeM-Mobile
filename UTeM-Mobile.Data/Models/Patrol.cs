namespace UTeM_Mobile.Data.Models
{
    public class Patrol
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public string Status { get; set; }
        public int RouteId { get; set; }
        public virtual Route Route { get; set; }
        public string GuardId { get; set; }
        public virtual ApplicationUser Guard { get; set; }
    }
}
