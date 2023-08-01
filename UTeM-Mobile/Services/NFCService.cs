using Plugin.NFC;
using UTeM_Mobile.Core.IServices;
using UTeM_Mobile.Core.Models;
using UTeM_Mobile.Core.Services;
using UTeM_Mobile.Data.Models;
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
                        Dictionary<string, string> popupContent = new Dictionary<string, string>
                                    {
                                        { "Heading", "Scan Error" },
                                        { "Title", "Scan failed" },
                                        { "Message", "Appropriate data not found in tag." },
                                        { "NavigateTo", "ReportListPage" },
                                        { "HasNavigate", "false" }
                                    };
                        await MainThread.InvokeOnMainThreadAsync(() => Application.Current.MainPage.Navigation.PushModalAsync(new MessagePopupPage(popupContent)));
                    }
                }
                else
                {
                    Dictionary<string, string> popupContent = new Dictionary<string, string>
                                    {
                                        { "Heading", "Scan Error" },
                                        { "Title", "Scan failed" },
                                        { "Message", "NFC data reading failed." },
                                        { "NavigateTo", "ReportListPage" },
                                        { "HasNavigate", "false" }
                                    };
                    await MainThread.InvokeOnMainThreadAsync(() => Application.Current.MainPage.Navigation.PushModalAsync(new MessagePopupPage(popupContent)));
                }
            }
            catch(Exception ex)
            {
                Dictionary<string, string> popupContent = new Dictionary<string, string>
                                    {
                                        { "Heading", "Scan Error" },
                                        { "Title", "Scan failed" },
                                        { "Message", "Unexpected error occured." },
                                        { "NavigateTo", "ReportListPage" },
                                        { "HasNavigate", "false" }
                                    };
                await MainThread.InvokeOnMainThreadAsync(() => Application.Current.MainPage.Navigation.PushModalAsync(new MessagePopupPage(popupContent)));
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
                                Dictionary<string, string> popupContent = new Dictionary<string, string>
                                        {
                                            { "Heading", "Scan sucessfull" },
                                            { "Title", "Checkpoint reached" },
                                            { "Message", checkpoint.Name + " checkpoint successfully scaned." },
                                            { "NavigateTo", "ReportListPage" },
                                            { "HasNavigate", "false" }
                                        };
                                await MainThread.InvokeOnMainThreadAsync(() => Application.Current.MainPage.Navigation.PushModalAsync(new MessagePopupPage(popupContent)));
                            }
                            else
                            {
                                Dictionary<string, string> popupContent = new Dictionary<string, string>
                                        {
                                            { "Heading", "Scan sucessfull" },
                                            { "Title", "Checkpoint write error" },
                                            { "Message", checkpoint.Name + " checkpoint failed to mark as scanned." },
                                            { "NavigateTo", "ReportListPage" },
                                            { "HasNavigate", "false" }
                                        };
                                await MainThread.InvokeOnMainThreadAsync(() => Application.Current.MainPage.Navigation.PushModalAsync(new MessagePopupPage(popupContent)));
                            }
                        }
                        else
                        {
                            Dictionary<string, string> popupContent = new Dictionary<string, string>
                                    {
                                        { "Heading", "Scan sucessfull" },
                                        { "Title", "Checkpoint not found" },
                                        { "Message", checkpoint.Name + " checkpoint not in this patrol." },
                                        { "NavigateTo", "ReportListPage" },
                                        { "HasNavigate", "false" }
                                    };
                            await MainThread.InvokeOnMainThreadAsync(() => Application.Current.MainPage.Navigation.PushModalAsync(new MessagePopupPage(popupContent)));
                        }
                    }
                    else
                    {
                        Dictionary<string, string> popupContent = new Dictionary<string, string>
                                    {
                                        { "Heading", "Scan sucessfull" },
                                        { "Title", "Checkpoint not found" },
                                        { "Message", "Patrol not started." },
                                        { "NavigateTo", "ReportListPage" },
                                        { "HasNavigate", "false" }
                                    };
                        await MainThread.InvokeOnMainThreadAsync(() => Application.Current.MainPage.Navigation.PushModalAsync(new MessagePopupPage(popupContent)));
                    }
                }
                else
                {
                    Dictionary<string, string> popupContent = new Dictionary<string, string>
                                    {
                                        { "Heading", "Scan Error" },
                                        { "Title", "Scan failed" },
                                        { "Message", "Appropriate data not found in server." },
                                        { "NavigateTo", "ReportListPage" },
                                        { "HasNavigate", "false" }
                                    };
                    await MainThread.InvokeOnMainThreadAsync(() => Application.Current.MainPage.Navigation.PushModalAsync(new MessagePopupPage(popupContent)));
                }
            }
            catch(Exception ex)
            {
                Dictionary<string, string> popupContent = new Dictionary<string, string>
                                    {
                                        { "Heading", "Scan Error" },
                                        { "Title", "Server error." },
                                        { "Message", "Unexpected error occured in server." },
                                        { "NavigateTo", "ReportListPage" },
                                        { "HasNavigate", "false" }
                                    };
                await MainThread.InvokeOnMainThreadAsync(() => Application.Current.MainPage.Navigation.PushModalAsync(new MessagePopupPage(popupContent)));
            }
        }
    }
}
