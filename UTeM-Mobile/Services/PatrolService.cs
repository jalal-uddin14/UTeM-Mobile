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
            try
            {
                var token = await LocalDBService.GetToken();
                IGenericService<PatrolDetail> _genericPatrolDetailService = new GenericService<PatrolDetail>();
                string patrolDetailStatusUrl = "patrolDetails/status";
                return await _genericPatrolDetailService.PostAsync(patrolDetailStatusUrl, null, token);
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public static async Task<Patrol> GetPatrol(int id)
        {
            try
            {
                var token = await LocalDBService.GetToken();
                IGenericService<Patrol> _genericPatrolService = new GenericService<Patrol>();
                ObjectResponse<Patrol> objectResponse = await _genericPatrolService.GetDetailsAsync("patrols/" + id, token);
                if (objectResponse.IsSuccess && objectResponse.Data != null)
                {
                    return objectResponse.Data;
                }
                return null;
            }
            catch(Exception ex)
            {
                return null;
            }
        }
        public static async Task<PatrolDetail> GetPatrolDetail(int id)
        {
            try
            {
                var token = await LocalDBService.GetToken();
                IGenericService<PatrolDetail> _genericPatrolDetailService = new GenericService<PatrolDetail>();
                ObjectResponse<PatrolDetail> objectResponse = await _genericPatrolDetailService.GetDetailsAsync("patrolDetails/" + id, token);
                if (objectResponse.IsSuccess && objectResponse.Data != null)
                {
                    return objectResponse.Data;
                }
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
