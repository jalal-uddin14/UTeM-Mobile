using System;

namespace UTeM_Mobile.Data.Models
{
    public class Report
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public string File { get; set; }
        public string FilePath { get; set; }
        public int PatrolId { get; set; }
        public virtual Patrol Patrol { get; set; }
        public string GuardName
        {
            get
            {
                return Patrol != null && Patrol.Guard != null ? Patrol.Guard.Name : "";
            }
        }
        public string RouteName
        {
            get
            {
                return Patrol != null && Patrol.Route != null ? Patrol.Route.Name : "";
            }
        }
        public virtual string Difference
        {
            get
            {
                DateTime now = DateTime.Now;
                var diff = now.Subtract(Date);
                var days = diff.Days;
                var hours = diff.Hours;
                var minutes = diff.Minutes;
                return string.Format("{0}d {1}h {2}m", days, diff.Hours, diff.Minutes);
            }
        }
    }
}
