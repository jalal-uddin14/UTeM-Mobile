using UTeM_Mobile.Core.Models;
using UTeM_Mobile.Data.Models;

namespace UTeM_Mobile.StaticProperties
{
    public static class StaticCredentials
    {
        public static PatrolDetail PatrolDetail { get; set; }
        public static Patrol Patrol { get; set; }
        public static PatrolDetail NextPatrolDetail { get; set; }
        public static CheckpointTimer CheckpointTimer { get; set; }
        public static bool IsScanning { get; set; }
        public static bool IsNotificationSend { get; set; }
        public static bool ShiftNotificationShowed { get; set; }
        public static bool PatrolNotificationShowed { get; set; }
    }
}
