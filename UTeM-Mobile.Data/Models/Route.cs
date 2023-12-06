namespace UTeM_Mobile.Data.Models
{
    public class Route
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public int CampusId { get; set; }

        public virtual Campus Campus { get; set; }
        public virtual List<RouteCheckpoint>? RouteCheckpoints { get; set; }
    }
}
