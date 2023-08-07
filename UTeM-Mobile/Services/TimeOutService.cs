using UTeM_Mobile.Core.Models;
using UTeM_Mobile.Core.Services.DBServices;
using UTeM_Mobile.Data.Models;
using UTeM_Mobile.PopupViews;

namespace UTeM_Mobile.Services
{
    public class TimeOutService
    {
        static IDispatcherTimer timer = Application.Current.Dispatcher.CreateTimer();
        public static void RunTimer()
        {
            timer.Interval = TimeSpan.FromSeconds(30);
            timer.Tick += async (s, e) =>
            {
                CheckpointTimer checkpointTimer = await TimerDBService.Get();
                if (checkpointTimer != null && checkpointTimer.ExpectedCheckedTime != null && checkpointTimer.ExpectedCheckedTime.Value.AddHours(-2) <= DateTime.Now)
                {
                    await MainThread.InvokeOnMainThreadAsync(() =>
                    {
                        ModalService.PopAllModals();
                        Application.Current.MainPage.Navigation.PushModalAsync(new TimeoutPopupPage(checkpointTimer));
                    });
                }
            };
            timer.Start();
        }
        public static void StopTimer()
        {
            timer.Stop();
        }

        public static async Task CheckTimerToken()
        {
            AuthToken token = await LocalDBService.GetToken();
            if (token != null && token.IsRemember && token.ValidTo > DateTime.Now)
            {
                ObjectResponse<Patrol> response = await PatrolService.GetPatrolStatus();
                if (response.IsSuccess && response.Data != null)
                {
                    Patrol patrol = response.Data;
                    await PatrolDBService.Delete();
                    await PatrolDBService.Insert(patrol);
                    if (patrol.PatrolCheckpoints != null && patrol.PatrolCheckpoints.Count > 0)
                    {
                        PatrolCheckpoint patrolCheckpoint = patrol.PatrolCheckpoints.FirstOrDefault(p => p.Status == "Scheduled");
                        if (patrolCheckpoint != null)
                        {
                            CheckpointTimer checkpointTimer = new CheckpointTimer
                            {
                                PatrolId = patrolCheckpoint.PatrolId,
                                CheckpointId = patrolCheckpoint.CheckpointId,
                                CheckpointName = patrolCheckpoint.Checkpoint.Name,
                                ExpectedCheckedTime = patrolCheckpoint.ExpectedCheckedTime
                            };
                            await TimerDBService.Delete();
                            await TimerDBService.Insert(checkpointTimer);
                        }
                    }
                    else if(patrol.Route != null)
                    {
                        RouteCheckpoint nextCheckpoint = patrol.Route.RouteCheckpoints
                            .FirstOrDefault(routeCheckpoints => !patrol.PatrolCheckpoints.Any(patrolCheckpoints => patrolCheckpoints.CheckpointId == routeCheckpoints.CheckpointId));
                        if (nextCheckpoint != null)
                        {
                            CheckpointTimer checkpointTimer = new CheckpointTimer
                            {
                                PatrolId = patrol.Id,
                                CheckpointId = nextCheckpoint.CheckpointId,
                                CheckpointName = nextCheckpoint.Checkpoint.Name,
                                ExpectedCheckedTime = DateTime.Now.AddHours(-2).AddMinutes(nextCheckpoint.ExpectedTime)
                            };
                            await TimerDBService.Delete();
                            await TimerDBService.Insert(checkpointTimer);
                        }
                    }
                }
            }
        }
    }
}
