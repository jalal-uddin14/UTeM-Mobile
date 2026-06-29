using System.Text.Json;
using UTeM_Mobile.Core.IServices;
using UTeM_Mobile.Core.Models;
using UTeM_Mobile.Core.Services;
using UTeM_Mobile.Core.Services.DBServices;
using UTeM_Mobile.Data.Models;
using UTeM_Mobile.Interfaces;

namespace UTeM_Mobile.ViewModels.Guard
{
    [QueryProperty(nameof(Id), "Id")]
    public class PatrolDetailListViewModel : MainViewModel, IOnAppearing
    {
        private IGenericService<Patrol> _genericPatrolService;
        private IGenericService<PatrolDetail> _genericPatrolDetailService;
        private IGenericService<ApplicationUser> _genericUserService;

        private readonly ITokenStorageService _tokenService;
        private ApplicationUser user;

        private string id;
        private Patrol patrol;
        private PatrolDetail patrolDetail;

        public ApplicationUser User { get => user; set => SetProperty(ref user, value); }
        public string Id { get => id; set => id = value; }
        public Patrol Patrol { get => patrol; set => SetProperty(ref patrol, value); }
        public PatrolDetail PatrolDetail { get => patrolDetail; set => SetProperty(ref patrolDetail, value); }

        public PatrolDetailListViewModel(ITokenStorageService storageService)
        {
            _genericPatrolService = new GenericService<Patrol>();
            _genericPatrolDetailService = new GenericService<PatrolDetail>();
            _genericUserService = new GenericService<ApplicationUser>();
            Patrol = new Patrol();
            _tokenService = storageService;
        }

        public async Task OnAppearing()
        {
            IsErrorMessage = false;
            await GetTokenAsync();
        }
        public async Task GetTokenAsync()
        {
            try
            {
                var tokenJson = await _tokenService.GetAccessTokenAsync();
                if (tokenJson != null)
                {
                    await GetUserDetailAsync();
                    await GetPatrolDetailAsync();
                }
            }
            catch (Exception)
            {
                SetErrorMessage("Internal error occured.");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task GetUserDetailAsync()
        {
            try
            {
                string url = "accounts/me";
                ObjectResponse<ApplicationUser> response = await _genericUserService.GetDetailsAsync(url, Token);
                if (response.IsSuccess && response.Data != null)
                {
                    User = response.Data;
                }
                else
                {
                    SetErrorMessage(response.Message, response.Errors);
                }
            }
            catch (Exception ex)
            {
                SetErrorMessage("Internal error occured.");
            }
        }

        private async Task GetPatrolDetailAsync()
        {
            try
            {
                IsBusy = true;
                if (Id != null)
                {
                    string url = "patrols/" + Id;
                    ObjectResponse<Patrol> response = await _genericPatrolService.GetDetailsAsync(url, Token);
                    if (response.IsSuccess && response.Data != null)
                    {
                        Patrol = response.Data;
                    }
                    else
                    {
                        SetErrorMessage(response.Message, response.Errors);
                    }
                }
                else
                {
                    SetErrorMessage("Patrol not found.");
                }
            }
            catch (Exception ex)
            {
                SetErrorMessage("Internal error occured.");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
