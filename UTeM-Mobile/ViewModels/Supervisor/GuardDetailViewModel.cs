using UTeM_Mobile.Core.IServices;
using UTeM_Mobile.Core.Services;
using UTeM_Mobile.Data.Models;
using UTeM_Mobile.Core.Models;
using UTeM_Mobile.Interfaces;
using UTeM_Mobile.Core.Services.DBServices;

namespace UTeM_Mobile.ViewModels.Supervisor
{
    [QueryProperty(nameof(Id), "Id")]
    public class GuardDetailViewModel : MainViewModel, IOnAppearing
    {
        private IGenericService<ApplicationUser> _genericService;
        private string id;
        private ApplicationUser guard;

        public string Id { get => id; set => id = value; }
        public ApplicationUser Guard { get => guard; set => SetProperty(ref guard, value); }

        public GuardDetailViewModel()
        {
            _genericService = new GenericService<ApplicationUser>();
            Guard = new ApplicationUser();
        }

        public void OnAppearing()
        {
            IsErrorMessage = false;
            Task.Run(async () => { await GetTokenAsync(); });
        }

        public async Task GetTokenAsync()
        {
            try
            {
                Token = await LocalDBService.GetToken();
                if (Token != null)
                {
                    await GetGuardDetail();
                }
            }
            catch (Exception)
            {
                SetErrorMessage("Internal error occured");
            }
        }

        private async Task GetGuardDetail()
        {
            try
            {
                string url = "guards/" + Id;
                ObjectResponse<ApplicationUser> response = await _genericService.GetDetailsAsync(url, Token);
                if (response != null && response.Data != null)
                {
                    Guard = response.Data;
                }
                else
                {
                    SetErrorMessage(response.Message, response.Errors);
                }
            }
            catch (Exception)
            {
                SetErrorMessage("Internal error occured");
            }
        }
    }
}
