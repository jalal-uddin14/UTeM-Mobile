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
            catch (Exception)
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
                        var lat = Convert.ToDouble(latlong[0]);
                        var lon = Convert.ToDouble(latlong[1]);
                        await ExecuteScanAsync(lat, lon);
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
            catch(Exception)
            {
                await MainThread.InvokeOnMainThreadAsync(() =>
                    Application.Current.MainPage.Navigation.PushModalAsync(new MessagePopupPage(PopMessage.GetMessage("Scan Error", "NFC Failed", "Unexpected error occured.")))
                );
            }
        }

        public static async Task ExecuteScanAsync(double _latitude, double _longitute)
        {
            try
            {
                var location = await LocationService.GetCurrentLocationAsync();
                if (!await StaticMessage.ShowInternetMessage())
                {
                    return;
                }
                IGenericService<Checkpoint> _genericCheckpointService = new GenericService<Checkpoint>();
                IGenericService<PatrolCheckpoint> _genericPatrolCheckpointService = new GenericService<PatrolCheckpoint>();
                ObjectResponse<PatrolDetail> patrolResponse = null;
                AuthToken token = await LocalDBService.GetToken();
                
                string url = "checkpoints/by-location";
                var body = new
                {
                    nfcLatitude = _latitude,
                    nfcLongitude = _longitute,
                    currentLatitude = location.Latitude,
                    currentLongitude = location.Longitude
                };
                ObjectResponse<Checkpoint> checkpointResponse = await _genericCheckpointService.PostAsync(url, body, token);
                if (checkpointResponse.IsSuccess && checkpointResponse.Data != null)
                {
                    Checkpoint checkpoint = checkpointResponse.Data;
                    var p = await PatrolDetailDBService.Get();
                    PatrolDetail patrolDetail = ConvertModelService.DBPatrolDetailToPatrolDetail(p);
                    if (patrolDetail == null)
                    {
                        patrolResponse = await PatrolService.GetPatrolStatus();
                        if (patrolResponse.IsSuccess && patrolResponse.Data != null)
                        {
                            patrolDetail = patrolResponse.Data;
                        }
                    }
                    
                    if (patrolDetail != null && patrolDetail.Status == "Started")
                    {
                        string patrolCheckpointUrl = "patrolCheckpoints/mark";
                        var content = new
                        {
                            patrolDetailId = patrolDetail.Id,
                            checkpointId = checkpoint.Id,
                            latitude = _latitude,
                            longitude = _longitute
                        };
                        ObjectResponse<PatrolCheckpoint> patrolCheckpointResponse = await _genericPatrolCheckpointService.PostAsync(patrolCheckpointUrl, content, token);
                        if (patrolCheckpointResponse.IsSuccess)
                        {
                            patrolResponse = await PatrolService.GetPatrolStatus();
                            if (patrolResponse.IsSuccess && patrolResponse.Data != null)
                            {
                                patrolDetail = patrolResponse.Data;
                            }
                            await PatrolDBService.Delete();
                            if (patrolDetail != null)
                            {
                                await PatrolDetailDBService.Insert(ConvertModelService.PatrolDetailToDbPatrolDetail(patrolDetail));
                                PatrolCheckpoint patrolCheckpoint = patrolDetail.PatrolCheckpoints.FirstOrDefault(p => p.Status == "Scheduled");
                                if (patrolCheckpoint != null)
                                {
                                    await TimerDBService.Delete();
                                    await TimerDBService.Insert(new CheckpointTimer { PatrolId = patrolDetail.Id, CheckpointId = patrolCheckpoint.CheckpointId, CheckpointName = patrolCheckpoint.Checkpoint.Name, ExpectedCheckedTime = patrolCheckpoint.ExpectedCheckedTime });
                                }
                            }
                            await MainThread.InvokeOnMainThreadAsync(() =>
                                Application.Current.MainPage.Navigation.PushModalAsync(new MessagePopupPage(PopMessage.GetMessage("Scan sucessful", "Checkpoint reached", patrolCheckpointResponse.Message)))
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
                            Application.Current.MainPage.Navigation.PushModalAsync(new MessagePopupPage(PopMessage.GetMessage("Scan sucessful", "Checkpoint not found", "Patrol not started.")))
                        );
                    }
                }
                else if (!checkpointResponse.IsSuccess && checkpointResponse.Data != null)
                {
                    await Application.Current.MainPage.Navigation.PushModalAsync(new MessagePopupPage(PopMessage.GetMessage("Scan sucessful", "Checkpoint reached", checkpointResponse.Message)));
                }
                else
                {
                    await MainThread.InvokeOnMainThreadAsync(() => Application.Current.MainPage.Navigation.PushModalAsync(new MessagePopupPage(PopMessage.GetMessage("Scan Error", "Checkpoint failed", "Appropriate data not found in server.")))
                    );
                }
            }
            catch(Exception)
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
