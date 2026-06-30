using Plugin.NFC;
using UTeM_Mobile.Core.IServices;
using UTeM_Mobile.Core.Models;
using UTeM_Mobile.Core.Services;
using UTeM_Mobile.Core.Services.DBServices;
using UTeM_Mobile.Data.Models;
using UTeM_Mobile.Interfaces;
using UTeM_Mobile.Models;
using UTeM_Mobile.PopupViews;
using UTeM_Mobile.StaticProperties;

namespace UTeM_Mobile.Services
{
    public class NFCService : INFCService
    {
        private readonly IPatrolService _patrolService;
        private readonly ICheckpointService _checkpointService;
        private readonly IDialogService _dialogService;

        private bool _isSubscribed;
        private bool _isScanning;

        public NFCService(IPatrolService patrolService, ICheckpointService checkpointService, IDialogService dialogService)
        {
            _dialogService = dialogService;
            _patrolService = patrolService;
            _checkpointService = checkpointService;
        }

        public async Task InitializeAsync()
        {
            if (!CrossNFC.Current.IsAvailable)
            {
                await _dialogService.ShowAlertAsync(
                    "NFC",
                    "NFC is not available in your phone.");
                return;
            }

            if (!CrossNFC.Current.IsEnabled)
            {
                await _dialogService.ShowAlertAsync(
                    "NFC",
                    "Please turn on NFC.");
                return;
            }

            SubscribeNFC();
        }

        public void SubscribeNFC()
        {
            try
            {
                if (_isSubscribed)
                    return;

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    CrossNFC.Current.OnMessageReceived -= Current_OnMessageReceived;
                    CrossNFC.Current.OnMessageReceived += Current_OnMessageReceived;

                    CrossNFC.Current.StartListening();

                    _isSubscribed = true;
                });
            }
            catch (Exception)
            {

            }
        }

        public void UnsubscribeNFC()
        {
            if (!_isSubscribed)
                return;

            MainThread.BeginInvokeOnMainThread(() =>
            {
                CrossNFC.Current.OnMessageReceived -= Current_OnMessageReceived;
                CrossNFC.Current.StopListening();

                _isSubscribed = false;
            });
        }

        private async void Current_OnMessageReceived(ITagInfo tagInfo)
        {
            try
            {
                var checkpoint = TryReadCheckpoint(tagInfo);

                if (checkpoint is null)
                {
                    await _dialogService.ShowAlertAsync(
                        "Scan Error",
                        "Appropriate data not found in tag.");
                    return;
                }

                await ExecuteScanAsync(checkpoint);
            }
            catch
            {
                await _dialogService.ShowAlertAsync(
                    "Scan Error",
                    "Unexpected error occurred while reading NFC.");
            }
        }

        private Checkpoint? TryReadCheckpoint(ITagInfo tagInfo)
        {
            if (tagInfo?.Records is null || tagInfo.Records.Length == 0)
                return null;

            var uri = tagInfo.Records[0].Uri;

            if (string.IsNullOrWhiteSpace(uri))
                return null;

            // Example: geo:0,0?q=2.413724,102.13129(Main gate)
            var locationPart = uri.Split('(')[0];
            var queryPart = locationPart.Split("q=");

            var location = queryPart.Length > 1
                ? queryPart[1]
                : queryPart[0];

            var latLong = location.Split(',');

            if (latLong.Length < 2)
                return null;

            if (!double.TryParse(latLong[0], out var latitude))
                return null;

            if (!double.TryParse(latLong[1], out var longitude))
                return null;

            return new Checkpoint
            {
                Latitude = latitude,
                Longitude = longitude
            };
        }

        public async Task ExecuteScanAsync(Checkpoint passedCheckpoint)
        {
            if (_isScanning)
            {
                await _dialogService.ShowAlertAsync(
                    "NFC Scan",
                    "NFC scan is already processing.");
                return;
            }

            try
            {
                _isScanning = true;

                var result = await _checkpointService.MarkCheckpointAsync(passedCheckpoint);

                if (!result.IsSuccess)
                {
                    await _dialogService.ShowAlertAsync(
                        "Scan Error",
                        result.Message ?? "Checkpoint scan failed.");
                    return;
                }

                await _dialogService.ShowAlertAsync(
                    "Scan Successful",
                    result.Message ?? "Checkpoint reached.");
            }
            catch
            {
                await _dialogService.ShowAlertAsync(
                    "Scan Error",
                    "Unexpected error occurred while scanning checkpoint.");
            }
            finally
            {
                _isScanning = false;
            }
        }

        public async Task ExecuteScanAsync1(Checkpoint passedCheckpoint)
        {
            try
            {
                //if (!StaticCredentials.IsScanning)
                //{
                //    StaticCredentials.IsScanning = true;
                //}
                //else if (StaticCredentials.IsScanning)
                //{
                //    await Application.Current.MainPage.Navigation.PopModalAsync();
                //    await MainThread.InvokeOnMainThreadAsync(() =>
                //        Application.Current.MainPage.Navigation.PushModalAsync(new MessagePopupPage(PopMessage.GetMessage("Scan Info", "NFC Scan", "NFC scan processing.")))
                //    );
                //    return;
                //}
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
                    nfcLatitude = passedCheckpoint.Latitude,
                    nfcLongitude = passedCheckpoint.Longitude,
                    currentLatitude = location.Latitude,
                    currentLongitude = location.Longitude
                    //currentLatitude = passedCheckpoint.Latitude,
                    //currentLongitude = passedCheckpoint.Longitude
                };
                ObjectResponse<Checkpoint> checkpointResponse = await _genericCheckpointService.PostAsync(url, body, token);
                if (checkpointResponse.IsSuccess && checkpointResponse.Data != null)
                {
                    Checkpoint checkpoint = checkpointResponse.Data;
                    PatrolDetail patrolDetail = StaticCredentials.PatrolDetail;
                    if (patrolDetail == null)
                    {
                        patrolResponse = await _patrolService.GetPatrolStatus();
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
                            latitude = passedCheckpoint.Latitude,
                            longitude = passedCheckpoint.Longitude
                        };
                        ObjectResponse<PatrolCheckpoint> patrolCheckpointResponse = await _genericPatrolCheckpointService.PostAsync(patrolCheckpointUrl, content, token);
                        if (patrolCheckpointResponse.IsSuccess && patrolCheckpointResponse.Data == null)
                        {
                            StaticCredentials.PatrolDetail = null;
                            StaticCredentials.CheckpointTimer = null;
                            StaticCredentials.NextPatrolDetail = null;
                            await MainThread.InvokeOnMainThreadAsync(() =>
                                Application.Current.MainPage.Navigation.PushModalAsync(new MessagePopupPage(PopMessage.GetMessage("Scan sucessful", "Checkpoint reached", patrolCheckpointResponse.Message)))
                            );
                        }
                        else if (patrolCheckpointResponse.IsSuccess && patrolCheckpointResponse.Data != null)
                        {
                            patrolResponse = await _patrolService.GetPatrolStatus();
                            if (patrolResponse.IsSuccess && patrolResponse.Data != null)
                            {
                                patrolDetail = patrolResponse.Data;
                            }
                            await PatrolDBService.Delete();
                            if (patrolDetail != null)
                            {
                                StaticCredentials.PatrolDetail = patrolDetail;
                                PatrolCheckpoint patrolCheckpoint = patrolDetail.PatrolCheckpoints.FirstOrDefault(p => p.Status == "Scheduled");
                                if (patrolCheckpoint != null)
                                {
                                    StaticCredentials.CheckpointTimer = new CheckpointTimer { PatrolId = patrolDetail.Id, CheckpointId = patrolCheckpoint.CheckpointId, CheckpointName = patrolCheckpoint.Checkpoint.Name, ExpectedCheckedTime = patrolCheckpoint.ExpectedCheckedTime };
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
                            Application.Current.MainPage.Navigation.PushModalAsync(new MessagePopupPage(PopMessage.GetMessage("Scan sucessful", "Wrong checkpoint", "Patrol not started.")))
                        );
                    }
                }
                else if (!checkpointResponse.IsSuccess && checkpointResponse.Data != null)
                {
                    await Application.Current.MainPage.Navigation.PushModalAsync(new MessagePopupPage(PopMessage.GetMessage("Scan Error", "Checkpoint failed", checkpointResponse.Message)));
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
            finally
            {
                StaticCredentials.IsScanning = false;
            }
        }
    }
}
