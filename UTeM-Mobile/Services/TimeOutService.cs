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
        static IDispatcherTimer patrolDetailTimer = Application.Current.Dispatcher.CreateTimer();
        static IDispatcherTimer patrolTimer = Application.Current.Dispatcher.CreateTimer();
        static IDispatcherTimer locationTimer = Application.Current.Dispatcher.CreateTimer();
        static Patrol patrol = null;
        static PatrolDetail patrolDetail = null;
        static Location location = null;


        public static void RunPatrolDetailTimer()
        {
            patrolDetailTimer.Interval = TimeSpan.FromSeconds(30);
            patrolDetailTimer.Tick += async (s, e) =>
            {
                try
                {
                    if (!StaticMessage.InternetNotConnected && StaticCredentials.CheckpointTimer != null && StaticCredentials.CheckpointTimer.ExpectedCheckedTime != null && StaticCredentials.CheckpointTimer.ExpectedCheckedTime.Value <= DateTime.UtcNow.AddHours(8))
                    {
                        await MainThread.InvokeOnMainThreadAsync(async () =>
                        {
                            if (Application.Current.MainPage.Navigation.ModalStack.Count <= 0)
                            {
                                await Application.Current.MainPage.Navigation.PushModalAsync(new TimeoutPopupPage(StaticCredentials.CheckpointTimer));
                            }
                        });
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            };
            patrolDetailTimer.Start();
        }

        public static void RunPatrolTimer()
        {
            patrolTimer.Interval = TimeSpan.FromSeconds(60);
            patrolTimer.Tick += async (s, e) =>
            {
                try
                {
                    if (!StaticMessage.InternetNotConnected && StaticCredentials.NextPatrol != null && StaticCredentials.NextPatrol.Start <= DateTime.UtcNow.AddHours(8).AddMinutes(10))
                    {
                        await MainThread.InvokeOnMainThreadAsync(async () =>
                        {
                            if (Application.Current.MainPage.Navigation.ModalStack.Count <= 0)
                            {
                                await Application.Current.MainPage.Navigation.PushModalAsync(new NextPatrolPopupPage(StaticCredentials.NextPatrol));
                            }
                        });
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            };
            patrolTimer.Start();
        }
        public static async Task RunLocationBroadcastAsync(AuthToken token)
        {
            locationTimer.Interval = TimeSpan.FromSeconds(10);
            locationTimer.Tick += async (s, e) =>
            {
                if (!StaticMessage.InternetNotConnected)
                {
                    patrolDetail = StaticCredentials.PatrolDetail;
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
                        StaticCredentials.PatrolDetail = patrolDetail;
                        StaticCredentials.CheckpointTimer = null;
                        if (patrolDetail.Status == "Completed" || patrolDetail.Status == "Missed")
                        {
                            return;
                        }
                        else if(patrolDetail.Status == "Started")
                        {
                            if (StaticCredentials.NextPatrol == null)
                            {
                                patrol = await PatrolService.GetPatrolDetail(patrolDetail.PatrolId);
                                PatrolDetail nextPatrolDetail = patrol.PatrolDetails.Where(p => p.Status == "Scheduled" && p.Start > patrolDetail.Start).FirstOrDefault();
                                if (nextPatrolDetail != null)
                                {
                                    StaticCredentials.NextPatrol = nextPatrolDetail;
                                }
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
                                    StaticCredentials.CheckpointTimer = checkpointTimer;
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
                                    StaticCredentials.CheckpointTimer = checkpointTimer;
                                }
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
