using MvvmHelpers;
using UTeM_Mobile.Core.IServices;
using UTeM_Mobile.Core.Models;
using UTeM_Mobile.Core.Services;
using UTeM_Mobile.Core.Services.DBServices;
using UTeM_Mobile.Data.Models;
using UTeM_Mobile.Interfaces;

namespace UTeM_Mobile.ViewModels.Supervisor
{
    [QueryProperty(nameof(Id), "Id")]
    public class PatrolDetailListViewModel : MainViewModel, IOnAppearing
    {
        private IGenericService<Patrol> _genericPatrolService;
        private IGenericService<PatrolDetail> _genericPatrolDetailService;
        private string id;
        private Patrol patrol;

        public ObservableRangeCollection<PatrolDetail> PatrolDetailList { get; set; }
        public string Id { get => id; set => id = value; }
        public Patrol Patrol { get => patrol; set => SetProperty(ref patrol, value); }

        public PatrolDetailListViewModel()
        {
            Patrol = new Patrol();
            PatrolDetailList = new ObservableRangeCollection<PatrolDetail>();
            _genericPatrolService = new GenericService<Patrol>();
            _genericPatrolDetailService = new GenericService<PatrolDetail>();
        }

        public void OnAppearing()
        {
            Task.Run(async () => { await GetTokenAsync(); });
        }

        public async Task GetTokenAsync()
        {
            try
            {
                Token = await LocalDBService.GetToken();
                if (Token != null)
                {
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
            catch (Exception)
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
