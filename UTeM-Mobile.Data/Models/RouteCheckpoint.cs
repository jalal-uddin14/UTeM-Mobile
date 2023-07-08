using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace UTeM_Mobile.Data.Models
{
    public class RouteCheckpoint
    {
        public int Id { get; set; }
        public int RouteId { get; set; }
        public virtual Route Route { get; set; }
        public int CheckpointId { get; set; }
        public virtual Checkpoint Checkpoint { get; set; }
    }
}
