namespace UTeM_Mobile.Data.Models
{
    public class TimeSchedule
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public virtual IList<TimeScheduleDetail> TimeScheduleDetails { get; set; }
    }
}
