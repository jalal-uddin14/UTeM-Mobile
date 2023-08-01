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
                DateTime now = DateTime.UtcNow.AddHours(8);
                string difference = "";
                var diff = now.Subtract(Date);
                difference += diff.Days > 0 ? diff.Days + "d " : "";
                difference += diff.Hours > 0 ? diff.Hours + "h " : diff.Days > 0 ? diff.Hours + "h " : "";
                difference += diff.Minutes + "m";
                return difference;
            }
        }
    }
}
