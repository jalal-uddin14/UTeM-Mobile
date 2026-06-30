using UTeM_Mobile.Core.Models;
using UTeM_Mobile.Data.Models;

namespace UTeM_Mobile.Interfaces
{
    public interface ICheckpointService
    {
        Task<ObjectResponse<Checkpoint>> MarkCheckpointAsync(Checkpoint passedCheckpoint);
    }
}
