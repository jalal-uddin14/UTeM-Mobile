using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace UTeM_Mobile.Data.Models
{
    public class Route
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public virtual IList<RouteCheckpoint> RouteCheckpoints { get; set; }
    }
}
