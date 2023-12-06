using UTeM_Mobile.Core.Models;
using UTeM_Mobile.Data.Models;

namespace UTeM_Mobile.Services
{
    public static class ConvertModelService
    {
        public static DBPatrolDetail PatrolDetailToDbPatrolDetail(PatrolDetail patrolDetail)
        {
            return new DBPatrolDetail
            {
                Id = patrolDetail.Id,
                Start = patrolDetail.Start,
                StartedAt = patrolDetail.StartedAt,
                CompletedAt = patrolDetail.CompletedAt,
                Remarks = patrolDetail.Remarks,
                Status = patrolDetail.Status,
                PatrolId = patrolDetail.PatrolId,
                IsStarted = patrolDetail.IsStarted,
                IsScheduled = patrolDetail.IsScheduled,
                IsCompleted = patrolDetail.IsCompleted,
                IsMissed = patrolDetail.IsMissed
            };
        }

        public static PatrolDetail DBPatrolDetailToPatrolDetail(DBPatrolDetail patrolDetail)
        {
            return new PatrolDetail 
            { 
                Id = patrolDetail.Id, 
                Start = patrolDetail.Start, 
                StartedAt = patrolDetail.StartedAt, 
                CompletedAt = patrolDetail.CompletedAt, 
                Remarks = patrolDetail.Remarks, 
                Status = patrolDetail.Status,
                PatrolId = patrolDetail.PatrolId
            };
        }

        public static DBPatrol PatrolToDBPatrol(Patrol patrol)
        {
            return new DBPatrol
            {
                Id = patrol.Id,
                TimerEnabled = patrol.TimerEnabled,
                Start = patrol.Start,
                End = patrol.End,
                Status = patrol.Status,
                Remarks = patrol.Remarks,
                StartedAt = patrol.StartedAt,
                CompletedAt = patrol.CompletedAt,
                GuardId = patrol.GuardId,
                ShiftId = patrol.ShiftId,
                RouteId = patrol.RouteId,
                TimeScheduleId = patrol.TimeScheduleId,
                IsStarted = patrol.IsStarted,
                IsMissed = patrol.IsMissed,
                IsCompleted = patrol.IsCompleted,
                IsScheduled = patrol.IsScheduled
            };
        }

        public static Patrol DBPatrolToPatrol(DBPatrol patrol)
        {
            return new Patrol
            {
                Id = patrol.Id,
                TimerEnabled = patrol.TimerEnabled,
                Start = patrol.Start,
                End = patrol.End,
                Status = patrol.Status,
                Remarks = patrol.Remarks,
                StartedAt = patrol.StartedAt,
                CompletedAt = patrol.CompletedAt,
                GuardId = patrol.GuardId,
                ShiftId = patrol.ShiftId,
                RouteId = patrol.RouteId,
                TimeScheduleId = patrol.TimeScheduleId
            };
        }
    }
}
