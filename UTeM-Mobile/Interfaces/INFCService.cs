using UTeM_Mobile.Data.Models;

namespace UTeM_Mobile.Interfaces
{
    public interface INFCService
    {
        Task ExecuteScanAsync(Checkpoint passedCheckpoint);
        void SubscribeNFC();
    }
}
