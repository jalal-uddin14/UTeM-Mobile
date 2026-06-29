using UTeM_Mobile.Data.Models;

namespace UTeM_Mobile.Interfaces
{
    public interface INFCService
    {
        void SubscribeNFC();
        Task InitializeAsync();
        Task ExecuteScanAsync(Checkpoint passedCheckpoint);
    }
}
