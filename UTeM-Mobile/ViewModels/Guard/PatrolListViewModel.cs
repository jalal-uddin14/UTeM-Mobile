using MvvmHelpers;
using MvvmHelpers.Commands;
using System.Windows.Input;
using UTeM_Mobile.Core.IServices;
using UTeM_Mobile.Core.Services;
using UTeM_Mobile.Data.Models;
using UTeM_Mobile.Interfaces;
using UTeM_Mobile.Core.Models;
using UTeM_Mobile.Views.Guard;

namespace UTeM_Mobile.ViewModels.Guard
{
    public class PatrolListViewModel : MainViewModel, IOnAppearing
    {
        private IGenericService<Patrol> _genericService;
        private IGenericService<ApplicationUser> _genericUserService;
        private ApplicationUser user;

        public ICommand NavigateToSendSoSCommand { get; }
        public ObservableRangeCollection<Patrol> PatrolList { get; }
        public ApplicationUser User { get => user; set => SetProperty(ref user, value); }

        public PatrolListViewModel()
        {
            _genericService = new GenericService<Patrol>();
            _genericUserService = new GenericService<ApplicationUser>();
            PatrolList = new ObservableRangeCollection<Patrol>();
            NavigateToSendSoSCommand = new AsyncCommand(ExecuteNavigateToSendSoSAsync);
        }
        private async Task ExecuteNavigateToSendSoSAsync()
        {
            await Shell.Current.GoToAsync($"//{nameof(ReportSendPage)}");
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
                    await GetPatrolList();
                    await GetUserDetailAsync();
                }
            }
            catch (Exception ex)
            {
                SetErrorMessage("Internal error occured.");
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

        private async Task GetPatrolList()
        {
            try
            {
                PatrolList.Clear();
                IsBusy = true;
                string url = "patrols";
                PaginatedResponse<Patrol> response = await _genericService.GetPagedListAsync(url, Token);
                if (response.IsSuccess && response.Data != null && response.Data.Data != null)
                {
                    PatrolList.AddRange(response.Data.Data);
                }
                else
                {
                    SetErrorMessage(response.Message, response.Errors);
                }
            }
            catch(Exception ex)
            {
                PatrolList.Clear();
                SetErrorMessage("Internal error occured.");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
