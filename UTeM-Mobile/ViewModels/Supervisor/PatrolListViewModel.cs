using MvvmHelpers;
using MvvmHelpers.Commands;
using System.Windows.Input;
using UTeM_Mobile.Core.IServices;
using UTeM_Mobile.Core.Services;
using UTeM_Mobile.Data.Models;
using UTeM_Mobile.Core.Models;
using UTeM_Mobile.Views.Supervisor;
using UTeM_Mobile.PopupViews;
using UTeM_Mobile.Models;
using UTeM_Mobile.Services;
using UTeM_Mobile.Interfaces;
using UTeM_Mobile.Core.Services.DBServices;

namespace UTeM_Mobile.ViewModels.Supervisor
{
    public class PatrolListViewModel : MainViewModel, IOnAppearing
    {
        private IGenericService<Patrol> _genericPatrolService;
        private IGenericService<PatrolDetail> _genericPatrolDetailService;
        private IGenericService<ApplicationUser> _genericUserService;
        private IGenericService<Shift> _genericShiftService;
        private IGenericService<Campus> _genericCampusService;
        private ApplicationUser user;
        private ApplicationUser selectedGuard;
        private DateTime? selectedStartDate;
        private DateTime? selectedEndDate;
        private Shift selectedShift;
        private Campus selectedCampus;

        private bool isFilterVisible;

        private bool isGuardNotVisible;
        private bool isGuardVisible;

        private bool isShiftNotVisible;
        private bool isShiftVisible;
        
        private bool isCampusNotVisible;
        private bool isCampusVisible;

        public ICommand ToggleFilterCommand { get; }
        public ICommand LogoutCommand { get; }
        public ICommand NavigateToGuardListCommand { get; set; }
        public ICommand NavigateToPatrolAddCommand { get; }
        public ICommand NavigateToProfileCommand { get; }
        public ObservableRangeCollection<Patrol> PatrolList { get; set; }
        public ObservableRangeCollection<PatrolDetail> PatrolDetaillList { get; set; }
        public ObservableRangeCollection<ApplicationUser> GuardList { get; set; }
        public ObservableRangeCollection<Shift> ShiftList { get; set; }
        public ObservableRangeCollection<Campus> CampusList { get; set; }
        public ApplicationUser User { get => user; set => SetProperty(ref user, value); }
        public ApplicationUser SelectedGuard
        {
            get => selectedGuard;
            set
            {
                SetProperty(ref selectedGuard, value);
                if (Token != null && SelectedGuard != null && PatrolList != null)
                {
                    Task.Run(async () => await GetPatrolListAsync());
                }
            }
        }
        public Shift SelectedShift
        {
            get => selectedShift;
            set
            {
                SetProperty(ref selectedShift, value);
                if (Token != null && selectedShift != null && PatrolList != null)
                {
                    Task.Run(async () => await GetPatrolListAsync());
                }
            }
        }
        public Campus SelectedCampus
        {
            get => selectedCampus;
            set
            {
                SetProperty(ref selectedCampus, value);
                if (Token != null && selectedCampus != null && PatrolList != null)
                {
                    Task.Run(async () => await GetPatrolListAsync());
                }
            }
        }

        public bool IsGuardNotVisible
        {
            get => isGuardNotVisible;
            set
            {
                SetProperty(ref isGuardNotVisible, value);
                IsGuardVisible = !value;
            }
        }
        public bool IsGuardVisible { get => isGuardVisible; set => SetProperty(ref isGuardVisible, value); }
        public DateTime? SelectedStartDate
        {
            get => selectedStartDate;
            set
            {
                SetProperty(ref selectedStartDate, value);
                if (Token != null)
                {
                    Task.Run(async() => await GetPatrolListAsync());
                }
            }
        }
        public DateTime? SelectedEndDate 
        { 
            get => selectedEndDate;
            set
            {
                SetProperty(ref selectedEndDate, value);
                if (Token != null)
                {
                    Task.Run(async () => await GetPatrolListAsync());
                }
            }
        }

        public bool IsShiftNotVisible 
        { 
            get => isShiftNotVisible;
            set
            {
                SetProperty(ref isShiftNotVisible, value);
                IsShiftVisible = !value;
            }
        }
        public bool IsShiftVisible { get => isShiftVisible; set => SetProperty(ref isShiftVisible, value); }
        public bool IsCampusNotVisible 
        { 
            get => isCampusNotVisible;
            set
            {
                SetProperty(ref isCampusNotVisible, value);
                IsCampusVisible = !value;
            }
        }
        public bool IsCampusVisible { get => isCampusVisible; set => SetProperty(ref isCampusVisible, value); }
        public bool IsFilterVisible { get => isFilterVisible; set => SetProperty(ref isFilterVisible, value); }

        public PatrolListViewModel()
        {
            IsFilterVisible = false;
            SelectedStartDate = DateTime.UtcNow.AddHours(8);
            SelectedEndDate = DateTime.UtcNow.AddHours(8);
            User = new ApplicationUser();
            SelectedGuard = new ApplicationUser();
            _genericPatrolService = new GenericService<Patrol>();
            _genericPatrolDetailService = new GenericService<PatrolDetail>();
            _genericUserService = new GenericService<ApplicationUser>();
            _genericShiftService = new GenericService<Shift>();
            _genericCampusService = new GenericService<Campus>();
            PatrolList = new ObservableRangeCollection<Patrol>();
            PatrolDetaillList = new ObservableRangeCollection<PatrolDetail>();
            GuardList = new ObservableRangeCollection<ApplicationUser>();
            ShiftList = new ObservableRangeCollection<Shift>();
            CampusList = new ObservableRangeCollection<Campus>();
            ToggleFilterCommand = new AsyncCommand(ExecuteToggleFilter);
            NavigateToGuardListCommand = new AsyncCommand(ExecuteNavigateToGuardList);
            NavigateToPatrolAddCommand = new AsyncCommand(ExecuteNavigateToPatrolAdd);
            NavigateToProfileCommand = new AsyncCommand(ExecuteNavigateToProfile);
            LogoutCommand = new AsyncCommand(ExecuteLogout);
        }

        private async Task ExecuteToggleFilter()
        {
            IsFilterVisible = !IsFilterVisible;
            await GetPatrolListAsync();
        }

        private async Task ExecuteLogout()
        {
            try
            {
                IsErrorMessage = false;
                await LogoutService.LogoutAsync();
            }
            catch (Exception)
            {
                await MainThread.InvokeOnMainThreadAsync(() =>
                    Application.Current.MainPage.Navigation.PushModalAsync(new MessagePopupPage(PopMessage.GetExceptionMessage()))
                );
                SetErrorMessage("Internal error occured.");
            }
        }
        private async Task ExecuteNavigateToGuardList()
        {
            await Shell.Current.GoToAsync($"{nameof(GuardListPage)}");
        }

        private async Task ExecuteNavigateToPatrolAdd()
        {
            await Shell.Current.GoToAsync($"{nameof(PatrolAddPage)}");
        }

        private async Task ExecuteNavigateToProfile()
        {
            await Shell.Current.GoToAsync($"{nameof(ProfilePage)}");
        }

        public void OnAppearing()
        {
            IsGuardNotVisible = true;
            IsErrorMessage = false;
            Task.Run(async () => { await GetTokenAsync(); });
        }

        public async Task GetTokenAsync()
        {
            try
            {
                IsBusy = true;
                Token = await LocalDBService.GetToken();
                if (Token != null)
                {
                    await GetProfileAsync();
                    await GetGuardListAsync();
                    await GetShiftListAsync();
                    await GetCampusListAsync();
                    await GetPatrolListAsync();
                }
            }
            catch(Exception)
            {
                SetErrorMessage("Internal error occured.");
            }
        }

        private async Task GetProfileAsync()
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
            catch(Exception)
            {
                SetErrorMessage("Internal error occured.");
            }
        }

        private async Task GetGuardListAsync()
        {
            try
            {
                IsGuardNotVisible = true;
                GuardList.Clear();
                string guardUrl = string.Format("guards?PageSize={0}", 100);
                PaginatedResponse<ApplicationUser> response = await _genericUserService.GetPagedListAsync(guardUrl, Token);
                if (response.IsSuccess && response.Data != null && response.Data.Data != null)
                {
                    GuardList.AddRange(response.Data.Data);
                    IsGuardNotVisible = false;
                }
                else
                {
                    SetErrorMessage(response.Message, response.Errors);
                }
            }
            catch (Exception)
            {
                GuardList.Clear();
                SetErrorMessage("Internal error occured.");
            }
        }

        private async Task GetShiftListAsync()
        {
            try
            {
                IsShiftNotVisible = true;
                ShiftList.Clear();
                string shiftUrl = string.Format("shifts?PageSize={0}", 100);
                PaginatedResponse<Shift> response = await _genericShiftService.GetPagedListAsync(shiftUrl, Token);
                if (response.IsSuccess && response.Data != null && response.Data.Data != null)
                {
                    ShiftList.AddRange(response.Data.Data);
                    IsShiftNotVisible = false;
                }
                else
                {
                    SetErrorMessage(response.Message, response.Errors);
                }
            }
            catch (Exception)
            {
                ShiftList.Clear();
                SetErrorMessage("Internal error occured.");
            }
        }

        private async Task GetCampusListAsync()
        {
            try
            {
                IsCampusNotVisible = true;
                CampusList.Clear();
                string campusUrl = string.Format("campuses?PageSize={0}", 100);
                PaginatedResponse<Campus> response = await _genericCampusService.GetPagedListAsync(campusUrl, Token);
                if (response.IsSuccess && response.Data != null && response.Data.Data != null)
                {
                    CampusList.AddRange(response.Data.Data);
                    IsCampusNotVisible = false;
                }
                else
                {
                    SetErrorMessage(response.Message, response.Errors);
                }
            }
            catch (Exception)
            {
                CampusList.Clear();
                SetErrorMessage("Internal error occured.");
            }
        }

        private async Task GetPatrolListAsync()
        {
            try
            {
                PatrolList.Clear();
                string guardId = SelectedGuard != null && SelectedGuard.Id != null ? SelectedGuard.Id : "";
                int? campusId = SelectedCampus?.Id;
                int? shiftId = SelectedShift?.Id;
                string url = string.Format("patrols");
                if (IsFilterVisible)
                {
                    url = string.Format("patrols?guardId={0}&&shiftId={1}&&campusId={2}&&startDate={3}&&endDate={4}", guardId, shiftId, campusId, SelectedStartDate?.ToString("yyyy-MM-dd"), SelectedEndDate?.ToString("yyyy-MM-dd"));
                }
                PaginatedResponse<Patrol> paginatedResponse = await _genericPatrolService.GetPagedListAsync(url, Token);
                if (paginatedResponse.IsSuccess && paginatedResponse.Data != null && paginatedResponse.Data.Data != null)
                {
                    PatrolList.AddRange(paginatedResponse.Data.Data);
                }
                else
                {
                    SetErrorMessage(paginatedResponse.Message, paginatedResponse.Errors);
                }
            }
            catch(Exception ex)
            {
                PatrolDetaillList.Clear();
                SetErrorMessage("Internal error occured.");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
