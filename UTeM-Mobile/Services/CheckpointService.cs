using System.Text.Json;
using UTeM_Mobile.Core.IServices;
using UTeM_Mobile.Core.Models;
using UTeM_Mobile.Data.Models;
using UTeM_Mobile.Interfaces;

namespace UTeM_Mobile.Services
{
    public class CheckpointService : ICheckpointService
    {
        private readonly ITokenStorageService _tokenService;
        private readonly IGenericService<Checkpoint> _genericCheckpointService;
        private readonly IDialogService _dialogService;
        public CheckpointService(IGenericService<Checkpoint> genericCheckpointService, ITokenStorageService storageService, IDialogService dialogService)
        {
            _genericCheckpointService = genericCheckpointService;
            _tokenService = storageService;
            _dialogService = dialogService;
        }

        public async Task<ObjectResponse<Checkpoint>> MarkCheckpointAsync(Checkpoint passedCheckpoint)
        {
            try
            {
                var tokenJson = await _tokenService.GetAccessTokenAsync();
                if (string.IsNullOrEmpty(tokenJson))
                {
                    return null;
                }
                var token = JsonSerializer.Deserialize<AuthToken>(tokenJson);
                string url = "checkpoints/by-location";
                var currentLocation = await LocationService.GetCurrentLocationAsync();
                var body = new
                {
                    nfcLatitude = passedCheckpoint.Latitude,
                    nfcLongitude = passedCheckpoint.Longitude,
                    currentLatitude = currentLocation.Latitude,
                    currentLongitude = currentLocation.Longitude
                };
                ObjectResponse<Checkpoint> checkpointResponse = await _genericCheckpointService.PostAsync(url, body, token);
                if (checkpointResponse.IsSuccess && checkpointResponse.Data != null)
                {
                    
                }
                else if (!checkpointResponse.IsSuccess && checkpointResponse.Data != null)
                {
                    await _dialogService.ShowAlertAsync("Scan Error", checkpointResponse.Message);
                }
                else
                {
                    await _dialogService.ShowAlertAsync("Scan Error", "Checkpoint failed", "Appropriate data not found in server.");
                }
                return checkpointResponse;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
