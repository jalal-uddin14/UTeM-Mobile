using UTeM_Mobile.Core.Models;
using UTeM_Mobile.Data.Models;

namespace UTeM_Mobile.Interfaces
{
    public interface IPatrolService
    {
        Task<ObjectResponse<PatrolDetail>> GetPatrolStatus();
        Task<Patrol> GetPatrol(int id);
        Task<PatrolDetail> GetPatrolDetail(int id);
    }
}
