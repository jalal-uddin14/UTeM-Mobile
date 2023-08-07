namespace UTeM_Mobile.Core.Models
{
    public class CheckpointTimer
    {
        public int PatrolId { get; set; }
        public int CheckpointId { get; set; }
        public DateTime? ExpectedCheckedTime { get; set; }
        public string CheckpointName { get; set; }
    }
}
