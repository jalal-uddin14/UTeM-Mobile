using UTeM_Mobile.Core.IServices;
using UTeM_Mobile.Core.Models;
using UTeM_Mobile.Core.Services;
using UTeM_Mobile.Core.Services.DBServices;
using UTeM_Mobile.Data.Models;

namespace UTeM_Mobile.Services
{
    public class PatrolService
    {
        public static async Task<ObjectResponse<PatrolDetail>> GetPatrolStatus()
        {
            var token = await LocalDBService.GetToken();
            IGenericService<PatrolDetail> _genericPatrolDetailService = new GenericService<PatrolDetail>();
            string patrolDetailStatusUrl = "patrolDetails/status";
            return await _genericPatrolDetailService.PostAsync(patrolDetailStatusUrl, null, token);
        }

        public static async Task<RouteCheckpoint> CheckNextCheckpoint(int patrolId)
        {
            RouteCheckpoint routeCheckpoint = null;
            var token = await LocalDBService.GetToken();
            string url = "patrols/" + patrolId;
            IGenericService<Patrol> _genericPatrolService = new GenericService<Patrol>();
            ObjectResponse<Patrol> response = await _genericPatrolService.GetDetailsAsync(url, token);
            if (response.IsSuccess && response.Data != null)
            {
                Patrol patrol = response.Data;
                if (patrol.Route.RouteCheckpoints.Count == 0)
                {
                    return null;
                }
                else
                {
                    foreach (var item in patrol.Route.RouteCheckpoints)
                    {
                        bool isFound = patrol.PatrolCheckpoints.Where(p => p.CheckpointId == item.CheckpointId).FirstOrDefault() != null;
                        if (!isFound)
                        {
                            routeCheckpoint = item;
                            break;
                        }
                    }
                }
            }
            return routeCheckpoint;
        }
    }
}
