using System.Text.Json;
using UTeM_Mobile.Core.IServices;
using UTeM_Mobile.Core.Models;
using UTeM_Mobile.Core.Services;
using UTeM_Mobile.Data.Models;
using UTeM_Mobile.Interfaces;

namespace UTeM_Mobile.Services
{
    public class PatrolService : IPatrolService
    {
        private readonly ITokenStorageService _tokenService;
        public PatrolService(ITokenStorageService tokenService)
        {
            _tokenService = tokenService;
        }
        public async Task<ObjectResponse<PatrolDetail>> GetPatrolStatus()
        {
            try
            {
                var tokenJson = await _tokenService.GetAccessTokenAsync();
                if (string.IsNullOrEmpty(tokenJson))
                {
                    return null;
                }
                var token = JsonSerializer.Deserialize<AuthToken>(tokenJson);
                IGenericService<PatrolDetail> _genericPatrolDetailService = new GenericService<PatrolDetail>();
                string patrolDetailStatusUrl = "patrolDetails/status";
                return await _genericPatrolDetailService.PostAsync(patrolDetailStatusUrl, null, token);
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public async Task<Patrol> GetPatrol(int id)
        {
            try
            {
                var tokenJson = await _tokenService.GetAccessTokenAsync();
                if (string.IsNullOrEmpty(tokenJson))
                {
                    return null;
                }
                var token = JsonSerializer.Deserialize<AuthToken>(tokenJson);
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
        public async Task<PatrolDetail> GetPatrolDetail(int id)
        {
            try
            {
                var tokenJson = await _tokenService.GetAccessTokenAsync();
                if (string.IsNullOrEmpty(tokenJson))
                {
                    return null;
                }
                var token = JsonSerializer.Deserialize<AuthToken>(tokenJson);
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
