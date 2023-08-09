using Plugin.NFC;
using UTeM_Mobile.Core.IServices;
using UTeM_Mobile.Core.Models;
using UTeM_Mobile.Core.Services;
using UTeM_Mobile.Core.Services.DBServices;
using UTeM_Mobile.Data.Models;
using UTeM_Mobile.Models;
using UTeM_Mobile.PopupViews;
using UTeM_Mobile.StaticProperties;

namespace UTeM_Mobile.Services
{
    public class NFCService
    {
        public static void SubscribeNFC()
        {
            try
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    CrossNFC.Current.OnMessageReceived += Current_OnMessageReceived;
                    CrossNFC.Current.StartListening();
                });
            }
            catch (Exception ex)
            {

            }
        }

        private static async void Current_OnMessageReceived(ITagInfo tagInfo)
        {
            try
            {
                if (tagInfo != null)
                {
                    var record = tagInfo.Records;
                    if (record != null)
                    {
                        //var data = "geo:0,0?q=2.413724,102.13129(Main+gate+2)";
                        //var data = "geo:2.414724,102.13129";
                        var data = record[0].Uri;
                        var first = data.Split('(')[0];
                        var sec = first.Split("q=");
                        string location = sec.Count() > 0 ? sec[1] : sec[0];
                        var latlong = location.Split(",");
                        var lat = latlong[0];
                        var lon = latlong[1];
                    }
                    else
                    {
                        await MainThread.InvokeOnMainThreadAsync(() => 
                            Application.Current.MainPage.Navigation.PushModalAsync(new MessagePopupPage(PopMessage.GetMessage("Scan Error", "Scan Failed", "Appropriate data not found in tag.")))
                        );
                    }
                }
                else
                {
                    await MainThread.InvokeOnMainThreadAsync(() =>
                        Application.Current.MainPage.Navigation.PushModalAsync(new MessagePopupPage(PopMessage.GetMessage("Scan Error", "NFC Error", "NFC data reading failed.")))
                    );
                }
            }
            catch(Exception ex)
            {
                await MainThread.InvokeOnMainThreadAsync(() =>
                    Application.Current.MainPage.Navigation.PushModalAsync(new MessagePopupPage(PopMessage.GetMessage("Scan Error", "NFC Failed", "Unexpected error occured.")))
                );
            }
        }

        public static async Task ExecuteScanAsync(float _latitude, float _longitute)
        {
            try
            {
                if (!await StaticMessage.ShowInternetMessage())
                {
                    return;
                }
                IGenericService<Checkpoint> _genericCheckpointService = new GenericService<Checkpoint>();
                IGenericService<PatrolCheckpoint> _genericPatrolCheckpointService = new GenericService<PatrolCheckpoint>();
                ObjectResponse<Patrol> patrolResponse = null;
                AuthToken token = await LocalDBService.GetToken();
                
                string url = "checkpoints/by-location";
                var body = new
                {
                    latitude = _latitude,
                    longitude = _longitute
                };
                ObjectResponse<Checkpoint> checkpointResponse = await _genericCheckpointService.PostAsync(url, body, token);
                if (checkpointResponse.IsSuccess)
                {
                    Checkpoint checkpoint = checkpointResponse.Data;
                    Patrol patrol = await PatrolDBService.Get();
                    if (patrol == null)
                    {
                        patrolResponse = await PatrolService.GetPatrolStatus();
                        if (patrolResponse.IsSuccess && patrolResponse.Data != null)
                        {
                            patrol = patrolResponse.Data;
                        }
                    }
                    
                    if (patrol != null && patrol.Status == "Started")
                    {
                        string patrolCheckpointUrl = "patrolCheckpoints/mark";
                        var content = new
                        {
                            patrolId = patrol.Id,
                            checkpointId = checkpoint.Id
                        };
                        ObjectResponse<PatrolCheckpoint> patrolCheckpointResponse = await _genericPatrolCheckpointService.PostAsync(patrolCheckpointUrl, content, token);
                        if (patrolCheckpointResponse.IsSuccess)
                        {
                            patrolResponse = await PatrolService.GetPatrolStatus();
                            if (patrolResponse.IsSuccess && patrolResponse.Data != null)
                            {
                                patrol = patrolResponse.Data;
                            }
                            await PatrolDBService.Delete();
                            if (patrol != null)
                            {
                                await PatrolDBService.Insert(patrol);
                                PatrolCheckpoint patrolCheckpoint = patrol.PatrolCheckpoints.FirstOrDefault(p => p.Status == "Scheduled");
                                if (patrolCheckpoint != null)
                                {
                                    await TimerDBService.Delete();
                                    await TimerDBService.Insert(new CheckpointTimer { PatrolId = patrol.Id, CheckpointId = patrolCheckpoint.CheckpointId, CheckpointName = patrolCheckpoint.Checkpoint.Name, ExpectedCheckedTime = patrolCheckpoint.ExpectedCheckedTime });
                                }
                            }
                            await MainThread.InvokeOnMainThreadAsync(() =>
                                Application.Current.MainPage.Navigation.PushModalAsync(new MessagePopupPage(PopMessage.GetMessage("Scan sucessfull", "Checkpoint reached", patrolCheckpointResponse.Message)))
                            );
                        }
                        else
                        {
                            await MainThread.InvokeOnMainThreadAsync(() =>
                                Application.Current.MainPage.Navigation.PushModalAsync(new MessagePopupPage(PopMessage.GetMessage("Scan Error", patrolCheckpointResponse.Message)))
                            );
                        }
                    }
                    else
                    {
                        await MainThread.InvokeOnMainThreadAsync(() =>
                            Application.Current.MainPage.Navigation.PushModalAsync(new MessagePopupPage(PopMessage.GetMessage("Scan sucessfull", "Checkpoint not found", "Patrol not started.")))
                        );
                    }
                }
                else
                {
                    await MainThread.InvokeOnMainThreadAsync(() =>
                        Application.Current.MainPage.Navigation.PushModalAsync(new MessagePopupPage(PopMessage.GetMessage("Scan Error", "Checkpoint failed", "Appropriate data not found in server.")))
                    );
                }
            }
            catch(Exception ex)
            {
                if (!await StaticMessage.ShowInternetMessage())
                {
                    return;
                }
                else
                {
                    await MainThread.InvokeOnMainThreadAsync(() => Application.Current.MainPage.Navigation.PushModalAsync(new MessagePopupPage(PopMessage.GetExceptionMessage("Scan error", "Unexpected error occured in server.")))
                    );
                }
            }
        }
    }
}
