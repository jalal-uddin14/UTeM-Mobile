using UTeM_Mobile.Core.Models;
using UTeM_Mobile.Core.Services;
using UTeM_Mobile.Core.Services.DBServices;
using UTeM_Mobile.Data.Models;
using UTeM_Mobile.Models;
using UTeM_Mobile.PopupViews;
using UTeM_Mobile.StaticProperties;

namespace UTeM_Mobile.Services
{
    public class TimeOutService
    {
        static IDispatcherTimer patrolDetailTimer = Application.Current.Dispatcher.CreateTimer();
        static IDispatcherTimer patrolTimer = Application.Current.Dispatcher.CreateTimer();
        static IDispatcherTimer shiftTimer = Application.Current.Dispatcher.CreateTimer();
        static IDispatcherTimer locationTimer = Application.Current.Dispatcher.CreateTimer();
        static Patrol patrol = null;
        static PatrolDetail patrolDetail = null;
        static PatrolDetail timerPatrolDetail = null;
        static Location location = null;


        public static void RunPatrolDetailTimer(AuthToken token, ApplicationUser user)
        {
            patrolDetailTimer.Interval = TimeSpan.FromSeconds(60);
            patrolDetailTimer.Tick += async (s, e) =>
            {
                try
                {
                    if (!StaticMessage.InternetNotConnected && StaticCredentials.CheckpointTimer != null && StaticCredentials.CheckpointTimer.ExpectedCheckedTime != null)
                    {
                        if (StaticCredentials.CheckpointTimer.ExpectedCheckedTime.Value <= DateTime.UtcNow.AddHours(8).AddMinutes(5) && StaticCredentials.CheckpointTimer.ExpectedCheckedTime.Value >= DateTime.UtcNow.AddHours(8).AddMinutes(4))
                        {
                            await MainThread.InvokeOnMainThreadAsync(async () =>
                            {
                                if (Application.Current.MainPage.Navigation.ModalStack.Count <= 0)
                                {
                                    await MainThread.InvokeOnMainThreadAsync(() =>
                                        Application.Current.MainPage.Navigation.PushModalAsync(new MessagePopupPage(PopMessage.GetMessage("Patrol Notification", "Patrol Time Expiring.", string.Format("You have 5 minutes to reach next checkpoint."))))
                                    );
                                }
                            });
                        }
                        if (StaticCredentials.CheckpointTimer.ExpectedCheckedTime.Value <= DateTime.UtcNow.AddHours(8))
                        {
                            if (!StaticCredentials.IsNotificationSend)
                            {
                                await PusherLocationService.SendNotificationAsync("UTeM-Guard", "guard.patrol-activities." + user.SupervisorId, new
                                {
                                    UserId = token.UserId,
                                    PatrolDetailId = patrolDetail.Id,
                                    User = user,
                                });
                                StaticCredentials.IsNotificationSend = true;
                            }

                            await MainThread.InvokeOnMainThreadAsync(async () =>
                            {
                                if (Application.Current.MainPage.Navigation.ModalStack.Count <= 0)
                                {
                                    await Application.Current.MainPage.Navigation.PushModalAsync(new TimeoutPopupPage(StaticCredentials.CheckpointTimer, "Do you need more time?"));
                                }
                            });
                        }
                        
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
                    if (!StaticMessage.InternetNotConnected && StaticCredentials.NextPatrolDetail != null && StaticCredentials.NextPatrolDetail.Start <= DateTime.UtcNow.AddHours(8).AddMinutes(10))
                    {
                        await MainThread.InvokeOnMainThreadAsync(async () =>
                        {
                            if (!StaticCredentials.PatrolNotificationShowed)
                            {
                                await Application.Current.MainPage.Navigation.PushModalAsync(new NextPatrolPopupPage(StaticCredentials.NextPatrolDetail));
                                StaticCredentials.PatrolNotificationShowed = true;
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
        public static void RunShiftTimer()
        {
            shiftTimer.Interval = TimeSpan.FromSeconds(60);
            shiftTimer.Tick += async (s, e) =>
            {
                try
                {
                    if (!StaticMessage.InternetNotConnected && StaticCredentials.Patrol != null && StaticCredentials.Patrol.Shift.End <= DateTime.UtcNow.AddHours(8).AddMinutes(5).TimeOfDay)
                    {
                        await MainThread.InvokeOnMainThreadAsync(async () =>
                        {
                            if (!StaticCredentials.ShiftNotificationShowed)
                            {
                                await Application.Current.MainPage.Navigation.PushModalAsync(new MessagePopupPage(PopMessage.GetMessage("Shift Notification", "Shift Time Expiring.", string.Format("Your shift will end in 5 minutes."))));
                                StaticCredentials.ShiftNotificationShowed = true;
                            }
                        });
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            };
            shiftTimer.Start();
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
                            PatrolDetailId = patrolDetail.Id,
                            Location = location
                        };
                        if (patrol != null && patrol.Status == "Started")
                        {
                            await PusherLocationService.SendNotificationAsync("UTeM-Guard", "guard.activities." + patrolDetail.Id, content);
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
                            if (StaticCredentials.NextPatrolDetail == null)
                            {
                                patrol = await PatrolService.GetPatrol(patrolDetail.PatrolId);
                                if (patrol != null)
                                {
                                    StaticCredentials.Patrol = patrol;
                                    PatrolDetail nextPatrolDetail = patrol.PatrolDetails.Where(p => p.Status == "Scheduled" && p.Start > DateTime.UtcNow.AddHours(8)).FirstOrDefault();
                                    if (nextPatrolDetail != null)
                                    {
                                        StaticCredentials.NextPatrolDetail = nextPatrolDetail;
                                    }
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
