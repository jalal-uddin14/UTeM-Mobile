using UTeM_Mobile.Core.Models;
using UTeM_Mobile.Core.Services;
using UTeM_Mobile.Core.Services.DBServices;
using UTeM_Mobile.Data.Models;
using UTeM_Mobile.PopupViews;
using UTeM_Mobile.StaticProperties;

namespace UTeM_Mobile.Services
{
    public class TimeOutService
    {
        static IDispatcherTimer timer = Application.Current.Dispatcher.CreateTimer();
        static Patrol patrol = null;
        static PatrolDetail patrolDetail = null;
        static Location location = null;


        public static void RunTimer()
        {
            timer.Interval = TimeSpan.FromSeconds(60);
            timer.Tick += async (s, e) =>
            {
                try
                {
                    CheckpointTimer checkpointTimer = await TimerDBService.Get();
                    if (!StaticMessage.InternetNotConnected && checkpointTimer != null && checkpointTimer.ExpectedCheckedTime != null && checkpointTimer.ExpectedCheckedTime.Value <= DateTime.UtcNow.AddHours(8))
                    {
                        await ModalService.PopAllModals();
                        await MainThread.InvokeOnMainThreadAsync(() =>
                        {
                            Application.Current.MainPage.Navigation.PushModalAsync(new TimeoutPopupPage(checkpointTimer));
                        });
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            };
            timer.Start();
        }
        public static async Task RunLocationBroadcastAsync(AuthToken token)
        {
            IDispatcherTimer locationTimer = Application.Current.Dispatcher.CreateTimer();
            locationTimer.Interval = TimeSpan.FromSeconds(10);
            locationTimer.Tick += async (s, e) =>
            {
                if (!StaticMessage.InternetNotConnected)
                {
                    var p = await PatrolDetailDBService.Get();
                    patrolDetail = ConvertModelService.DBPatrolDetailToPatrolDetail(p);
                    if (patrolDetail == null)
                    {
                        var response = await PatrolService.GetPatrolStatus();
                        if (response.IsSuccess && response.Data != null)
                        {
                            patrolDetail = response.Data;
                        }
                    }
                    location = await LocationService.GetCurrentLocationAsync();
                    if (location != null && patrol != null)
                    {
                        var content = new
                        {
                            UserId = token.UserId,
                            PatrolId = patrol.Id,
                            Location = location
                        };
                        if (patrol != null && patrol.Status == "Started")
                        {
                            await PusherLocationService.SendNotificationAsync("UTeM-Guard", "guard.activities." + patrol.Id, content);
                        }
                    }
                }
            };
            locationTimer.Start();
        }

        public static async Task CheckTimerToken()
        {
            try
            {
                AuthToken token = await LocalDBService.GetToken();
                if (token != null && token.ValidTo > DateTime.UtcNow.AddHours(8))
                {
                    ObjectResponse<PatrolDetail> response = await PatrolService.GetPatrolStatus();
                    if (response.IsSuccess && response.Data != null)
                    {
                        PatrolDetail patrolDetail = response.Data;
                        patrol = patrolDetail.Patrol;
                        await PatrolDetailDBService.Delete();
                        await PatrolDetailDBService.Insert(ConvertModelService.PatrolDetailToDbPatrolDetail(patrolDetail));
                        await TimerDBService.Delete();
                        if (patrolDetail.Status == "Completed" || patrolDetail.Status == "Missed")
                        {
                            return;
                        }
                        if (patrolDetail.PatrolCheckpoints != null && patrolDetail.PatrolCheckpoints.Count > 0)
                        {
                            PatrolCheckpoint patrolCheckpoint = patrolDetail.PatrolCheckpoints.FirstOrDefault(p => p.Status == "Scheduled");
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
                        else if (patrolDetail.Patrol.Route != null)
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
                                    ExpectedCheckedTime = DateTime.UtcNow.AddHours(8).AddMinutes(nextCheckpoint.ExpectedTime)
                                };
                                await TimerDBService.Delete();
                                await TimerDBService.Insert(checkpointTimer);
                            }
                        }
                    }
                }
            }
            catch(Exception ex)
            {

            }
        }
    }
}
