using Plugin.NFC;
using UTeM_Mobile.Core.IServices;
using UTeM_Mobile.Core.Models;
using UTeM_Mobile.Core.Services;
using UTeM_Mobile.Data.Models;
using UTeM_Mobile.Models;
using UTeM_Mobile.PopupViews;

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
                IGenericService<Checkpoint> _genericCheckpointService = new GenericService<Checkpoint>();
                IGenericService<Patrol> _genericPatrolService = new GenericService<Patrol>();
                IGenericService<PatrolCheckpoint> _genericPatrolCheckpointService = new GenericService<PatrolCheckpoint>();
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
                    string patrolStatusUrl = "patrols/status";
                    ObjectResponse<Patrol> patrolResponse = await _genericPatrolService.PostAsync(patrolStatusUrl, null, token);
                    if (patrolResponse.IsSuccess && patrolResponse.Data != null && patrolResponse.Data.Status == "Started")
                    {
                        string patrolCheckpointUrl = "patrolCheckpoints/mark";
                        Patrol patrol = patrolResponse.Data;
                        var hasCheckpoint = patrol.Route.RouteCheckpoints.Where(p => p.CheckpointId == checkpoint.Id).FirstOrDefault() != null;
                        if (hasCheckpoint)
                        {
                            var content = new
                            {
                                patrolId = patrol.Id,
                                checkpointId = checkpoint.Id,
                                checkedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                            };
                            ObjectResponse<PatrolCheckpoint> patrolCheckpointResponse = await _genericPatrolCheckpointService.PostAsync(patrolCheckpointUrl, content, token);
                            if (patrolCheckpointResponse.IsSuccess)
                            {
                                PatrolCheckpoint patrolCheckpoint = patrolCheckpointResponse.Data;
                                bool isComplete = true;
                                patrolResponse = await _genericPatrolService.PostAsync(patrolStatusUrl, null, token);
                                patrol = patrolResponse.Data;
                                foreach (var item in patrol.Route.RouteCheckpoints)
                                {
                                    if (patrol.PatrolCheckpoints.Where(p => p.CheckpointId == item.CheckpointId).FirstOrDefault() == null)
                                    {
                                        isComplete = false;
                                        break;
                                    }
                                }
                                if (isComplete)
                                {
                                    string updateStatusUrl = "patrols/update-status";
                                    var updateStatusContent = new
                                    {
                                        Id = patrol.Id,
                                        Status = "Completed",
                                        CompletedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                                    };
                                    ObjectResponse<Patrol> response = await _genericPatrolService.PutAsync(updateStatusUrl, updateStatusContent, token);
                                }
                                await MainThread.InvokeOnMainThreadAsync(() =>
                                    Application.Current.MainPage.Navigation.PushModalAsync(new MessagePopupPage(PopMessage.GetMessage("Scan sucessfull", "Checkpoint reached", checkpoint.Name + " checkpoint successfully scaned.")))
                                );
                            }
                            else
                            {
                                await MainThread.InvokeOnMainThreadAsync(() =>
                                    Application.Current.MainPage.Navigation.PushModalAsync(new MessagePopupPage(PopMessage.GetMessage("Scan sucessfull", "Checkpoint write error", checkpoint.Name + " checkpoint failed to mark as scanned.")))
                                );
                            }
                        }
                        else
                        {
                            await MainThread.InvokeOnMainThreadAsync(() =>
                                Application.Current.MainPage.Navigation.PushModalAsync(new MessagePopupPage(PopMessage.GetMessage("Scan sucessfull", "Checkpoint not found", checkpoint.Name + " checkpoint not in this patrol.")))
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
                await MainThread.InvokeOnMainThreadAsync(() => 
                    Application.Current.MainPage.Navigation.PushModalAsync(new MessagePopupPage(PopMessage.GetExceptionMessage("Scan error", "Unexpected error occured in server.")))
                );
            }
        }
    }
}
