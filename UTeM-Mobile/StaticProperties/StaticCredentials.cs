using UTeM_Mobile.Core.Models;
using UTeM_Mobile.Data.Models;

namespace UTeM_Mobile.StaticProperties
{
    public static class StaticCredentials
    {
        public static PatrolDetail PatrolDetail { get; set; }
        public static Patrol Patrol { get; set; }
        public static PatrolDetail NextPatrol { get; set; }
        public static CheckpointTimer CheckpointTimer { get; set; }
    }
}
