using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace UTeM_Mobile.Data.Models
{
    public class TimeScheduleDetail
    {
        public int Id { get; set; }
        public int TimeScheduleId { get; set; }
        public TimeSpan Time { get; set; }
    }
}
