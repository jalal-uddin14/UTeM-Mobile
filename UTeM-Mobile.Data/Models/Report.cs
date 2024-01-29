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
        public int PatrolCheckpointId { get; set; }
        public virtual PatrolCheckpoint PatrolCheckpoint { get; set; }
        public string GuardName
        {
            get
            {
                return PatrolCheckpoint != null && PatrolCheckpoint.PatrolDetail != null && PatrolCheckpoint.PatrolDetail.Patrol != null && PatrolCheckpoint.PatrolDetail.Patrol.Guard != null ? PatrolCheckpoint.PatrolDetail.Patrol.Guard.Name : "";
            }
        }
        public string? RouteName
        {
            get
            {
                return PatrolCheckpoint != null && PatrolCheckpoint.PatrolDetail != null && PatrolCheckpoint.PatrolDetail.Patrol != null && PatrolCheckpoint.PatrolDetail.Patrol.Route != null ? PatrolCheckpoint.PatrolDetail.Patrol.Route.Name : "";
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
